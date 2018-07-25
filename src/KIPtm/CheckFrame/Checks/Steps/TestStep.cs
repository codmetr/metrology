using System;
using System.Collections.Generic;
using System.Threading;
using ArchiveData.DTO.Params;
using KipTM.Model.Checks;

namespace CheckFrame.Checks.Steps
{
    public abstract class TestStep: TestStepBase, ITestStep<IDictionary<ParameterDescriptor, ParameterResult>>
    {

        /// <summary>
        /// Получены какие-то результаты шага
        /// </summary>
        public event EventHandler<EventArgStepResult<IDictionary<ParameterDescriptor, ParameterResult>>> ResultUpdated;

        #region Инвокаторы

        protected virtual void OnResultUpdated(EventArgStepResultDict e)
        {
            EventHandler<EventArgStepResult<IDictionary<ParameterDescriptor, ParameterResult>>> handler = ResultUpdated;
            if (handler != null) handler(this, e);
        }
        #endregion
    }
}
