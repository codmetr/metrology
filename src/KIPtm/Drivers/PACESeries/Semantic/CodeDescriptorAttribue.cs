using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PACESeries.Semantic
{
    public class CodeDescriptorAttribue : Attribute
    {

        public string Code { get; private set; }

        public string Note { get; private set; }

        public CodeDescriptorAttribue(string code, string note, string predicate = ":")
        {
            Code = predicate + code;
            Note = note;
        }
    }
}
