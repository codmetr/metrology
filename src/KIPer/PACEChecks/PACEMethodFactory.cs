using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using KipTM.Interfaces;
using KipTM.Model.Checks;

namespace PACEChecks
{
    public class PACEMethodFactory : IMethodFactory
    {
        private Dictionary<string, ICheckMethod> _methods;

        public string GetKey()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, ICheckMethod> GetMethods()
        {
            throw new NotImplementedException();
        }

        public Bitmap GetBigImage()
        {
            throw new NotImplementedException();
        }

        public Bitmap GetSmallImage()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }
    }
}
