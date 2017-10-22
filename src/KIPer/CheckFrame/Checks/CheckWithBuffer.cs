using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using CheckFrame.Model.Checks.Steps;
using KipTM.Archive;
using KipTM.EventAggregator;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Checks.Steps;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel.Events;
using NLog;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Базовая реализация проверки с линейным прохождением шагов и сохранением результатов через буфер
    /// </summary>
    public abstract class CheckWithBuffer: CheckBase
    {
        private SimpleDataBuffer _dataBuffer = new SimpleDataBuffer();
        protected CheckWithBuffer(Logger logger):base(logger)
        {}

        /// <summary>
        /// Деcтвие перед запуском проверки
        /// </summary>
        protected override bool PrepareCheck(CancellationToken cancel)
        {
            return base.PrepareCheck(cancel);
        }

        protected override void AttachStep(ITestStep step)
        {
            var stepWithBuffer =  step as ITestStepWithBuffer;
            if(stepWithBuffer != null)
                stepWithBuffer.SetBuffer(_dataBuffer);
            base.AttachStep(step);
        }

        protected override void StepEnd(object sender, EventArgEnd e)
        {
            //if(e.Result)
        }
    }
}
