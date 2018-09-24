using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEEE488;
using PACESeries;
using QueryLoop;

namespace PACESeriesUtil
{
    public class PacePresenter
    {
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

            _vm.Config.Connect += Config_Connect;
            _vm.Config.Disсonnect += Config_Disсonnect;
        }

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
    }
}
