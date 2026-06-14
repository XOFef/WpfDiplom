using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfDiplom.Data.Context;
using WpfDiplom.Data.Entities;

namespace WpfDiplom
{
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e) => SearchClients();

        private void SearchClients()
        {
            using (var db = new AppDbContext())
            {
                string nameQuery = TxtSearchName.Text.Trim();
                string innQuery = TxtSearchInn.Text.Trim();
                string passportQuery = TxtSearchPassport.Text.Trim();
                string phoneQuery = TxtSearchPhone.Text.Trim();
                string emailQuery = TxtSearchEmail.Text.Trim();
                string ogrnQuery = TxtSearchOgrn.Text.Trim();
                string addressQuery = TxtSearchAddress.Text.Trim();
                string cityQuery = TxtSearchCity.Text.Trim();
                string selectedType = (CmbType.SelectedItem as ComboBoxItem)?.Content.ToString();
                string selectedStatus = (CmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
                string selectedBranch = (CmbBranch.SelectedItem as ComboBoxItem)?.Content.ToString();
                DateTime? regFrom = DtRegFrom.SelectedDate;
                DateTime? regTo = DtRegTo.SelectedDate;

                var individualQuery = db.IndividualClients.Include(i => i.Passport).Include(i => i.Manager).Include(i => i.Branch).AsQueryable();
                var legalQuery = db.LegalClients.Include(l => l.Manager).Include(l => l.Branch).AsQueryable();
                var entrepreneurQuery = db.EntrepreneurClients.Include(e => e.Passport).Include(e => e.Manager).Include(e => e.Branch).AsQueryable();

                bool searchIndividual = (selectedType == "Все" || selectedType == "Физ. лицо");
                bool searchLegal = (selectedType == "Все" || selectedType == "Юр. лицо");
                bool searchEntrepreneur = (selectedType == "Все" || selectedType == "ИП");

                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Все")
                {
                    if (searchIndividual) individualQuery = individualQuery.Where(c => c.Status == selectedStatus);
                    if (searchLegal) legalQuery = legalQuery.Where(c => c.Status == selectedStatus);
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(c => c.Status == selectedStatus);
                }
                if (!string.IsNullOrEmpty(selectedBranch) && selectedBranch != "Все")
                {
                    if (searchIndividual) individualQuery = individualQuery.Where(c => c.Branch.Name == selectedBranch);
                    if (searchLegal) legalQuery = legalQuery.Where(c => c.Branch.Name == selectedBranch);
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(c => c.Branch.Name == selectedBranch);
                }
                if (regFrom.HasValue)
                {
                    if (searchIndividual) individualQuery = individualQuery.Where(c => c.RegDate >= regFrom.Value);
                    if (searchLegal) legalQuery = legalQuery.Where(c => c.RegDate >= regFrom.Value);
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(c => c.RegDate >= regFrom.Value);
                }
                if (regTo.HasValue)
                {
                    var endDate = regTo.Value.Date.AddDays(1);
                    if (searchIndividual) individualQuery = individualQuery.Where(c => c.RegDate < endDate);
                    if (searchLegal) legalQuery = legalQuery.Where(c => c.RegDate < endDate);
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(c => c.RegDate < endDate);
                }
                if (!string.IsNullOrEmpty(nameQuery))
                {
                    if (searchIndividual) individualQuery = individualQuery.Where(i => i.LastName.Contains(nameQuery) || i.FirstName.Contains(nameQuery) || i.MiddleName.Contains(nameQuery));
                    if (searchLegal) legalQuery = legalQuery.Where(l => l.FullName.Contains(nameQuery));
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(e => e.LastName.Contains(nameQuery) || e.FirstName.Contains(nameQuery) || e.MiddleName.Contains(nameQuery));
                }
                if (!string.IsNullOrEmpty(innQuery))
                {
                    if (searchLegal) legalQuery = legalQuery.Where(l => l.Inn.Contains(innQuery));
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(e => e.Inn.Contains(innQuery));
                }
                if (!string.IsNullOrEmpty(passportQuery))
                {
                    if (searchIndividual) individualQuery = individualQuery.Where(i => i.Passport != null && i.Passport.Number.Contains(passportQuery));
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(e => e.Passport != null && e.Passport.Number.Contains(passportQuery));
                }
                if (!string.IsNullOrEmpty(phoneQuery))
                {
                    if (searchIndividual) individualQuery = individualQuery.Where(i => i.Phone.Contains(phoneQuery));
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(e => e.Phone.Contains(phoneQuery));
                }
                if (!string.IsNullOrEmpty(emailQuery))
                {
                    if (searchIndividual) individualQuery = individualQuery.Where(i => i.Email.Contains(emailQuery));
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(e => e.Email.Contains(emailQuery));
                }
                if (!string.IsNullOrEmpty(ogrnQuery))
                {
                    if (searchLegal) legalQuery = legalQuery.Where(l => l.Ogrn.Contains(ogrnQuery));
                    if (searchEntrepreneur) entrepreneurQuery = entrepreneurQuery.Where(e => e.Ogrnip.Contains(ogrnQuery));
                }
                if (!string.IsNullOrEmpty(addressQuery) && searchLegal) legalQuery = legalQuery.Where(l => l.LegalAddress.Contains(addressQuery));
                if (!string.IsNullOrEmpty(cityQuery) && searchLegal) legalQuery = legalQuery.Where(l => l.LegalAddress.Contains(cityQuery));

                var individualProj = individualQuery.Select(i => new { i.Id, Type = "Физ. лицо", FullName = i.LastName + " " + i.FirstName + (string.IsNullOrEmpty(i.MiddleName) ? "" : " " + i.MiddleName), InnPassport = i.Passport != null ? i.Passport.Number : null, i.RegDate, i.Status, Manager = i.Manager != null ? i.Manager.FullName : null });
                var legalProj = legalQuery.Select(l => new { l.Id, Type = "Юр. лицо", FullName = l.FullName, InnPassport = l.Inn, l.RegDate, l.Status, Manager = l.Manager != null ? l.Manager.FullName : null });
                var entrepreneurProj = entrepreneurQuery.Select(e => new { e.Id, Type = "ИП", FullName = e.LastName + " " + e.FirstName + (string.IsNullOrEmpty(e.MiddleName) ? "" : " " + e.MiddleName), InnPassport = e.Inn, e.RegDate, e.Status, Manager = e.Manager != null ? e.Manager.FullName : null });

                var combined = individualProj.Concat(legalProj).Concat(entrepreneurProj).OrderBy(c => c.Id).ToList();
                SearchResultsGrid.ItemsSource = combined;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtSearchName.Text = ""; TxtSearchInn.Text = ""; TxtSearchPassport.Text = ""; TxtSearchPhone.Text = ""; TxtSearchEmail.Text = ""; TxtSearchOgrn.Text = ""; TxtSearchAddress.Text = ""; TxtSearchCity.Text = "";
            CmbType.SelectedIndex = 0; CmbStatus.SelectedIndex = 0; CmbBranch.SelectedIndex = 0; DtRegFrom.SelectedDate = null; DtRegTo.SelectedDate = null;
            SearchResultsGrid.ItemsSource = null;
        }

        private void BtnViewCard_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = SearchResultsGrid.SelectedItem;
            if (selected != null) new ClientCardWindow(selected.Id).Show();
            else MessageBox.Show("Выберите клиента в таблице.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = SearchResultsGrid.SelectedItem;
            if (selected != null)
            {
                var edit = new AddEditClientWindow(selected.Id);
                edit.ShowDialog();
                SearchClients();
            }
            else MessageBox.Show("Выберите клиента в таблице.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = SearchResultsGrid.SelectedItem;
            if (selected != null && MessageBox.Show("Удалить выбранного клиента?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                        SearchClients();
                    }
                }
            }
            else if (selected == null) MessageBox.Show("Выберите клиента в таблице.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}