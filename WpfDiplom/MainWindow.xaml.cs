using System.Windows;
using System.Windows.Controls;

namespace WpfDiplom
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            PasswordBoxCtrl.Loaded += (s, e) => UpdatePasswordPlaceholder(null, null);
            PasswordBoxCtrl.PasswordChanged += UpdatePasswordPlaceholder;
            PasswordBoxCtrl.GotFocus += UpdatePasswordPlaceholder;
            PasswordBoxCtrl.LostFocus += UpdatePasswordPlaceholder;
        }

        private void UpdatePasswordPlaceholder(object sender, RoutedEventArgs e)
        {
            if (PasswordBoxCtrl == null) return;
            var placeholder = PasswordBoxCtrl.Template?.FindName("Placeholder", PasswordBoxCtrl) as TextBlock;
            if (placeholder != null)
            {
                placeholder.Visibility =
                    string.IsNullOrEmpty(PasswordBoxCtrl.Password) && !PasswordBoxCtrl.IsFocused
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBoxCtrl.Password;

            // --- Тестовый вход ---
            if (login == "admin" && password == "123")
            {
                // Открываем главное окно
                DashboardWindow dashboard = new DashboardWindow();
                dashboard.Show();
                this.Close();
            }
            else
            {
                ErrorMessage.Text = "Неверный логин или пароль";
                ErrorMessage.Visibility = Visibility.Visible;
            }
        }
    }
}