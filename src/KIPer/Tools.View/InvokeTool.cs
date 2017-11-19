using System;

namespace Tools.View
{
    public static class InvokeTool
    {
        /// <summary>
        /// Обертка для удобного вызова Invoke
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="action"></param>
        public static void InvokeIfRequired<T>(this T c, Action<T> action)
            where T : System.Windows.Controls.Control
        {
            c.Dispatcher.Invoke(new Action(() => action(c)));
        }

        /// <summary>
        /// Обертка для удобного вызова Invoke
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="action"></param>
        public static void InvokeIfRequiredF<T>(this T c, Action<T> action)
            where T : System.Windows.Forms.Control
        {
            if (c.InvokeRequired)
            {
                c.Invoke(new Action(() => action(c)));
            }
            else
            {
                action(c);
            }
        }
    }
}
