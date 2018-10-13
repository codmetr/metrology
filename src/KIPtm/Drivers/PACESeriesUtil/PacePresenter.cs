using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IEEE488;
using PACESeries;
using QueryLoop;

namespace PACESeriesUtil
{
    public class PacePresenter:IDisposable
    {
        private CancellationTokenSource _cancellation = new CancellationTokenSource();
        private PaceViewModel _vm;
        private IContext _context;
        private PACE1000Driver _pase;
        private Model _model;
        private ILoops _syncPort;
        private string _lockKey = "IEE488";
        private ITransportIEEE488 _transport;

        public PacePresenter(IContext context, PaceViewModel vm)
        {
            _context = context;
            _vm = vm;
            _transport = new VisaIEEE488();
            _syncPort = new Loops();
            _syncPort.AddLocker(_lockKey, new object());

            _vm.ControlState.Units = Enum.GetValues(typeof(PressureUnits)).Cast<PressureUnits>();

            _vm.Config.Connect += Config_Connect;
            _vm.Config.Disсonnect += Config_Disсonnect;
            _vm.ControlState.EvSetLimit += ControlState_EvSetLimit;
            _vm.ControlState.EvSetPress += ControlState_EvSetPress;
            _vm.ControlState.EvSetUnit += ControlState_EvSetUnit;
        }

        #region Control

        private void ControlState_EvSetUnit(PressureUnits unit)
        {
            var token = _cancellation.Token;
            _syncPort.StartImportantAction(_lockKey, (arg)=>SetUnit(unit, token));
        }

        private void ControlState_EvSetPress(string obj)
        {
            var token = _cancellation.Token;
            var press = 0d;
            if(!double.TryParse(obj, NumberStyles.Any, CultureInfo.CurrentUICulture, out press))
                return;
            _syncPort.StartImportantAction(_lockKey, (arg) => SetPress(press, token));
        }

        private void ControlState_EvSetLimit(string obj)
        {
            var token = _cancellation.Token;
            _syncPort.StartImportantAction(_lockKey, (arg) => SetLimit(obj, token));
        }

        private void SetLimit(string limit, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _pase.SetPressureRange(limit);
        }

        private void SetUnit(PressureUnits unit, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _pase.SetPressureUnit(unit);
        }

        private void SetPress(double press, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _pase.SetPressure(press);
        }

        #endregion

        #region Config

        private void Config_Connect()
        {
            var isConnect = false;
            try
            {
                _pase = new PACE1000Driver(_vm.Config.Address, _transport);
                isConnect = _pase.Open();
            }
            catch (Exception e)
            {
                isConnect = false;
            }
            _context.Invoke(()=>_vm.Config.IsConnected = isConnect);
        }

        private void Config_Disсonnect()
        {
            if(_pase == null)
                return;
            _pase.Dispose();
            _context.Invoke(() => _vm.Config.IsConnected = false);
        }

        #endregion

        public void Dispose()
        {
            _syncPort.Dispose();
        }
    }
}
