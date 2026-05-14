using System.Windows;

namespace WpfDiplom
{
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
            // Здесь можно загрузить тестовые данные при открытии
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Заглушка поиска
            MessageBox.Show("Поиск выполнен. (Здесь будет реальная логика поиска)", "Поиск",
                            MessageBoxButton.OK, MessageBoxImage.Information);
            // В будущем: собрать параметры из полей и заполнить SearchResultsGrid.ItemsSource
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtSearchName.Text = "";
            TxtSearchInn.Text = "";
            TxtSearchPassport.Text = "";
            TxtSearchPhone.Text = "";
            TxtSearchEmail.Text = "";
            TxtSearchOgrn.Text = "";
            TxtSearchAddress.Text = "";
            TxtSearchCity.Text = "";
            CmbType.SelectedIndex = 0;
            CmbStatus.SelectedIndex = 0;
            CmbBranch.SelectedIndex = 0;
            DtRegFrom.SelectedDate = null;
            DtRegTo.SelectedDate = null;
        }

        private void BtnViewCard_Click(object sender, RoutedEventArgs e)
        {
            if (SearchResultsGrid.SelectedItem != null)
            {
                // Открыть карточку клиента (заглушка)
                ClientCardWindow card = new ClientCardWindow();
                card.Show();
            }
            else
            {
                MessageBox.Show("Выберите клиента в таблице.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (SearchResultsGrid.SelectedItem != null)
            {
                AddEditClientWindow edit = new AddEditClientWindow();
                edit.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите клиента в таблице.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SearchResultsGrid.SelectedItem != null)
            {
                if (MessageBox.Show("Удалить выбранного клиента?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Клиент удалён.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента в таблице.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}