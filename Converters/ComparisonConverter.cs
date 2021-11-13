using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Assignment5.Converters
{
    //Found this on stack overflow for mapping to the enum for GameType
    public class ComparisonConverter : IValueConverter
    {
        /// <summary>
        /// Convert Value to different type mapping 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }
        /// <summary>
        /// Converts from source back to original value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }
}
