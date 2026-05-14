using System.Windows;

namespace WpfDiplom
{
    public partial class QuickSearchWindow : Window
    {
        public QuickSearchWindow()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchQuery.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Введите поисковый запрос.", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Заглушка поиска – здесь будет реальная логика
            MessageBox.Show($"Поиск по запросу: «{query}» выполнен. (Заглушка)", "Поиск",
                            MessageBoxButton.OK, MessageBoxImage.Information);
            // В будущем заполнить SearchResultsGrid.ItemsSource
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
                MessageBox.Show("Выберите клиента в таблице.", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Выберите клиента в таблице.", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SearchResultsGrid.SelectedItem != null)
            {
                if (MessageBox.Show("Удалить выбранного клиента?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Клиент удалён.", "Удаление",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента в таблице.", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}