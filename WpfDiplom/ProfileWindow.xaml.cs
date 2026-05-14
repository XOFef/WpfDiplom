using System.Windows;

namespace WpfDiplom
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Простейшая валидация
            if (string.IsNullOrWhiteSpace(OldPassword.Password) ||
                string.IsNullOrWhiteSpace(NewPassword.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword.Password))
            {
                MessageBox.Show("Заполните все поля пароля.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NewPassword.Password != ConfirmPassword.Password)
            {
                MessageBox.Show("Новый пароль и подтверждение не совпадают.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Заглушка сохранения
            MessageBox.Show("Настройки профиля сохранены.", "Успешно",
                            MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}