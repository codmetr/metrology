using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ADTSChecks.ViewModel.Checks
{
    public class DoubleBindingRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double res;
            if(!double.TryParse((string)value, NumberStyles.Any, cultureInfo, out res))
                return new ValidationResult(false, "Недопустимые символы.");

            return new ValidationResult(true, null);
        }
    }
}
