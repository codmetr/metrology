using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
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
                short pInterfaceType =0;
                short pInterfaceNumber = 0;
                string pSessionType = "";

                var allDevices = rm.FindRsrc("?*");
                bool isFinded = false;
                foreach (var device in allDevices)
                {
                    if(device!=address)
                        continue;
                    isFinded = true;
                    break;
                }
                
                if(!isFinded)
                    throw new Exception(string.Format("Device \"{0}\" not found in VISA devices", address));

                rm.FindRsrc(address);
                rm.ParseRsrc(address, ref pInterfaceType, ref pInterfaceNumber, ref pSessionType);
                var visaSession = rm.Open(address, Ivi.Visa.Interop.AccessMode.NO_LOCK, 2000, "");
                this.msg = (visaSession) as IMessage;
                this.ioArbFG.IO = msg;
            }
            catch (ArgumentException e)
            {
                this.ioArbFG.IO = null;
                throw;
            }
            catch (InvalidCastException e)
            {
                this.ioArbFG.IO = null;
                throw;
            }
            catch (COMException e)
            {
                this.ioArbFG.IO = null;
                throw;
            }
            catch (Win32Exception e)
            {
                this.ioArbFG.IO = null;
                throw;
            }
            catch (Exception e)
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
