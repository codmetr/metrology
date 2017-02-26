using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PACESeries.Semantic;
using static PACESeries.PACEParserV2;

namespace PACESeries
{
    /// <summary>
    /// Транслятор протокола PACE
    /// </summary>
    public class PACEParserV2
    {
        #region GetIdentificator "*IDN?"
        /// <summary>
        /// Получить команду "Запрос идентификатора"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetIdentificator()
        {
            return new LexNode<Codes>(Codes.IDN).Get();
        }

        /// <summary>
        /// Разобрать ответ на команду "Запрос идентификатора"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="idn">Идентификатор</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetIdentificator(string message, out string idn)
        {
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
            {
                {"Idn", PeremeterTypes.String}
            });
            idn = (string)answer["Idn"];
            return true;
        }
        #endregion

        #region GetDate ":SYST:DATE?"
        /// <summary>
        /// Получить команду "Запрос текущей даты"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetDate()
        {
            return new LexNode<Codes>(Codes.SYST).Add(Codes.d).Get(); ;
        }

        /// <summary>
        /// Разобрать ответ на команду "Запрос текущей даты"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="year">год</param>
        /// <param name="month">месяц</param>
        /// <param name="day">день</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetDate(string message, out int year, out int month, out int day)
        {
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
            {
                {"year", PeremeterTypes.Integer},
                {"month", PeremeterTypes.Integer},
                {"day", PeremeterTypes.Integer}
            });
            year = (int)answer["year"];
            month = (int)answer["month"];
            day = (int)answer["day"];
            return true;
        }
        #endregion
    }
}
