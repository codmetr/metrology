using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using ArchiveData;
using ArchiveData.DTO;
using KipTM.EventAggregator;
using KipTM.Interfaces;
using KipTM.Model.Checks;
using KipTM.ViewModel.Events;
using PressureSensorData;
using Tools.View;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Визуальная модель результата поверки датчика давления
    /// </summary>
    public class PressureSensorResultVM:INotifyPropertyChanged
    {
        /// <summary>
        /// Хранилище результата для конкретной проверки
        /// </summary>
        private readonly IDataAccessor _accessor;

        private PressureSensorConfig _conf;
        private PressureSensorResult _data;
        private string _assay;
        private string _leak;
        private string _commonResult;
        private string _visualCheckResult;
        private DateTime? _timeStamp;
        private readonly IEventAggregator _agregator;
        private readonly IContext _context;
        private bool _isSaveEnable = true;

        /// <summary>
        /// Визуальная модель результата поверки датчика давления
        /// </summary>
        /// <param name="checkResId"></param>
        /// <param name="accessor"></param>
        /// <param name="result"></param>
        /// <param name="conf"></param>
        /// <param name="agregator"></param>
        /// <param name="context"></param>
        public PressureSensorResultVM(TestResultID checkResId, IDataAccessor accessor, PressureSensorResult result, PressureSensorConfig conf, IEventAggregator agregator, IContext context)
        {
            Identificator = checkResId;
            _accessor = accessor;
            PointResults = new ObservableCollection<PointViewModel>();
            Data = result;
            _conf = conf;
            _agregator = agregator;
            _context = context;
            _agregator.Subscribe(this);
        }

        /// <summary>
        /// Текущий результат проверки
        /// </summary>
        public PressureSensorResult Data
        {
            get { return _data; }
            set { _data = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущий результат проверки
        /// </summary>
        public PressureSensorConfig Conf
        {
            get { return _conf; }
        }

        /// <summary>
        /// Идентифитатор проверки
        /// </summary>
        public TestResultID Identificator { get; }

        /// <summary>
        /// Результат опробирования
        /// </summary>
        public string Assay
        {
            get { return _assay; }
            set { _assay = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Результат проверки на герметичность
        /// </summary>
        public string Leak
        {
            get { return _leak; }
            set { _leak = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Общий результат поверки
        /// </summary>
        public string CommonResult
        {
            get { return _commonResult; }
            set { _commonResult = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Результат визуального осмотра
        /// </summary>
        public string VisualCheckResult
        {
            get { return _visualCheckResult; }
            set { _visualCheckResult = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PointViewModel> PointResults { get; set; }

        /// <summary>
        /// Дата протокола
        /// </summary>
        public DateTime? TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Операция сохранения доступна
        /// </summary>
        public bool IsSaveEnable
        {
            get { return _isSaveEnable; }
            set
            {
                _isSaveEnable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        public ICommand Save{ get { return new CommandWrapper(OnOnSaveCalled); } }

        /// <summary>
        /// Вызвано сохранение
        /// </summary>
        public event Action OnSaveCalled;

        public void SetPoints(IEnumerable<PressureSensorPoint> points)
        {
            var wh = new ManualResetEvent(false);
            _context.Invoke(() =>
            {
                PointResults.Clear();
                var i = 0;
                foreach (var point in points)
                {
                    var pointVm = new PointViewModel(_context)
                    {
                        Index = i++,
                        Config =
                        {
                            PressureUnit = point.Result.PressureUnit,
                            PressurePoint = point.Result.PressurePoint,
                            OutUnit = point.Result.VoltageUnit,
                            OutPoint = point.Result.VoltagePoint,
                            Tollerance = point.Config.Tollerance
                        },
                        Result =new PointResultViewModel()
                    };
                    if (point.Result == null)
                    {
                        PointResults.Add(pointVm);
                        continue;
                    }
                    pointVm.Result.PressureReal = point.Result.PressureValue;

                    if (double.IsNaN(point.Result.OutPutValue))
                    {
                        pointVm.Result.IReal = null;
                        pointVm.Result.dIReal = null;
                        pointVm.Result.dIvar = null;
                    }
                    else
                    {
                        pointVm.Result.IReal = point.Result.OutPutValue;
                        pointVm.Result.dIReal = point.Result.OutPutValue - point.Result.PressurePoint;
                    }

                    if (double.IsNaN(point.Result.OutPutValueBack))
                    {
                        pointVm.Result.Iback = null;
                        pointVm.Result.dIvar = null;
                    }
                    else
                    {
                        pointVm.Result.Iback = point.Result.OutPutValueBack;
                        pointVm.Result.dIvar = point.Result.OutPutValue - point.Result.OutPutValueBack;
                    }
                    pointVm.Result.IsCorrect = point.Result.IsCorrect;
                    PointResults.Add(pointVm);
                }
                TimeStamp = DateTime.Now;
                wh.Set();
            });
            wh.WaitOne();
        }

        /// <summary>
        /// Очистить список точек
        /// </summary>
        public void CleanPoints()
        {
            _context.Invoke(() => PointResults.Clear());
        }

        /// <summary>
        /// добавить точку
        /// </summary>
        /// <returns></returns>
        public PointViewModel AddPointResult()
        {
            var point = new PointViewModel(_context);
            var wh = new ManualResetEvent(false);
            _context.Invoke(() =>
            {
                PointResults.Add(point);
                wh.Set();
            });
            wh.WaitOne();
            return point;
        }

        /// <summary>
        /// Установить состояние 
        /// </summary>
        /// <param name="isSaveEnable"></param>
        public void SetIsSaveEnable(bool isSaveEnable)
        {
            _context.Invoke(()=>IsSaveEnable = isSaveEnable);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {

            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        protected virtual void OnOnSaveCalled()
        {
            OnSaveCalled?.Invoke();
        }
    }
}



