using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using WpfDiplom.Data.Context;

namespace WpfDiplom
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBoxCtrl.Password;

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == login && u.IsActive);
                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    App.CurrentUser = user;
                    App.LogAction("Login", $"Пользователь {user.Login} вошёл в систему", "", "");
                    DashboardWindow dashboard = new DashboardWindow();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    ErrorMessage.Text = "Неверный логин или пароль,\nили учётная запись заблокирована";
                    ErrorMessage.Visibility = Visibility.Visible;
                    ErrorMessage.FontSize = 12;
                }
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            using (var sha256 = SHA256.Create())
            {
                var computedHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
                return computedHash == hash;
            }
        }
    }
}