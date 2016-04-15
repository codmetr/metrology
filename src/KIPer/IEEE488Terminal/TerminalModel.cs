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
        public bool TryConnect(int boardID, int pad, int sad, int tmo, int eot, int eos)
        {
            var desc = Ag488Wrap.ibdev(boardID, pad, sad, tmo, eot, eos);
            return desc == 0;
        }

        /// <summary>
        /// Get a device descriptor
        /// </summary>
        /// <param name="boardID">Board number to which the device is connected</param>
        /// <returns>Status outcome</returns>
        public bool TryConnect(string boardID)
        {
            var visa = new VisaDriver.Visa(boardID);
            try
            {
                visa.WriteString("*IDN?");
                _log.Add(string.Format("<<*IDN?"));
                string m_strReturn = visa.ReadString();
                _log.Add(string.Format(">>{0}", m_strReturn));

                return true;
            }
            catch (Exception)
            {
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

        public ICommand Connect { get { return new CommandWrapper(() => TryConnect(_boardnum)); } }

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
