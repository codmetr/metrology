using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTS;
using KipTM.Model.Channels;
using KipTM.Settings;

namespace KipTM.Model.Checks
{
    public class ADTSCheckParameters
    {
        private readonly CalibChannel _calibChannel;
        private readonly MethodicSettings _settings;
        private readonly IEthalonChannel _ethalonChannel;
        private readonly IUserChannel _userChannel;

        public ADTSCheckParameters(CalibChannel calibChannel, MethodicSettings settings, IEthalonChannel ethalonChannel, IUserChannel userChannel)
        {
            _calibChannel = calibChannel;
            _settings = settings;
            _ethalonChannel = ethalonChannel;
            _userChannel = userChannel;
        }

        public CalibChannel CalibChannel
        {
            get { return _calibChannel; }
        }

        public MethodicSettings Settings
        {
            get { return _settings; }
        }

        public IEthalonChannel EthalonChannel
        {
            get { return _ethalonChannel; }
        }

        public IUserChannel UserChannel
        {
            get { return _userChannel; }
        }
    }
}
