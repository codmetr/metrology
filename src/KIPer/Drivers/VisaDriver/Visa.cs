using System;
using System.Collections.Generic;
using System.Text;
using Ivi.Visa.Interop;

namespace VisaDriver
{
    public class Visa:IDisposable
    {
        private Ivi.Visa.Interop.ResourceManager rm;
        private Ivi.Visa.Interop.FormattedIO488 ioArbFG;
        private Ivi.Visa.Interop.IMessage msg;

        /// <summary>
        /// Канал Visa
        /// </summary>
        /// <param name="address">Адрес прибора</param>
        public Visa(string address)
        {
            try
            {
                rm = new ResourceManager();
                ioArbFG = new FormattedIO488Class();
                this.msg = (rm.Open(address, Ivi.Visa.Interop.AccessMode.NO_LOCK, 2000, "")) as IMessage;
                this.ioArbFG.IO = msg;
            }
            catch (SystemException ex)
            {
                this.ioArbFG.IO = null;
                throw;
            }
        }

        /// <summary>
        /// Переключение к заданому адресу
        /// </summary>
        /// <param name="address">Адрес прибора</param>
        /// <returns>true - Удалось подключиться</returns>
        public bool SetAddress(string address)
        {
            if (this.ioArbFG.IO!=null)
                this.ioArbFG.IO.Close();
            try
            {
                rm = new ResourceManager();
                ioArbFG = new FormattedIO488Class();
                this.msg = (rm.Open(address, Ivi.Visa.Interop.AccessMode.NO_LOCK, 2000, "")) as IMessage;
                this.ioArbFG.IO = msg;
                return true;
            }
            catch (SystemException ex)
            {
                this.ioArbFG.IO = null;
                throw;
            }
        }

        /// <summary>
        /// Записать в канал
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void WriteString(string message)
        {
            ioArbFG.WriteString(message, true);
        }

        /// <summary>
        /// Прочитать из канала
        /// </summary>
        /// <returns>Ответ</returns>
        public string ReadString()
        {
            return ioArbFG.ReadString();
        }

        /// <summary>
        /// Закрыть подключение
        /// </summary>
        public void Dispose()
        {
            ioArbFG.IO.Close();
        }
    }
}
