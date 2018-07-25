using System.Threading;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ADTSData;
using CheckFrame.Checks;
using CheckFrame.Model.Checks.Steps;
using KipTM.EventAggregator;
using NLog;

namespace ADTSChecks.Checks
{
    /// <summary>
    /// Базовая реализация проверки ADTS
    /// </summary>
    public abstract class CheckBaseADTS : CheckBase
    {
        public const string KeySettingsPS = KeysDic.KeySettingsPS;
        public const string KeySettingsPT = KeysDic.KeySettingsPT;
        public const string KeySettingsPSPT = KeysDic.KeySettingsPSPT;

        public const string KeyPoints = "Points";
        public const string KeyRate = "Rate";
        public const string KeyUnit = "Unit";
        //public const string KeyChannel = "Channel";

        protected string MethodName = "ADTS";

        /// <summary>
        /// Параметры проведения проверки
        /// </summary>
        protected ADTSParameters _parameters;

        protected ADTSModel _adts;
        
        protected CheckBaseADTS(Logger logger):base(logger)
        {
            Title = MethodName;
        }
        
        protected override bool PrepareCheck(CancellationToken cancel)
        {
            if (!base.PrepareCheck(cancel))
                return false;
            _adts.Start(ChConfig.ChannelType);
            return true;
        }

        /// <summary>
        /// Задать АДТС
        /// </summary>
        /// <param name="adts"></param>
        public void SetADTS(ADTSModel adts)
        {
            _adts = adts;
        }

        /// <summary>
        /// Получить модель АДТС
        /// </summary>
        /// <returns></returns>
        public ADTSModel GetADTS()
        {
            return _adts;
        }

        /// <summary>
        /// Установить текущую точку как очередную ожидаемую
        /// </summary>
        public void SetCurrentValueAsPoint()
        {
            IStoppedOnPoint pointstep;
            lock (_currenTestStepLocker)
            {
                pointstep = _currenTestStep as IStoppedOnPoint;
            }
            if (pointstep == null)
                return;
            pointstep.SetCurrentValueAsPoint();
        }

    }
}