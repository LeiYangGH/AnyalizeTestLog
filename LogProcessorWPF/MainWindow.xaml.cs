using System.Windows;
using LogProcessorWPF.ViewModel;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
namespace LogProcessorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private MainViewModel mainViewModel;
        #endregion Fields

        public MainWindow()
        {
            InitializeComponent();
            this.mainViewModel = new MainViewModel();
            this.DataContext = this.mainViewModel;
            Messenger.Default.Register<bool?>(this, (p) =>
            {
                this.SortCvsTs(p);
            });
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree.
        /// http://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child.</param>
        /// <returns>The first parent item that matches the submitted type parameter.
        /// If not matching item can be found,
        /// a null parent is being returned.</returns>
        public T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid.
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child.
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 根据选择的列名排序Tests
        /// </summary>
        /// <param name="byStatus">根据Status排序</param>
        private void SortCvsTs(bool? byStatus)
        {
            UC4Log uc = this.uC4Log;
            ItemsControl icp = this.FindChild<ItemsControl>(uc, "ICP");
            foreach (var pvm in icp.Items)
            {
                var container = icp.ItemContainerGenerator.ContainerFromItem(pvm);
                var exp = this.FindChild<Expander>(container, "exp");
                CollectionViewSource cvsT =
                    (CollectionViewSource)(exp.FindResource("cvsT"));
                cvsT.SortDescriptions.Clear();
                if (byStatus ?? false)
                {
                    cvsT.SortDescriptions.Add(new SortDescription("Status", ListSortDirection.Ascending));
                    cvsT.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
                }
                else
                {
                    cvsT.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
                }
            }
        }

        #region Properties

        #endregion Properties
    }

}
