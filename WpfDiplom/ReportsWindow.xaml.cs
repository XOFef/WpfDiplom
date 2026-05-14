using System.Windows;

namespace WpfDiplom
{
    public partial class ReportsWindow : Window
    {
        public ReportsWindow()
        {
            InitializeComponent();
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            // Заглушка генерации отчёта
            MessageBox.Show("Отчёт сформирован. (Здесь будет вызов логики формирования)",
                            "Отчёт", MessageBoxButton.OK, MessageBoxImage.Information);
            TxtNoData.Visibility = Visibility.Collapsed;
            // В будущем: заполнить ReportGrid.ItemsSource данными
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Экспорт в Excel выполнен.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Экспорт в PDF выполнен.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}