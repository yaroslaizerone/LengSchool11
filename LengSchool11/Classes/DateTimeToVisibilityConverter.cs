using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LengSchool11.Classes
{
    public class DateTimeToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static DateTimeToVisibilityConverter _converter;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Проверяем, равно ли значение DateService минимальной дате
                if (dateTime == DateTime.MinValue)
                {
                    // Если да, то скрываем дату
                    return string.Empty;
                }
            }

            // Если значение DateService не минимальное, то отображаем содержимое TextBlock
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new DateTimeToVisibilityConverter());
        }
    }
}
