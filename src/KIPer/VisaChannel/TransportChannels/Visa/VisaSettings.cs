namespace VisaChannel.TransportChannels.Visa
{
    public class VisaSettings
    {
        private string _address;

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                AddressFull = string.Format("GPIB0::{0}", _address);
            }
        }

        public string AddressFull { get; private set; }

        public override string ToString()
        {
            return string.Format("Visa:{0}", AddressFull);
        }
    }
}
