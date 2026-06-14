using System;
using System.Linq;
using System.Windows;
using WpfDiplom.Data.Context;
using WpfDiplom.Data.Entities;

namespace WpfDiplom
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            LoadDashboardData();

            BtnClients.Click += (s, e) => { new ClientListWindow().Show(); };
            BtnSearch.Click += (s, e) => { new SearchWindow().Show(); };
            BtnReports.Click += (s, e) => { new ReportsWindow().Show(); };
            BtnAdmin.Click += (s, e) => { new AdminWindow().Show(); };
            BtnProfile.Click += (s, e) => { new ProfileWindow().ShowDialog(); };
            BtnAddClient.Click += (s, e) => { new AddEditClientWindow().ShowDialog(); LoadDashboardData(); };
            BtnQuickSearch.Click += (s, e) => { new QuickSearchWindow().Show(); };
        }

        private void LoadDashboardData()
        {
            using (var db = new AppDbContext())
            {
                var today = DateTime.Today;
                TxtNewToday.Text = db.Clients.Count(c => c.RegDate.Date == today).ToString();
                TxtNewTodayInfo.Text = db.Clients.Count(c => c.RegDate.Date == today) == 0 ? "Нет данных" : $"+{db.Clients.Count(c => c.RegDate.Date == today)} сегодня";
                TxtOnCheck.Text = db.Clients.Count(c => c.Status == "На проверке").ToString();
                TxtOnCheckInfo.Text = db.Clients.Count(c => c.Status == "На проверке") == 0 ? "Нет заявок" : $"Требуют проверки: {db.Clients.Count(c => c.Status == "На проверке")}";
                TxtExpiredDocs.Text = db.Passports.Count(p => p.IssueDate < DateTime.Now.AddYears(-10)).ToString();
                TxtExpiredDocsInfo.Text = db.Passports.Count(p => p.IssueDate < DateTime.Now.AddYears(-10)) == 0 ? "Нет" : $"У {db.Passports.Count(p => p.IssueDate < DateTime.Now.AddYears(-10))} клиентов";
                TxtTotalClients.Text = db.Clients.Count().ToString();
                TxtActiveClients.Text = $"Активных: {db.Clients.Count(c => c.Status == "Активен")}";
            }
        }
    }
}