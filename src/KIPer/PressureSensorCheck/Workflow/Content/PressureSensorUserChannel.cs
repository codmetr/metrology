using System;
using System.Threading;
using KipTM.Model.Channels;

namespace PressureSensorCheck.Workflow
{
    public class PressureSensorUserChannel : IUserChannel
    {
        private readonly PressureSensorRunVm _vm;

        public PressureSensorUserChannel(PressureSensorRunVm vm)
        {
            _vm = vm;
        }

        public UserQueryType QueryType { get; }
        public string Message { get { return _vm.Note; } set { _vm.Note = value; } }
        public bool AcceptValue { get; set; }
        public double RealValue { get; set; }
        public bool AgreeValue { get; set; }
        public void NeedQuery(UserQueryType queryType, EventWaitHandle wh)
        {
            _vm.IsAsk = true;
            _vm.DoAccept = () => ConfigQuery(wh);
        }

        private void ConfigQuery(EventWaitHandle wh)
        {
            wh.Set();
            _vm.DoAccept = () => { };
            Message = "";
            _vm.IsAsk = false;
        }

        public event EventHandler QueryStarted;

        public void Clear()
        {
            _vm.DoAccept = () => { };
            Message = "";
            _vm.IsAsk = false;
        }
    }
}