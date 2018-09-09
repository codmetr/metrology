﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchiveData.DTO;
using DPI620Genii;
using KipTM.EventAggregator;
using KipTM.Interfaces;
using KipTM.Model.Checks;
using NLog;
using PressureSensorCheck.Check;
using PressureSensorCheck.Devices;
using PressureSensorData;
using Tools.View.ModalContent;

namespace PressureSensorCheck.Workflow
{
    public class PressureSensorRunPresenter:IObserver<MeasuringPoint>, IDisposable
    {
        private readonly PressureSensorRunVm1 _vm;
        private readonly IDPI620Driver _dpi620;
        private DPI620GeniiConfig _dpiConf;
        private readonly Logger _logger;
        private CancellationTokenSource _cancellation = new CancellationTokenSource();
        private DateTime? _startTime = null;
        private readonly ManualResetEvent _autorepeatWh = new ManualResetEvent(true);
        private readonly TimeSpan _periodAutoread = TimeSpan.FromMilliseconds(100);
        private readonly PressureSensorConfig _config;
        private AutoUpdater _autoupdater;
        private Action<Action> _invoker = act => act();
        private PressureSensorResult _result;
        private DateTime? _lastResultTime;
        private readonly IEventAggregator _agregator;
        private bool _isRun;


        public PressureSensorRunPresenter(PressureSensorRunVm1 vm, IEventAggregator agregator)
        {
            _vm = vm;
            _vm.CallAddPoint += VmOnCallAddPoint;
            _vm.CallPauseCheck += VmOnCallPauseCheck;
            _vm.CallStartCheck += VmOnCallStartCheck;
            _vm.CallStopCheck += VmOnCallStopCheck;
            _agregator = agregator;
        }

        private void VmOnCallAddPoint()
        {
            DoAddPoint();
        }

        private void VmOnCallStartCheck()
        {
            DoStart();
        }

        private void VmOnCallPauseCheck()
        {
            //throw new NotImplementedException();
        }

        private void VmOnCallStopCheck()
        {
            DoStop();
        }


        /// <summary>
        /// Текущий результат
        /// </summary>
        public PressureSensorResult Result
        {
            get { return _result; }
        }


        /// <summary>
        /// Время последних результатов
        /// </summary>
        public DateTime? LastResultTime
        {
            get { return _lastResultTime; }
        }

        private void DoAddPoint()
        {
            _vm.Points.Add(new PointViewModel()
            {
                Config = new PointConfigViewModel()
                {
                    Pressure = _vm.NewConfig.Pressure,
                    I = _vm.NewConfig.I,
                    dI = _vm.NewConfig.dI,
                    Unit = _config.Unit,
                },
                Result = new PointResultViewModel()
            });
            _config.Points.Add(new PressureSensorPoint()
            {
                PressurePoint = _vm.NewConfig.Pressure,
                OutPoint = _vm.NewConfig.I,
                Tollerance = _vm.NewConfig.dI,
                PressureUnit = _config.Unit,
                OutUnit = Units.mA
            });
        }

        private void DoStart()
        {
            if (_startTime != null)
                return;
            IsRun = true;
            // подготовка внутрених переменных к старту проверки
            _startTime = DateTime.Now;
            var cancel = _cancellation.Token;
            // запуск самой проверки
            var task = new Task(() =>
            {
                try
                {
                    var check = Config(cancel);
                    if (check == null)
                        return;
                    if (cancel.IsCancellationRequested)
                        return;
                    TryStart(check, cancel);
                }
                catch (Exception ex)
                {
                    Log($"Config error: {ex.ToString()}");
                }
                finally
                {
                    IsRun = false;
                    _startTime = null;
                }
            });
            task.Start(TaskScheduler.Default);
        }

        public bool IsRun
        {
            get { return _isRun; }
            private set
            {
                _isRun = value;
                _vm.IsRun = value;
            }
        }

        /// <summary>
        /// Точки проверки с результатом
        /// </summary>
        public IEnumerable<PointViewModel> Points { get { return _vm.Points; } }

        /// <summary>
        /// Конфигурация оборудования и запуск автоопроса
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        private PresSensorCheck Config(CancellationToken cancel)
        {
            _autorepeatWh.Reset();

            // выбор входных и выходных слотов
            DPI620GeniiConfig.DpiSlotConfig inSlot;
            DPI620GeniiConfig.DpiSlotConfig outSlot;
            int inSlotNum;
            int outSlotNum;
            if (_dpiConf.Slot1.ChannelType == ArchiveData.DTO.ChannelType.Pressure)
            {
                inSlot = _dpiConf.Slot1;
                outSlot = _dpiConf.Slot2;
                inSlotNum = 1;
                outSlotNum = 2;
            }
            else if (_dpiConf.Slot2.ChannelType == ArchiveData.DTO.ChannelType.Pressure)
            {
                inSlot = _dpiConf.Slot2;
                outSlot = _dpiConf.Slot1;
                inSlotNum = 2;
                outSlotNum = 1;
            }
            else
            {
                ShowMessage("Укажите конфигурацию DPI620Genii на вкладке \"Настройка\": укажите какие датчики в каких слотах", cancel);
                return null;
            }

            // попытка подключения к DPI 620
            try
            {
                var dpiCom = _dpi620 as DPI620DriverCom;
                if (dpiCom != null)
                {
                    dpiCom.SetPort(_dpiConf.SelectPort);
                }
                _dpi620.Open();
            }
            catch (Exception ex)
            {
                Log($"OpenPort (_dpiConf.SelectPort) error: {ex.ToString()}");
                ShowMessage("Не удалось связаться с DPI620Genii. Укажите конфигурацию DPI620Genii на вкладке \"Настройка\": укажите порт подключения", cancel);
                return null;
            }

            // запуск авточтения состояния
            var tUpdate = new Task((arg) => _autoupdater.Start(_dpi620, (AutoUpdater.AutoreadState)arg, _config),
                new AutoUpdater.AutoreadState(cancel, _periodAutoread, _startTime.Value, _autorepeatWh),
                cancel, TaskCreationOptions.None);
            tUpdate.Start(TaskScheduler.Default);
            var checkLogger = NLog.LogManager.GetLogger(string.Format("Check{0}",
                _startTime.Value.ToString("yy.MM.dd_hh:mm:ss.fff")));

            // конфигурирование шагов проверки
            var check = new PresSensorCheck(checkLogger, null,
                new DPI620Ethalon(_dpi620, inSlotNum, ChannelType.Pressure, inSlot.SelectedUnit, _config.Unit),
                new DPI620Ethalon(_dpi620, outSlotNum, ChannelType.Current, outSlot.SelectedUnit, Units.mA), _result);
            check.ChConfig.UsrChannel = new PressureSensorUserChannel(_vm);
            check.FillSteps(_config);
            _vm.CLearAllLines();
            return check;
        }

