using System.Windows;
using LogProcessorWPF.ViewModel;
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

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             
        }

        #region Properties
  
        #endregion Properties
         
    }

}
