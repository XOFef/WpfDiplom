using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using WpfDiplom.Data.Context;
using WpfDiplom.Data.Entities;

namespace WpfDiplom
{
    public partial class QuickSearchWindow : Window
    {
        public QuickSearchWindow()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs args)
        {
            string query = SearchQuery.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Введите поисковый запрос.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var individualMatches = db.IndividualClients
                    .Include(i => i.Passport)
                    .Where(i => i.LastName.Contains(query) || i.FirstName.Contains(query) || (i.Passport != null && i.Passport.Number.Contains(query)))
                    .Select(i => new
                    {
                        i.Id,
                        Type = "Физ. лицо",
                        FullName = i.LastName + " " + i.FirstName + (string.IsNullOrEmpty(i.MiddleName) ? "" : " " + i.MiddleName),
                        InnPassport = i.Passport != null ? i.Passport.Number : null,
                        i.Status
                    });

                var legalMatches = db.LegalClients
                    .Where(l => l.FullName.Contains(query) || l.Inn.Contains(query))
                    .Select(l => new
                    {
                        l.Id,
                        Type = "Юр. лицо",
                        FullName = l.FullName,
                        InnPassport = l.Inn,
                        l.Status
                    });

                var entrepreneurMatches = db.EntrepreneurClients
                    .Where(ent => ent.LastName.Contains(query) || ent.FirstName.Contains(query) || ent.Inn.Contains(query))
                    .Select(ent => new
                    {
                        ent.Id,
                        Type = "ИП",
                        FullName = ent.LastName + " " + ent.FirstName + (string.IsNullOrEmpty(ent.MiddleName) ? "" : " " + ent.MiddleName),
                        InnPassport = ent.Inn,
                        ent.Status
                    });

                var results = individualMatches
                    .Concat(legalMatches)
                    .Concat(entrepreneurMatches)
                    .Take(50)
                    .ToList();

                SearchResultsGrid.ItemsSource = results;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        private void BtnViewCard_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = SearchResultsGrid.SelectedItem;
            if (selected != null)
            {
                ClientCardWindow card = new ClientCardWindow(selected.Id);
                card.Show();
            }
            else
            {
                MessageBox.Show("Выберите клиента в таблице.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = SearchResultsGrid.SelectedItem;
            if (selected != null)
            {
                AddEditClientWindow edit = new AddEditClientWindow(selected.Id);
                edit.ShowDialog();
                BtnSearch_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Выберите клиента в таблице.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = SearchResultsGrid.SelectedItem;
            if (selected != null)
            {
                if (MessageBox.Show("Удалить выбранного клиента?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var client = db.Clients.Find(selected.Id);
                        if (client != null)
                        {
                            string clientInfo = "";
                            if (client is IndividualClient ind) clientInfo = $"{ind.LastName} {ind.FirstName}";
                            else if (client is LegalClient legal) clientInfo = legal.FullName;
                            else if (client is EntrepreneurClient ent) clientInfo = $"{ent.LastName} {ent.FirstName}";
                            db.Clients.Remove(client);
                            db.SaveChanges();
                            App.LogAction("DeleteClient", $"Клиент ID={client.Id}, {clientInfo}", "", "");
                            MessageBox.Show("Клиент удалён.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                            BtnSearch_Click(sender, e);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента в таблице.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}