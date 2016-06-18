using System.Windows;
using LogProcessorW.ViewModel;
namespace LogProcessorW
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

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             
        }

        #region Properties
  
        #endregion Properties
         
    }

}
