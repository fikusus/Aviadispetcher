using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Aviadispetcher
{
    /// <summary>
    /// Логика взаимодействия для LogInForm.xaml
    /// </summary>
    public partial class LogInForm : Window
    {
        public LogInForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            AuthCheck();
        }
        private void AuthCheck()
        {
            if (MainWindow.logedUser.LogCheck(logTextBox.Text, passwordTextBox.Text) == 2)
            {
                Application.Current.MainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Введіть правильні дані авторизації. ", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AuthCheck();
            }
        }

        private void LogInForm_Closed(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Show();
        }
    }
}
