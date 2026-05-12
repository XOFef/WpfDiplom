using System.Windows;

namespace WpfDiplom
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            BtnClients.Click += (s, e) => OpenClientList();
        }

        private void OpenClientList()
        {
            ClientListWindow clientWindow = new ClientListWindow();
            clientWindow.Show();          // немодальное окно
            // clientWindow.ShowDialog(); // или модальное, зависит от задачи
        }
    }
}