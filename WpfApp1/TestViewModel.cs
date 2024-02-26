using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class TestViewModel
    {
        public List<DataGridRowItem> Rows { get; }
        public List<DataGridRowItem> GroupingRow { get; }

        public TestViewModel()
        {
            this.GroupingRow = new List<DataGridRowItem>
    {
      // The single row for the grouping top level DataGrid
      new DataGridRowItem()
      {
        Columns = new List<DataGridColumnGroupItem>()
        {
          // First column group
          new DataGridColumnGroupItem()
          {
            GroupHeader = "Group 1",
            TableData = new List<Appointment>
            {
              new Appointment() { Start = DateTime.Now.AddDays(1), End = DateTime.Now.AddDays(2) },
              new Appointment() { Start = DateTime.Now.AddDays(5), End = DateTime.Now.AddDays(6) }
            }
          },

          // Second column group
          new DataGridColumnGroupItem()
          {
            GroupHeader = "Group 2",
            TableData = new List<Appointment>
            {
              new Appointment() { Start = DateTime.Now.AddDays(3), End = DateTime.Now.AddDays(4) },
              new Appointment() { Start = DateTime.Now.AddDays(7), End = DateTime.Now.AddDays(8) }
            }
          }
        }
      }
    };
        }
    }
}
