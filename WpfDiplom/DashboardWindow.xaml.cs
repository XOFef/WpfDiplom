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

            BtnSearch.Click += (s, e) =>
            {
                SearchWindow search = new SearchWindow();
                search.Show();        // немодально, как и список клиентов
            };

            BtnReports.Click += (s, e) =>
            {
                ReportsWindow reports = new ReportsWindow();
                reports.Show();   // немодально, как и другие дочерние окна
            };

            BtnAdmin.Click += (s, e) =>
            {
                AdminWindow admin = new AdminWindow();
                admin.Show();   // немодально
            };

            BtnProfile.Click += (s, e) =>
            {
                ProfileWindow profile = new ProfileWindow();
                profile.ShowDialog();   // модально, чтобы не отвлекаться
            };

            BtnQuickSearch.Click += (s, e) =>
            {
                QuickSearchWindow quickSearch = new QuickSearchWindow();
                quickSearch.Show();   // немодально
            };
        }
    }
}