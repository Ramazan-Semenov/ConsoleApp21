using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    public class GroupDefinition : FrameworkContentElement
    {
        public int Column
        {
            get => (int)GetValue(ColumnProperty);
            set => SetValue(ColumnProperty, value);
        }

        public static readonly DependencyProperty ColumnProperty = DependencyProperty.Register(
          "Column",
          typeof(int),
          typeof(GroupDefinition),
          new PropertyMetadata(default));

        public int ColumnSpan
        {
            get => (int)GetValue(ColumnSpanProperty);
            set => SetValue(ColumnSpanProperty, value);
        }

        public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.Register(
          "ColumnSpan",
          typeof(int),
          typeof(GroupDefinition),
          new PropertyMetadata(default));

        public object Header
        {
            get => (object)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
          "Header",
          typeof(object),
          typeof(GroupDefinition),
          new PropertyMetadata(default));
    }
}
