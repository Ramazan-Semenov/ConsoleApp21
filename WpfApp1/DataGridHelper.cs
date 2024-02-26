using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public class DataGridHelper : DependencyObject
    {
        public static object GetSynchronizeGroupKey(DependencyObject attachedElement)
          => (object)attachedElement.GetValue(SynchronizeGroupKeyProperty);
        public static void SetSynchronizeGroupKey(DependencyObject attachedElement, object value)
          => attachedElement.SetValue(SynchronizeGroupKeyProperty, value);

        public static readonly DependencyProperty SynchronizeGroupKeyProperty = DependencyProperty.RegisterAttached(
          "SynchronizeGroupKey",
          typeof(object),
          typeof(DataGridHelper),
          new PropertyMetadata(default(object), OnSynchronizeGroupKeyChanged));

        public static bool GetIsSynchronizeSelectedRowEnabled(DependencyObject attachedElement)
          => (bool)attachedElement.GetValue(IsSynchronizeSelectedRowEnabledProperty);
        public static void SetIsSynchronizeSelectedRowEnabled(DependencyObject attachedElement, bool value)
          => attachedElement.SetValue(IsSynchronizeSelectedRowEnabledProperty, value);

        public static readonly DependencyProperty IsSynchronizeSelectedRowEnabledProperty = DependencyProperty.RegisterAttached(
          "IsSynchronizeSelectedRowEnabled",
          typeof(bool),
          typeof(DataGridHelper),
          new PropertyMetadata(default(bool), OnIsSynchronizeSelectedRowEnabledChanged));

        private static Dictionary<object, IList<WeakReference<DataGrid>>> DataGridTable { get; } = new Dictionary<object, IList<WeakReference<DataGrid>>>();
        private static void OnIsSynchronizeSelectedRowEnabledChanged(DependencyObject attachingElement, DependencyPropertyChangedEventArgs e)
        {
            if (!(attachingElement is DataGrid dataGrid))
            {
                throw new ArgumentException($"Attaching element must of type {typeof(DataGrid)}.", nameof(attachingElement));
            }

            if ((bool)e.NewValue)
            {
                RegisterDataGridForSelectedItemSynchronization(dataGrid);
            }
            else
            {
                UnregisterDataGridForSelectedItemSynchronization(dataGrid);
            }
        }

        private static void RegisterDataGridForSelectedItemSynchronization(DataGrid dataGrid)
          => WeakEventManager<DataGrid, SelectionChangedEventArgs>.AddHandler(dataGrid, nameof(DataGrid.SelectionChanged), SynchronizeSelectedItem_OnSelectionChanged);

        private static void UnregisterDataGridForSelectedItemSynchronization(DataGrid dataGrid)
          => WeakEventManager<DataGrid, SelectionChangedEventArgs>.RemoveHandler(dataGrid, nameof(DataGrid.SelectionChanged), SynchronizeSelectedItem_OnSelectionChanged);

        private static void OnSynchronizeGroupKeyChanged(DependencyObject attachingElement, DependencyPropertyChangedEventArgs e)
        {
            if (!(attachingElement is DataGrid dataGrid))
            {
                throw new ArgumentException($"Attaching element must of type {typeof(DataGrid)}.", nameof(attachingElement));
            }

            if (e.NewValue == null)
            {
                throw new ArgumentNullException($"{null} is not a valid value for the attached property {nameof(SynchronizeGroupKeyProperty)}.", nameof(e.NewValue));
            }

            if (!DataGridTable.TryGetValue(e.NewValue, out IList<WeakReference<DataGrid>> dataGridGroup))
            {
                dataGridGroup = new List<WeakReference<DataGrid>>();
                DataGridTable.Add(e.NewValue, dataGridGroup);
            }

            dataGridGroup.Add(new WeakReference<DataGrid>(dataGrid));
        }

        private static void SynchronizeSelectedItem_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var synchronizationSourceDataGrid = sender as DataGrid;
            var synchronizationSourceDataGridGroupKey = GetSynchronizeGroupKey(synchronizationSourceDataGrid);
            if (!DataGridTable.TryGetValue(synchronizationSourceDataGridGroupKey, out IList<WeakReference<DataGrid>> dataGridGroup))
            {
                return;
            }

            var selectedIndices = synchronizationSourceDataGrid.SelectedItems
              .Cast<object>()
              .Select(synchronizationSourceDataGrid.Items.IndexOf)
              .ToList();

            foreach (WeakReference<DataGrid> dataGridReference in dataGridGroup)
            {
                if (!dataGridReference.TryGetTarget(out DataGrid dataGrid)
                  || dataGrid == synchronizationSourceDataGrid
                  || dataGrid.Items.Count == 0)
                {
                    continue;
                }

                UnregisterDataGridForSelectedItemSynchronization(dataGrid);
                dataGrid.SelectedItems.Clear();
                foreach (int selectedItemIndex in selectedIndices)
                {
                    var selectedItem = dataGrid.Items[selectedItemIndex];
                    dataGrid.SelectedItems.Add(selectedItem);
                }

                RegisterDataGridForSelectedItemSynchronization(dataGrid);
            }
        }
    }
}
