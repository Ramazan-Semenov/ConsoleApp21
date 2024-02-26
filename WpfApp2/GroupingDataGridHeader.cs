using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
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
}
