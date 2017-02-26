using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PACESeries.Semantic
{

    public class LexNode<Te>
    {
        private readonly StringBuilder _command;

        public LexNode()
        {
            _command = new StringBuilder();
        }

        public LexNode(Te code) :this()
        {
            Add(code);
        }

        /// <summary>
        /// Добавить параметр
        /// </summary>
        /// <param name="option">параметр</param>
        /// <returns></returns>
        public LexNode<Te> Add(int option)
        {
            _command.Append(option.ToString());
            return this;
        }

        /// <summary>
        /// Добавить команду
        /// </summary>
        /// <param name="code">команда</param>
        /// <returns></returns>
        public LexNode<Te> Add(Te code)
        {
            var attr = GetAtr<CodeDescriptorAttribue>(code);
            if (attr == null)
                throw new NullReferenceException(string.Format("for {0} not found CodeDescriptorAttribue", code));
            _command.Append(attr.Code);
            return this;
        }

        /// <summary>
        /// Получить команду установку
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string Set(params object[] parameters)
        {
            var isFirst = true;
            foreach (var parameter in parameters)
            {
                if (isFirst)
                    _command.Append(" " + parameter.ToString());
                else
                    _command.Append(" " + parameter.ToString());
                isFirst = false;
            }
            return ToString();
        }

        /// <summary>
        /// Получить команд запрос
        /// </summary>
        /// <returns></returns>
        public string Get()
        {
            _command.Append("?");
            return ToString();
        }

        public override string ToString()
        {
            return _command.ToString();
        }

        /// <summary>
        /// Получить арибуты
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        private T GetAtr<T>(Te code) where T : System.Attribute
        {
            var type = code.GetType();
            var memInfo = type.GetMember(code.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : (T)null;
        }
    }
}
