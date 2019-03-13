using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData;
using ArchiveData.DTO;
using KipTM.EventAggregator;
using KipTM.Model.Checks;
using KipTM.ViewModel.Events;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    class PressureSensorResultPresenter: ISubscriber<EventArgSave>
    {
        private readonly TestResultID _checkResId;
        private readonly IDataAccessor _accessor;
        private PressureSensorResult _result;
        private readonly PressureSensorConfig _conf;
        private readonly IEventAggregator _agregator;
        private readonly PressureSensorResultVM _resultVm;

        public PressureSensorResultPresenter(TestResultID checkResId, IDataAccessor accessor, PressureSensorResult result, PressureSensorConfig conf, IEventAggregator agregator, PressureSensorResultVM resultVm)
        {
            _checkResId = checkResId;
            _accessor = accessor;
            _result = result;
            _conf = conf;
            _agregator = agregator;
            _resultVm = resultVm;
            _resultVm.OnSaveCalled += OnSave;
            _agregator.Subscribe(this);
        }

        #region ISubscriber<EventArgSave>

        /// <inheritdoc cref="ISubscriber&lt;EventArgSave&gt;"/>
        public void OnEvent(EventArgSave message)
        {
            OnSave();
        }

        #endregion


        public void SetResult(PressureSensorResult result)
        {
            _result = result;
            _resultVm.SetPoints(result.Points);

        }

        /// <summary>
        /// Фактическое выполнение сохранение
        /// </summary>
        private void OnSave()
        {
            _resultVm.SetIsSaveEnable(false);
            try
            {
                _agregator?.Post(new HelpMessageEventArg("Сохранение.."));
                if (_checkResId.Id == null)
                {
                    _checkResId.CreateTime = DateTime.Now;
                    _checkResId.Timestamp = DateTime.Now;
                    _accessor.Add(_checkResId, _result, _conf);
                }
                else
                {
                    _checkResId.Timestamp = DateTime.Now;
                    _accessor.Update(_checkResId);
                    _accessor.Save(_checkResId, _result);
                    _accessor.SaveConfig(_checkResId, _conf);
                }
                _agregator?.Post(new HelpMessageEventArg("Сохранено"));
            }
            finally
            {
                _resultVm.SetIsSaveEnable(true);
            }
        }


    }
}
