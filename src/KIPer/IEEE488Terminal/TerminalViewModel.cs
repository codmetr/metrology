using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Agilent.TMFramework.Connectivity;
namespace IEEE488Terminal
{
    public class TerminalViewModel:INotifyPropertyChanged
    {
        private string _boardnum;
        private ObservableCollection<string> _log = new ObservableCollection<string>();
        private VisaDriver.Visa _visa;

        /// <summary>
        /// Get a device descriptor
        /// </summary>
        /// <param name="boardID">Board number to which the device is connected</param>
        /// <param name="pad">Primary address of the device</param>
        /// <param name="sad">Secondary address of the device (0 if none)</param>
        /// <param name="tmo">Timeout for the device (Txxx constants: see ibtmo reference in help)</param>
        /// <param name="eot">1 to enable end-of-transmission EOI, 0 to disable EOI</param>
        /// <param name="eos">0 to disable end-of-string termination, nonzero to enable (see ibeos)</param>
        /// <returns>Status outcome</returns>
        public bool _tryConnect(int boardID, int pad, int sad, int tmo, int eot, int eos)
        {
            var desc = Ag488Wrap.ibdev(boardID, pad, sad, tmo, eot, eos);
            return desc == 0;
        }

        /// <summary>
        /// Get a device descriptor
        /// </summary>
        /// <param name="boardID">Board number to which the device is connected</param>
        /// <returns>Status outcome</returns>
        public bool _tryConnect(string boardID)
        {
            _visa = new VisaDriver.Visa(boardID);
            try
            {
                _visa.WriteString("*IDN?");
                _log.Add(string.Format("<<*IDN?"));
                string m_strReturn = _visa.ReadString();
                _log.Add(string.Format(">>{0}", m_strReturn));

                return true;
            }
            catch (Exception ex)
            {
                _log.Add(ex.ToString());
                return false;
            }
        }

        public bool _sendReceive(string command)
        {
            try
            {
                _visa.WriteString(command);
                _log.Add(string.Format("<<{0}", command));
                string m_strReturn = _visa.ReadString();
                _log.Add(string.Format(">>{0}", m_strReturn));

                return true;
            }
            catch (Exception ex)
            {
                _log.Add(ex.ToString());
                return false;
            }
        }

        public bool _send(string command)
        {
            try
            {
                _visa.WriteString(command);
                _log.Add(string.Format("<<{0}", command));

                return true;
            }
            catch (Exception ex)
            {
                _log.Add(ex.ToString());
                return false;
            }
        }

        #region Boardnum

        public string Boardnum
        {
            get { return _boardnum; }
            set
            {
                if (value == _boardnum)
                    return;
                _boardnum = value;
                OnPropertyChanged("Boardnum");
            }
        }

        #endregion

        #region Command

        private string _command;

        public string Command
        {
            get { return _command; }
            set
            {
                if (value == _command)
                    return;
                _command = value;
                OnPropertyChanged("Command");
            }
        }

        #endregion

        public ICommand Connect { get { return new CommandWrapper(() => _tryConnect(_boardnum)); } }

        public ICommand Send { get { return new CommandWrapper(() => _send(_command)); } }

        public ICommand SendReceive { get { return new CommandWrapper(() => _sendReceive(_command)); } }

        public ObservableCollection<string> Log
        {
            get { return _log; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
