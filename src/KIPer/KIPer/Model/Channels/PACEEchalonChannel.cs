using System;
using System.Threading;
using KipTM.Model.Checks;
using KipTM.Model.Devices;

namespace KipTM.Model.Channels
{
    public class PACEEchalonChannel : IEthalonChannel
    {
        public static string Key = "PACEChannel";

        private readonly PACE1000Model _paseModel;

        public PACEEchalonChannel(PACE1000Model paseModel)
        {
            _paseModel = paseModel;
        }

        #region Implementation IEthalonChannel
        public bool Activate()
        {
            IsActive = true;
            return true;
        }

        public void Stop()
        {
            IsActive = false;
        }

        public double GetEthalonValue(double point, CancellationToken cancel)
        {
            var result = double.NaN;
            var wh = new ManualResetEvent(false);
            _paseModel.UpdatePressure(wh);
            while (!cancel.IsCancellationRequested)
            {
                if(wh.WaitOne(TimeSpan.FromMilliseconds(20)))
                    break;
            }
            if (!cancel.IsCancellationRequested)
                result = _paseModel.Pressure;
            OnStateUpdated();
            return result;
        }
        #endregion

        public PACE1000Model Model{get { return _paseModel; }}

        public double Pressure{get { return _paseModel.Pressure; }}

        public string PressureUnit{get { return _paseModel.PressureUnit.ToString(); }}

        public bool IsActive { get; set; }

        public event EventHandler StateUpdated;

        protected virtual void OnStateUpdated()
        {
            EventHandler handler = StateUpdated;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void AutoUpdate()
        {
            UpdateState();
            
        }

        private void UpdateState()
        {
            
        }
    }
}
