using System.Windows.Data;
using GalaSoft.MvvmLight;

namespace FileNameOrganiser.Converters
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ItemCountToOpacityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? count = value as int?;
            if (count.HasValue && count.Value > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}