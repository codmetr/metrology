using System;
using System.Collections.Generic;
using ArchiveData.DTO.Params;
using KipTM.Model.Checks;
using NLog;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Базовая реализация проверки с линейным прохождением шагов
    /// </summary>
    public abstract class Check: CheckBase
    {
        protected Check(Logger logger):base(logger)
        {}

        #region events

        /// <summary>
        /// Получен результат
        /// </summary>
        public event EventHandler<EventArgTestStepResult> ResultUpdated;

        #endregion

        #region Steps events

        protected override void AttachStep(ITestStep step)
        {
            var stepWithRes =  step as ITestStep<IDictionary<ParameterDescriptor, ParameterResult>>;
            if(stepWithRes!=null)
                stepWithRes.ResultUpdated += StepResultUpdated;
            base.AttachStep(step);
        }

        protected void DetachStep(ITestStep step)
        {
            var stepWithRes = step as ITestStep<IDictionary<ParameterDescriptor, ParameterResult>>;
            if (stepWithRes != null)
                stepWithRes.ResultUpdated -= StepResultUpdated;
            base.DetachStep(step);
        }

        protected virtual void StepResultUpdated(object sender, EventArgStepResult<IDictionary<ParameterDescriptor, ParameterResult>> e)
        {
            FillResult(e);
        }
        #endregion

        #region Event invocators

        protected virtual void OnResultUpdated(EventArgTestStepResult e)
        {
            EventHandler<EventArgTestStepResult> handler = ResultUpdated;
            if (handler != null) handler(this, e);
        }
        #endregion

        #region Fill results
        /// <summary>
        /// Заполнение полученных результатов проверки
        /// </summary>
        /// <param name="e"></param>
        private void FillResult(EventArgStepResult<IDictionary<ParameterDescriptor, ParameterResult>> e)
        {
            foreach (var parameterResult in e.Result)
            {
                SwitchParameter(parameterResult.Key, parameterResult.Value);
            }
        }

        /// <summary>
        /// Распределить результат в нужное поле результата
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="result"></param>
        protected abstract void SwitchParameter(ParameterDescriptor descriptor, ParameterResult result);

        #endregion
    }
}
