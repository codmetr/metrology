using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using PACESeries;

namespace PACETool
{
    class Pace5000Model
    {
        private IPace5000Vm _vm;
        private ConnectionPannelVm _connectionVm;
        private PACE1000Driver _pace;
        private PortCaller _portCaller;
        private CancellationTokenSource _cancellation = new CancellationTokenSource();
        private SerialPort _port = null;
        private Logger _logger;

        public Pace5000Model(Pace5000Vm vm, ConnectionPannelVm connectionVm)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _vm = vm;
            _vm.CallStartAutoUpdate += VmOnCallStartAutoUpdate;
            _vm.CallStopAutoUpdate += VmOnCallStopAutoUpdate;
            _vm.CallUpdatePressureAndUnits += VmOnCallUpdatePressureAndUnits;
            _vm.CallUpdateUnits += VmOnCallUpdateUnits;
            _vm.CallSetSelectedUnit += VmOnCallSetSelectedUnit;
            _vm.CallSetLloOn += VmOnCallSetLloOn;
            _vm.CallSetLloOff += VmOnCallSetLloOff;
            _vm.CallSetLocal += VmOnCallSetLocal;
            _vm.CallSetRemote += VmOnCallSetRemote;
            _vm.CallToControll += VmOnCallToControll;
            _vm.CallToMeasuring += VmOnCallToMeasuring;
            _vm.CallSetAim += VmOnCallSetAim;
            _vm.CallReadAim += VmOnCallReadAim;
            _connectionVm = connectionVm;
            _connectionVm.CallSwitchConnect += ConnectionVmOnCallSwitchConnect;

            _pace = null;
        }

        private void VmOnCallStopAutoUpdate()
        {
            _portCaller.StopAutoupdate();
        }

        private void VmOnCallStartAutoUpdate(TimeSpan timeSpan)
        {
            _portCaller.AddToAutoupdate(UpdatePressureAndUnits, timeSpan);
        }

        private void UpdatePressureAndUnits()
        {
            var pressure = _pace.GetPressure();
            var unit = _pace.GetPressureUnit();
            if(double.IsNaN(pressure) || unit == null)
                throw new Exception("Read Pressure and Unit error");
            _vm.SetPressure(pressure);
            _vm.SetUnit(unit.Value);
        }

        private void VmOnCallSetAim(double d)
        {
            _portCaller.CallSync(()=> {
                _pace.SetPressure(d);
                _vm.SetAimVal(d);
            });
        }

        private void VmOnCallReadAim()
        {
            _portCaller.CallSync(() =>
            {
                double aim;
                _pace.GetAimPressure(out aim);
                _vm.SetAimVal(aim);
            });
        }

        private void VmOnCallToMeasuring()
        {
            _portCaller.CallSync(() => {
                _pace.SetOutputState(false);
            _vm.SetPaceMode(false);
            });
        }

        private void VmOnCallToControll()
        {
            _portCaller.CallSync(() => {
                _pace.SetOutputState(true);
            _vm.SetPaceMode(true);
            });
        }

        private void VmOnCallSetRemote()
        {
            _portCaller.CallSync(() => {
                _pace.SetRemote();
            });
        }

        private void VmOnCallSetLocal()
        {
            _portCaller.CallSync(() => {
                _pace.SetLocal();
            });
        }

        private void VmOnCallSetLloOff()
        {
            _portCaller.CallSync(() => {
                _pace.SetOffLocalLockOutMode();
            });
        }

        private void VmOnCallSetLloOn()
        {
            _portCaller.CallSync(() => {
                _pace.SetLocalLockOutMode();
            });
        }

        private void VmOnCallSetSelectedUnit(PressureUnits pressureUnits)
        {
            _portCaller.CallSync(() => {
                _pace.SetPressureUnit(pressureUnits);
            });
        }

        private void VmOnCallUpdateUnits()
        {
            _portCaller.CallSync(() => {
                var unit = _pace.GetPressureUnit();
                if (unit == null)
                    throw new Exception("Read Unit error");
                _vm.SetUnit(unit.Value);
            });
            throw new NotImplementedException();
        }

        private void VmOnCallUpdatePressureAndUnits()
        {
            UpdatePressureAndUnits();
        }

        private void ConnectionVmOnCallSwitchConnect(bool isConnected, ConnectionPannelVm.ConfigConnnection config)
        {
            if (!isConnected)
            {
                _portCaller = new PortCaller(_cancellation.Token);
                _port = new SerialPort(config.Port, config.Rate, config.Parity, config.DataBits, config.StopBits);
                _port.NewLine = "\r";
                _port.ReadTimeout = 1000;
                _port.WriteTimeout = 1000;
                //_port.Handshake = Handshake.None;
                //_port.DtrEnable = false;
                //_port.RtsEnable = false;
                _port.Open();
                _pace = new PACE1000Driver(_port, Log);
                _connectionVm.SetOpened(true);
            }
            else
            {
                Cancel();
                _portCaller.StopAutoupdate();
                _portCaller = null;
                _port.Close();
                _pace = null;
                _connectionVm.SetOpened(false);
            }
        }

        private void Cancel()
        {
            _cancellation.Cancel();
            _cancellation = new CancellationTokenSource();
        }

        private void Log(string obj)
        {
            _logger.Trace(obj);
        }
    }
}
