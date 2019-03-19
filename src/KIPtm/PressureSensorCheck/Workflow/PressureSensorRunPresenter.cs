using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchiveData.DTO;
using DPI620Genii;
using KipTM.EventAggregator;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Model.Checks;
using NLog;
using PressureSensorCheck.Check;
using PressureSensorCheck.Devices;
using PressureSensorCheck.Workflow.Content;
using PressureSensorData;
using Tools.View.ModalContent;

namespace PressureSensorCheck.Workflow
{
    public class PressureSensorRunPresenter : IObserver<MeasuringPoint>, IDisposable
    {
        private readonly PressureSensorRunVm _vm;
        private readonly IDPI620Driver _dpi620; //TODO Init
        private DPI620GeniiConfig _dpiConf; //TODO Init
        private readonly Logger _logger; //TODO Init
        private CancellationTokenSource _cancellation = new CancellationTokenSource();
        private DateTime? _startTime = null;
        private readonly ManualResetEvent _autorepeatWh = new ManualResetEvent(true);
        private readonly TimeSpan _periodAutoread = TimeSpan.FromMilliseconds(100);
        private readonly PressureSensorConfig _config; //TODO Init
        private AutoUpdater _autoupdater; //TODO Init
        private PressureSensorResult _result;
        private DateTime? _lastResultTime;
        private readonly IEventAggregator _agregator;
        private bool _isRun;
        private readonly IContext _context;
        private IEtalonSourceChannel<Units> _pressureSrc;
        private SerialPort _port = null;


        public PressureSensorRunPresenter(PressureSensorRunVm vm, PressureSensorConfig config, IDPI620Driver dpi620, DPI620GeniiConfig dpiConf, PressureSensorResult result, IEventAggregator agregator, IContext context)
        {
            _vm = vm;
            //_vm.CallAddPoint += DoAddPoint;
            _vm.CallPauseCheck += DoPause;
            _vm.CallStartCheck += DoStart;
            _vm.CallStopCheck += DoStop;
            _config = config;
            _dpi620 = dpi620;
            _dpiConf = dpiConf;
            _result = result;
            _agregator = agregator;
            _context = context;

            _logger = NLog.LogManager.GetLogger("PressureSensorRun");
            _autoupdater = new AutoUpdater(_logger);
            _pressureSrc = null;
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

        /// <summary>
        /// Признак "Проверка запущена"
        /// </summary>
        public bool IsRun
        {
            get { return _isRun; }
            private set
            {
                _isRun = value;
                _vm.SetIsRun(value);
            }
        }

        /// <summary>
        /// Точки проверки с результатом
        /// </summary>
        public IEnumerable<PointViewModel> Points { get { return _vm.Points; } }

        /// <summary>
        /// Обновить этолонный канал
        /// </summary>
        /// <param name="pressureSrc"></param>
        public void UpdateSourceChannel(IEtalonSourceChannel<Units> pressureSrc)
        {
            _pressureSrc = pressureSrc;
        }

        /// <summary>
        /// Обновить набор точек в проверке по конфигурации
        /// </summary>
        /// <param name="points"></param>
        public void UpdatePoint(IEnumerable<PressureSensorPointConf> points)
        {
            _vm.UpdatePoints(points);
        }

        /// <summary>
        /// Добавление точки
        /// </summary>
        //private void DoAddPoint()
        //{
        //    PointConfigViewModel newPointConf = null;

        //    _context.Invoke(() => newPointConf = _vm.NewConfig);

        //    var pointRes = new PointViewModel(_context)
        //    {
        //        Result = new PointResultViewModel()
        //    };
        //    pointRes.UpdateConf(newPointConf);
        //    var pointConf = new PressureSensorPoint()
        //    {
        //        PressurePoint = _vm.NewConfig.Pressure,
        //        OutPoint = _vm.NewConfig.I,
        //        Tollerance = _vm.NewConfig.dI,
        //        PressureUnit = _config.Unit,
        //        OutUnit = Units.mA
        //    };
        //    _context.Invoke(() =>
        //    {
        //        _vm.Points.Add(pointRes);
        //    });
        //    _config.Points.Add(pointConf);
        //}

        /// <summary>
        /// Запуск проверки
        /// </summary>
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

        /// <summary>
        /// Конфигурация оборудования и запуск автоопроса
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        private PresSensorCheck Config(CancellationToken cancel)
        {
            _autorepeatWh.Reset();

            // выбор входных и выходных слотов
            DpiSlotConfig inSlot;
            DpiSlotConfig outSlot;
            if (_dpiConf.Slot1.ChannelType == ArchiveData.DTO.ChannelType.Pressure)
            {
                inSlot = _dpiConf.Slot1;
                outSlot = _dpiConf.Slot2;
            }
            else if (_dpiConf.Slot2.ChannelType == ArchiveData.DTO.ChannelType.Pressure)
            {
                inSlot = _dpiConf.Slot2;
                outSlot = _dpiConf.Slot1;
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
                _port = new SerialPort(_dpiConf.SelectPort, 19200, Parity.None, 8, StopBits.One);
                if (dpiCom != null)
                {
                    dpiCom.SetPort(_port);
                }
                _port.Open();
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
            var check = new PresSensorCheck(checkLogger, _pressureSrc,
                new Dpi620Etalon(_dpi620, inSlot.SelectedSlotIndex, ChannelType.Pressure, inSlot.SelectedUnit, _config.Unit),
                new Dpi620Etalon(_dpi620, outSlot.SelectedSlotIndex, ChannelType.Current, outSlot.SelectedUnit, Units.mA), _result);
            check.ChConfig.UsrChannel = new PressureSensorUserChannel(_vm, _context);
            check.FillSteps(_config);
            _vm.ClearAllLines();
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

        /// <summary>
        /// Обработчик окончания проверки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void CheckOnEndMethod(object sender, EventArgs eventArgs)
        {
            _vm.ToBaseState();
        }

        /// <summary>
        /// Обработчик обновления результата
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
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
            _vm.UpdateResult(checkResult, _pressureSrc != null);
            _lastResultTime = DateTime.Now;
        }

        /// <summary>
        /// Показать сообщение пользователю
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cancel"></param>
        private void ShowMessage(string msg, CancellationToken cancel)
        {
            var wh = new ManualResetEvent(false);
            IDisposable modal = null;
            try
            {
                modal = _vm.ShowModalAsk("", msg, wh);
                WaitHandle.WaitAny(new[] {cancel.WaitHandle, wh});
            }
            finally
            {
                if(modal!=null)
                    modal.Dispose();
            }
        }

        /// <summary>
        /// Лог
        /// </summary>
        /// <param name="msg"></param>
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

        /// <summary>
        /// Остановить проверку
        /// </summary>
        private void DoStop()
        {
            IsRun = false;
            _cancellation.Cancel();
            _cancellation = new CancellationTokenSource();
            _autorepeatWh.WaitOne();
            _dpi620.Close();
            if(_port!=null)
                _port.Close();
            _startTime = null;
        }

        #region IObserver<MeasuringPoint>

        public void OnNext(MeasuringPoint value)
        {
            _vm.AddLastMeasured(value);
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

        #region IDisposable

        public void Dispose()
        {
            if (!IsRun)
                return;
            _cancellation.Cancel();
            _autorepeatWh.WaitOne(TimeSpan.FromSeconds(10));
            _dpi620.Close();
            if (_port != null)
                _port.Close();
        }

        #endregion
    }
}
