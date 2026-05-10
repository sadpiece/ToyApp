using System.Windows;

namespace WpfApp4
{
    public partial class Window1 : Window
    {

        private const string ExpectedLogin = "admin";
        private const string ExpectedPassword = "1234";

        public Window1()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click_1(object sender, RoutedEventArgs e)
        {

            if (TxtLogin.Text == ExpectedLogin && TxtPassword.Password == ExpectedPassword)
            {

                this.DialogResult = true;
            }
            else
            {

                MessageBox.Show("Невірний логін або пароль!", "Помилка авторизації", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click_1(object sender, RoutedEventArgs e)
        { 

            this.DialogResult = false;
        }


    }
}
