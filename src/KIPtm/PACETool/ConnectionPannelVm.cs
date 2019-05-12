using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Tools.View;

namespace PACETool
{
    class ConnectionPannelVm: BaseVm
    {
        public struct ConfigConnnection
        {
            public readonly string Port;
            public readonly int Rate;
            public readonly Parity Parity;
            public readonly int DataBits;
            public readonly StopBits StopBits;

            public ConfigConnnection(string port, int rate, Parity parity, int dataBits, StopBits stopBits)
            {
                Port = port;
                Rate = rate;
                Parity = parity;
                DataBits = dataBits;
                StopBits = stopBits;
            }
        }

        private bool _isOpened;
        private readonly ObservableCollection<string> _ports = new ObservableCollection<string>();
        private readonly IEnumerable<int> _boudRates;
        private readonly IEnumerable<Tuple<string, Parity>> _parites;
        private readonly IEnumerable<int> _dataBites;
        private readonly IEnumerable<Tuple<float, StopBits>> _stopBits;

        public ConnectionPannelVm(Dispatcher disp) : base(disp)
        {
            UpdateComPorts();
            _boudRates = new[]
            {
                110, 300, 600, 1200, 2400, 4800, 9600, 14400,
                19200, 28800, 38400, 56000, 57600, 115200
            };
            _parites = new[]
            {
                new Tuple<string, Parity>("Четность", Parity.Even),
                new Tuple<string, Parity>("Нечетность", Parity.Odd),
                new Tuple<string, Parity>("Нет", Parity.None)
                //new Tuple<string, Parity>("Последний бит 1", Parity.Mark),
                //new Tuple<string, Parity>("Последний бит 0", Parity.Space)
            };
            _dataBites = new[] { 7, 8, 9 };
            _stopBits = new[]
            {
                //new Tuple<float, StopBits>(0f, StopBits.None), // StopBits.None Значение не поддерживается(MSDN)
                new Tuple<float, StopBits>(1f, StopBits.One),
                new Tuple<float, StopBits>(1.5f, StopBits.OnePointFive), // Параметр задан неверно ??????????
                new Tuple<float, StopBits>(2, StopBits.Two)
            };
            BaseConfig();
        }

        private void BaseConfig()
        {
            SelectedPort = _ports.FirstOrDefault();
            SelectedBaudRate = 9600;
            SelectedParity = new Tuple<string, Parity>("Нет", Parity.None);
            SelectedDataBits = 8;
            SelectedStopBits = new Tuple<float, StopBits>(1f, StopBits.One);
        }

        public bool IsOpened
        {
            get { return _isOpened; }
            set
            {
                _isOpened = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Доступные порты
        /// </summary>
        public ObservableCollection<string> ComPorts { get { return _ports; } }
        /// <summary>
        /// Доступные наборы скоростей
        /// </summary>
        public IEnumerable<int> BoudRateSet { get { return _boudRates; } }
        /// <summary>
        /// Доступная колекция правил формирования бита четности
        /// </summary>
        public IEnumerable<Tuple<string, Parity>> PariteSet { get { return _parites; } }
        /// <summary>
        /// Доступная колекция длин данных
        /// </summary>
        public IEnumerable<int> DataBitesSet { get { return _dataBites; } }
        /// <summary>
        /// Дотупная колекция стоп битов
        /// </summary>
        public IEnumerable<Tuple<float, StopBits>> StopBitsSet { get { return _stopBits; } }

        public string SelectedPort { get; set; }
        public int SelectedBaudRate { get; set; }
        public Tuple<string, Parity> SelectedParity { get; set; }
        public int SelectedDataBits { get; set; }
        public Tuple<float, StopBits> SelectedStopBits { get; set; }

        public ICommand SwitchConnect
        {
            get
            {
                return new CommandWrapper(() => OnCallSwitchConnect(IsOpened,
                    new ConfigConnnection(SelectedPort, SelectedBaudRate, SelectedParity.Item2, SelectedDataBits,
                        SelectedStopBits.Item2)));
            }
        }

        #region Public interface

        public event Action<bool, ConfigConnnection> CallSwitchConnect;

        public void SetOpened(bool isIpened)
        {
            Sync(()=>IsOpened = isIpened);
        }

        #endregion

        protected virtual void OnCallSwitchConnect(bool isOpen, ConfigConnnection conf)
        {
            CallSwitchConnect?.Invoke(isOpen, conf);
        }

        private void UpdateComPorts()
        {
            var ports = SerialPort.GetPortNames();
            foreach (var portName in ports)
            {
                _ports.Add(portName);
            }
        }
    }
}
