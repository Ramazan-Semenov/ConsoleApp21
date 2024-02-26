using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DataGridRowItem
    {
        public List<DataGridColumnGroupItem> Columns { get; set; }
    }
    public class DataGridColumnGroupItem
    {
        public string GroupHeader { get; set; }
        public List<Appointment> TableData { get; set; }
    }
    public class Appointment
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
