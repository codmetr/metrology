using System.Threading;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ADTSData;
using CheckFrame.Checks;
using NLog;

namespace ADTSChecks.Checks
{
    /// <summary>
    /// Базовая реализация проверки ADTS
    /// </summary>
    public abstract class CheckBase : Check
    {
        public const string KeySettingsPS = KeysDic.KeySettingsPS;
        public const string KeySettingsPT = KeysDic.KeySettingsPT;
        public const string KeySettingsPSPT = KeysDic.KeySettingsPSPT;

        protected string MethodName = "ADTS";

        /// <summary>
        /// Параметры проведения проверки
        /// </summary>
        protected ADTSParameters _parameters;

        protected ADTSModel _adts;


        protected CheckBase(Logger logger):base(logger)
        {
            Title = MethodName;
        }


        protected override void OnStartAction(CancellationToken cancel)
        {
            _adts.Start(ChannelType);
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

    }
}