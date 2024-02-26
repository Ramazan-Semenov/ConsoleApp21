using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GroupColumn
{

    public class GroupingDataGridHeader : ContentControl
    {
        public GroupDefinition GroupDefinition { get; }

        public GroupingDataGridHeader() : this(new GroupDefinition())
        {
        }

        public GroupingDataGridHeader(GroupDefinition groupDefinition)
        {
            this.GroupDefinition = groupDefinition;
            this.Content = this.GroupDefinition?.Header ?? string.Empty;
        }

        static GroupingDataGridHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupingDataGridHeader), new FrameworkPropertyMetadata(typeof(GroupingDataGridHeader)));
        }
    }
    public class GroupDefinitionCollection : Collection<GroupDefinition>
    { }
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