        /// <summary>
        /// Выполнение проверки с гарантированым снятием признака запуска проверки
        /// </summary>
        /// <param name="check"></param>
        /// <param name="cancel"></param>
        private void TryStart(PresSensorCheck check, CancellationToken cancel)
        {
            try
            {
                using (_autoupdater.Subscribe(this))
                {
                    check.ResultUpdated += CheckOnResultUpdated;
                    check.EndMethod += CheckOnEndMethod;
                    _agregator.Send(new EventArgRunState(true));
                    if (check.Start(cancel))
                        if (!cancel.IsCancellationRequested)
                            UpdateResult(check.Result);
                }
            }
            catch (Exception ex)
            {
                Log($"TryStart exception: {ex.ToString()}");
            }
            finally
            {
                _agregator.Send(new EventArgRunState(false));
                check.ResultUpdated -= CheckOnResultUpdated;
                check.EndMethod += CheckOnEndMethod;
            }
        }

        private void CheckOnEndMethod(object sender, EventArgs eventArgs)
        {
            _invoker(() =>
            {
                _vm.ResetSetAcceptAction();
                _vm.Note = "";
                _vm.IsAsk = false;
            });
        }

        private void CheckOnResultUpdated(object sender, EventArgs eventArgs)
        {
            var check = sender as PresSensorCheck;
            if (check != null)
                UpdateResult(check.Result);
        }

        /// <summary>
        /// Обновить состояние визуальной модели результата
        /// </summary>
        /// <param name="checkResult"></param>
        private void UpdateResult(PressureSensorResult checkResult)
        {
            _result = checkResult;
            foreach (var point in _vm.Points)
            {
                var res = checkResult.Points.FirstOrDefault(el => Math.Abs(el.PressurePoint - point.Config.Pressure) < double.Epsilon);
                if (res == null)
                    continue;
                if (point.Result == null)
                    point.Result = new PointResultViewModel();

                if (double.IsNaN(res.VoltageValueBack))
                { // прямой ход
                    point.Result.IReal = res.VoltageValue;
                    point.Result.dIReal = res.VoltageValue - res.VoltagePoint;
                    point.Result.IsCorrect = point.Result.dIReal <= point.Config.dI;
                }
                else
                { // обратный ход
                    point.Result.Iback = res.VoltageValueBack;
                    point.Result.dIvar = Math.Abs(res.VoltageValue - res.VoltageValueBack);
                }
            }
            _lastResultTime = DateTime.Now;
        }

        /// <summary>
        /// Показать сообщение пользователю
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cancel"></param>
        private void ShowMessage(string msg, CancellationToken cancel)
        {
            _vm.AskModal("", msg, cancel);
        }


        private void Log(string msg)
        {
            _logger?.Trace(msg);
        }

        /// <summary>
        /// Приостановить проверку
        /// </summary>
        private void DoPause()
        {
            throw new NotImplementedException();
        }

        private void DoStop()
        {
            IsRun = false;
            _cancellation.Cancel();
            _cancellation = new CancellationTokenSource();
            _autorepeatWh.WaitOne();
            _dpi620.Close();
            _startTime = null;
        }

        #region IObserver<MeasuringPoint>

        public void OnNext(MeasuringPoint value)
        {
            _vm.AddToLine(value.TimeStamp, value.I, value.Pressure);
            _vm.LastMeasuredPoint = value;
            Log($"Readed repeat: P:{value.Pressure} {_config.Unit}");
        }

        public void OnError(Exception error)
        {
            Log(error.ToString());
        }

        public void OnCompleted()
        {
            Log("Measuring complite");
        }

        #endregion

        public void Dispose()
        {
            if (!IsRun)
                return;
            _cancellation.Cancel();
            _autorepeatWh.WaitOne(TimeSpan.FromSeconds(10));
            _dpi620.Close();
        }

        /// <summary>
        /// Обновить набор точек в проверке по конфигурации
        /// </summary>
        /// <param name="points"></param>
        public void UpdatePoint(IEnumerable<PointConfigViewModel> points)
        {
            _vm.Points.Clear();
            foreach (var point in points)
            {
                var resPoint = _vm.Points.FirstOrDefault(el => Math.Abs(el.Config.Pressure - point.Pressure) < Double.Epsilon);
                if (resPoint == null)
                    _vm.Points.Add(new PointViewModel() { Config = point, Result = new PointResultViewModel() });
                else
                {
                    resPoint.Config.Unit = point.Unit;
                    resPoint.Config.I = point.I;
                    resPoint.Config.dI = point.dI;
                    resPoint.Config.Ivar = point.Ivar;
                }
            }
        }
    }
}
