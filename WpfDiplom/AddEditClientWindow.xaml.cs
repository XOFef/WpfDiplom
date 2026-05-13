using System.Windows;
using System.Windows.Controls;

namespace WpfDiplom
{
    public partial class AddEditClientWindow : Window
    {
        public AddEditClientWindow()
        {
            InitializeComponent();
        }

        private void ClientTypeChanged(object sender, RoutedEventArgs e)
        {
            if (PanelIndividual == null || PanelLegal == null ||
        PanelEntrepreneur == null || PanelPassport == null)
                return; // ещё не загружены элементы

            if (RbIndividual.IsChecked == true)
            {
               PanelIndividual.Visibility = Visibility.Visible;
                PanelLegal.Visibility = Visibility.Collapsed;
                PanelEntrepreneur.Visibility = Visibility.Collapsed;
               PanelPassport.Visibility = Visibility.Visible;
            }
            else if (RbLegal.IsChecked == true)
            {
                PanelIndividual.Visibility = Visibility.Collapsed;
                PanelLegal.Visibility = Visibility.Visible;
                PanelEntrepreneur.Visibility = Visibility.Collapsed;
                PanelPassport.Visibility = Visibility.Collapsed;
            }
            else if (RbEntrepreneur.IsChecked == true)
            {
                PanelIndividual.Visibility = Visibility.Collapsed;
                PanelLegal.Visibility = Visibility.Collapsed;
                PanelEntrepreneur.Visibility = Visibility.Visible;
                PanelPassport.Visibility = Visibility.Visible;
            }
        }

        private void CheckDuplicate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Дубликаты не найдены.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Клиент сохранён.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BtnSaveAndDocs_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Клиент сохранён. Переход к загрузке документов.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}