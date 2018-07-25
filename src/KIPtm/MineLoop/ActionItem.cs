using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MainLoop
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ActionItem<T>
    {
        /// <summary>
        /// Real action
        /// </summary>
        private readonly Action<T> _realAction;
        
        /// <summary>
        /// Marker complete
        /// </summary>
        private readonly EventWaitHandle _whComplete;

        /// <summary>
        /// Подпись метода
        /// </summary>
        private readonly string _note;

        /// <summary>
        /// Обертка метода
        /// </summary>
        /// <param name="realAction">сам метод</param>
        /// <param name="whComplete">индикатор завершения</param>
        /// <param name="note">подпись метода</param>
        public ActionItem(Action<T> realAction, EventWaitHandle whComplete, string note)
        {
            _realAction = realAction;
            _whComplete = whComplete;
            _note = note;
        }

        /// <summary>
        /// Обертка метода
        /// </summary>
        /// <param name="realAction">сам метод</param>
        /// <param name="whComplete">индикатор завершения</param>
        public ActionItem(Action<T> realAction, EventWaitHandle whComplete)
            :this(realAction, whComplete, null)
        { }

        /// <summary>
        /// Обертка метода
        /// </summary>
        /// <param name="realAction"></param>
        public ActionItem(Action<T> realAction)
            : this(realAction, null, null)
        { }

        /// <summary>
        /// Real action
        /// </summary>
        public Action<T> RealAction
        {
            get { return _realAction; }
        }

        /// <summary>
        /// Marker complete
        /// </summary>
        public EventWaitHandle WhComplete
        {
            get { return _whComplete; }
        }

        /// <summary>
        /// Подпись метода
        /// </summary>
        public string Note
        {
            get { return _note; }
        }
    }
}
