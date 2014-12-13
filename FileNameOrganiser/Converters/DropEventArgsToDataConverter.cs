using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FileNameOrganiser.Converters
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DropEventArgsToDataConverter : IEventArgsConverter
    {

        public object Convert(object value, object parameter)
        {
            var args = (DragEventArgs)value;
            return args.Data;
        }
    }
}