using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PACESeries
{
    public class AnswerException:Exception
    {
        public readonly string Answer;
        public readonly string WaitedAnswer;

        public AnswerException(string answer, string waitedAnswer)
        {
            Answer = answer;
            WaitedAnswer = waitedAnswer;
        }
    }
}
