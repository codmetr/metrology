using System;
using System.Collections.Generic;
using System.Threading;
using ArchiveData.DTO.Params;
using KipTM.Model.Checks;

namespace CheckFrame.Checks.Steps
{
    /// <summary>
    /// Тест с передачей результата через буфер данных
    /// </summary>
    public abstract class TestStepWithBuffer: TestStepBase, ITestStep
    {
        protected IDataBuffer _buffer = null;

        /// <summary>
        /// Задать буфер
        /// </summary>
        /// <param name="buffer"></param>
        public void SetBuffer(IDataBuffer buffer)
        {
            _buffer = buffer;
        }
    }
}
