using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfApp2
{
    public class DataGridColumnRangeWidthToGroupHeaderWidthConverter : IMultiValueConverter
    {
        private IList<DataGridColumn> DataGridColumns { get; }

        public DataGridColumnRangeWidthToGroupHeaderWidthConverter(IList<DataGridColumn> dataGridColumns)
        {
            this.DataGridColumns = dataGridColumns;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
          => values.Cast<DataGridLength>().Sum(gridLength => gridLength.DisplayValue);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var groupDefinition = (GroupDefinition)parameter;
            double currentGroupedColumnsWidth = this.DataGridColumns
              .Skip(groupDefinition.Column)
              .Take(groupDefinition.ColumnSpan)
              .Select(column => column.Width.DisplayValue)
              .Sum();

            var result = new object[groupDefinition.ColumnSpan];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Binding.DoNothing;
            }
           // Array.Fill(result, Binding.DoNothing);

            DataGridColumn lastGroupColumn = this.DataGridColumns[groupDefinition.Column + groupDefinition.ColumnSpan - 1];
            var newColumnWidth = new DataGridLength(lastGroupColumn.Width.DisplayValue + (double)value - currentGroupedColumnsWidth, DataGridLengthUnitType.Pixel);
            result[result.Length - 1] = newColumnWidth;

            return result;
        }
    }
    public static class ArrayExtensions
    {
        public static void Fill<T>(this T[] originalArray, T with)
        {
            for (int i = 0; i < originalArray.Length; i++)
            {
                originalArray[i] = with;
            }
        }
    }
}
