using System.Windows;

namespace WpfDiplom
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();

            // Открытие списка клиентов
            BtnClients.Click += (s, e) =>
            {
                ClientListWindow clientList = new ClientListWindow();
                clientList.Show();
            };

            // Открытие окна добавления клиента
            BtnAddClient.Click += (s, e) =>
            {
                AddEditClientWindow addWindow = new AddEditClientWindow();
                addWindow.ShowDialog();
            };
        }
    }
}