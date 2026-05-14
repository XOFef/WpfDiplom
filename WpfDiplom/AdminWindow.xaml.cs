using System.Windows;

namespace WpfDiplom
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Открыть форму добавления пользователя.", "Пользователи", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem != null)
                MessageBox.Show("Редактирование выбранного пользователя.", "Пользователи", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Выберите пользователя.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem != null)
                MessageBox.Show("Пароль сброшен.", "Пользователи", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Выберите пользователя.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnBlockUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem != null)
            {
                if (MessageBox.Show("Заблокировать пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    MessageBox.Show("Пользователь заблокирован.", "Пользователи", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Выберите пользователя.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnSaveRights_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Права для роли сохранены.", "Роли и права", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAddDict_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление записи в справочник.", "Справочники", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEditDict_Click(object sender, RoutedEventArgs e)
        {
            if (DictionaryGrid.SelectedItem != null)
                MessageBox.Show("Редактирование записи.", "Справочники", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Выберите запись.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnDeleteDict_Click(object sender, RoutedEventArgs e)
        {
            if (DictionaryGrid.SelectedItem != null)
            {
                if (MessageBox.Show("Удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    MessageBox.Show("Запись удалена.", "Справочники", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Выберите запись.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnAuditSearch_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Поиск по журналу выполнен.", "Аудит", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAuditClear_Click(object sender, RoutedEventArgs e)
        {
            CmbAuditUser.SelectedIndex = 0;
            DtAuditFrom.SelectedDate = null;
            DtAuditTo.SelectedDate = null;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}