using Labb_3.Models;
using Labb_3.ViewModels;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Labb_3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        private void MenuView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(
            object sender,
            CancelEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "You may have unsaved changes.\n\nDo you want to exit?",
                    "Exit Application",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}