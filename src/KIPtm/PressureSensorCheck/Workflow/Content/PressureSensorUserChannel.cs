using System;
using System.Threading;
using KipTM.Interfaces;
using KipTM.Model.Channels;

namespace PressureSensorCheck.Workflow
{
    public class PressureSensorUserChannel : IUserChannel
    {
        private readonly IUserVmAsk _vm;
        private IContext _context;

        public PressureSensorUserChannel(IUserVmAsk vm, IContext context)
        {
            _vm = vm;
            _context = context;
        }

        /// <summary>
        /// Тип ожидаемого действия пользователя
        /// </summary>
        public UserQueryType QueryType { get; private set; }

        /// <summary>
        /// Уточняющее сообщение для получения эталонного значения
        /// </summary>
        public string Message { get { return _vm.Note; } set { Invoke(()=>_vm.Note = value); } }

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
            QueryType = queryType;
            Invoke(() =>
            {
                _vm.IsAsk = true;
                _vm.SetAcceptAction(() => ConfigQuery(wh));
            });
        }

        /// <summary>
        /// Показать модальное сообщение пользователю
        /// </summary>
        /// <param name="title">заголовок</param>
        /// <param name="msg">сообщение</param>
        /// <param name="cancel">отменятор</param>
        public void ShowModal(string title, string msg, CancellationToken cancel)
        {
            Invoke(() => _vm.AskModal(title, msg, cancel));
        }

        /// <summary>
        /// Действие в случае подтверждения
        /// </summary>
        /// <param name="wh"></param>
        private void ConfigQuery(EventWaitHandle wh)
        {
            wh.Set();
            _vm.ResetSetAcceptAction();
            _vm.Note = "";
            _vm.IsAsk = false;
        }

        /// <summary>
        /// Инвокатор
        /// </summary>
        /// <param name="act"></param>
        private void Invoke(Action act)
        {
            if (_context == null)
                act();
            else
                _context.Invoke(act);
        }

        public event EventHandler QueryStarted;
    }
}