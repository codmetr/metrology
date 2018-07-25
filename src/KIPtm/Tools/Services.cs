using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public static class Extentions
    {
        /// <summary>
        /// Монада "Maybe", позволяет запускать функции, которые могут вернуть null подряд
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="o"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> func)
            where TInput : class
            where TResult : class
        {
            if (o == null) return null;
            return func(o);
        }

        public static void With<TInput>(this TInput o, Action<TInput> func)
          where TInput : class
        {
            if (o == null) return;
            func(o);
        }
    }
}
