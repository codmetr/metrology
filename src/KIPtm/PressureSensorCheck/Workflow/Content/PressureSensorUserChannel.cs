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

        /// <summary>
        /// Тип ожидаемого действия пользователя
        /// </summary>
        public UserQueryType QueryType { get; }

        /// <summary>
        /// Уточняющее сообщение для получения эталонного значения
        /// </summary>
        public string Message { get { return _vm.Note; } set { _vm.Note = value; } }

        /// <summary>
        /// Эталонное значение от пользователя
        /// </summary>
        /// <remarks>Так же, при необходимости устанавливается в значение по умолчанию</remarks>
        public double RealValue { get; set; }


        /// <summary>
        /// Эталонное значение от пользователя
        /// </summary>
        /// <remarks>Так же, при необходимости устанавливается в значение по умолчанию</remarks>
        public bool AcceptValue { get; set; }

        /// <summary>
        /// Значение, показывающее, что пользователь подтвердил введенное эталонное значение
        /// </summary>
        public bool AgreeValue { get; set; }

        /// <summary>
        /// Запрос на получение эталонного значение параметра от пользователя
        /// </summary>
        /// <param name="wh">Симофор по которому можно будет понять, что пользователь подтвердил ввод</param>
        public void NeedQuery(UserQueryType queryType, EventWaitHandle wh)
        {
            _vm.IsAsk = true;
            _vm.SetAcceptAction(() => ConfigQuery(wh));
        }

        /// <summary>
        /// Показать модальное сообщение пользователю
        /// </summary>
        /// <param name="title">заголовок</param>
        /// <param name="msg">сообщение</param>
        /// <param name="cancel">отменятор</param>
        public void ShowModal(string title, string msg, CancellationToken cancel)
        {
            _vm.AskModal(title, msg, cancel);
        }

        private void ConfigQuery(EventWaitHandle wh)
        {
            wh.Set();
            _vm.ResetSetAcceptAction();
            Message = "";
            _vm.IsAsk = false;
        }

        public event EventHandler QueryStarted;
    }
}