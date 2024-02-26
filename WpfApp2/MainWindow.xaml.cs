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

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<User> DataGridItems { get; set; } = new ObservableCollection<User>();
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 1; i++)
            {
                //DataGridItems.Add(new User { Name_1=i.ToString() });
            }
            DataContext = this;
        }
    }

    public class User
    {
        public int Name_1 { get; set; }
        public string Name_2 { get; set; }
        public string Name_3 { get; set; }
        public string Name_4 { get; set; }
        public string Name_5 { get; set; }
        public string Name_6 { get; set; }
        public string Name_7 { get; set; }
    }
}
