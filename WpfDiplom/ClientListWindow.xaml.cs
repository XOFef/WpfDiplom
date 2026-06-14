using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfDiplom.Data.Context;
using WpfDiplom.Data.Entities;

namespace WpfDiplom
{
    public partial class ClientListWindow : Window
    {
        private int currentPage = 1;
        private int pageSize = 20;
        private int totalPages;

        public ClientListWindow()
        {
            InitializeComponent();
            LoadClients();

            BtnViewCard.Click += (s, e) => OpenClientCard();
            BtnAddClient.Click += BtnAddClient_Click;
            BtnEdit.Click += BtnEdit_Click;
            BtnDelete.Click += BtnDelete_Click;
            BtnSearch.Click += BtnSearch_Click;
            BtnClear.Click += BtnClear_Click;
            BtnPrevPage.Click += BtnPrevPage_Click;
            BtnNextPage.Click += BtnNextPage_Click;
            BtnPage1.Click += (s, e) => GoToPage(1);
            BtnPage2.Click += (s, e) => GoToPage(2);
            BtnPage3.Click += (s, e) => GoToPage(3);
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditClientWindow();
            if (addWindow.ShowDialog() == true)
                LoadClients();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = ClientsGrid.SelectedItem;
            if (selected != null)
            {
                var editWindow = new AddEditClientWindow(selected.Id);
                if (editWindow.ShowDialog() == true)
                    LoadClients();
            }
            else
                MessageBox.Show("Выберите клиента для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = ClientsGrid.SelectedItem;
            if (selected != null)
            {
                if (MessageBox.Show($"Удалить клиента {selected.FullName}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var client = db.Clients.Find((int)selected.Id);
                        if (client != null)
                        {
                            string clientInfo = "";
                            if (client is IndividualClient ind) clientInfo = $"{ind.LastName} {ind.FirstName}";
                            else if (client is LegalClient legal) clientInfo = legal.FullName;
                            else if (client is EntrepreneurClient ent) clientInfo = $"{ent.LastName} {ent.FirstName}";
                            db.Clients.Remove(client);
                            db.SaveChanges();
                            App.LogAction("DeleteClient", $"Клиент ID={client.Id}, {clientInfo}", "", "");
                            LoadClients();
                            MessageBox.Show("Клиент удалён.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            else
                MessageBox.Show("Выберите клиента для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            LoadClients();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            CmbFilterType.SelectedIndex = 0;
            CmbFilterStatus.SelectedIndex = 0;
            TxtSearchName.Text = "";
            TxtSearchInn.Text = "";
            currentPage = 1;
            LoadClients();
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadClients(); }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages) { currentPage++; LoadClients(); }
        }

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= totalPages) { currentPage = page; LoadClients(); }
        }

        private void LoadClients()
        {
            using (var db = new AppDbContext())
            {
                string selectedType = (CmbFilterType.SelectedItem as ComboBoxItem)?.Content.ToString();
                string selectedStatus = (CmbFilterStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
                string nameQuery = TxtSearchName.Text.Trim();
                string innQuery = TxtSearchInn.Text.Trim();

                var individualQuery = db.IndividualClients.Include(i => i.Passport).Include(i => i.Manager).AsQueryable();
                var legalQuery = db.LegalClients.Include(l => l.Manager).AsQueryable();
                var entrepreneurQuery = db.EntrepreneurClients.Include(e => e.Passport).Include(e => e.Manager).AsQueryable();

                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Все")
                {
                    individualQuery = individualQuery.Where(c => c.Status == selectedStatus);
                    legalQuery = legalQuery.Where(c => c.Status == selectedStatus);
                    entrepreneurQuery = entrepreneurQuery.Where(c => c.Status == selectedStatus);
                }
                if (!string.IsNullOrEmpty(nameQuery))
                {
                    individualQuery = individualQuery.Where(i => i.LastName.Contains(nameQuery) || i.FirstName.Contains(nameQuery) || i.MiddleName.Contains(nameQuery));
                    legalQuery = legalQuery.Where(l => l.FullName.Contains(nameQuery));
                    entrepreneurQuery = entrepreneurQuery.Where(e => e.LastName.Contains(nameQuery) || e.FirstName.Contains(nameQuery) || e.MiddleName.Contains(nameQuery));
                }
                if (!string.IsNullOrEmpty(innQuery))
                {
                    individualQuery = individualQuery.Where(i => i.Passport != null && i.Passport.Number.Contains(innQuery));
                    legalQuery = legalQuery.Where(l => l.Inn.Contains(innQuery));
                    entrepreneurQuery = entrepreneurQuery.Where(e => e.Inn.Contains(innQuery));
                }

                var individualProj = individualQuery.Select(i => new { i.Id, Type = "Физ. лицо", FullName = i.LastName + " " + i.FirstName + (string.IsNullOrEmpty(i.MiddleName) ? "" : " " + i.MiddleName), InnPassport = i.Passport != null ? i.Passport.Number : null, i.RegDate, i.Status, Manager = i.Manager != null ? i.Manager.FullName : null });
                var legalProj = legalQuery.Select(l => new { l.Id, Type = "Юр. лицо", FullName = l.FullName, InnPassport = l.Inn, l.RegDate, l.Status, Manager = l.Manager != null ? l.Manager.FullName : null });
                var entrepreneurProj = entrepreneurQuery.Select(e => new { e.Id, Type = "ИП", FullName = e.LastName + " " + e.FirstName + (string.IsNullOrEmpty(e.MiddleName) ? "" : " " + e.MiddleName), InnPassport = e.Inn, e.RegDate, e.Status, Manager = e.Manager != null ? e.Manager.FullName : null });

                var combined = individualProj.Concat(legalProj).Concat(entrepreneurProj);
                if (!string.IsNullOrEmpty(selectedType) && selectedType != "Все")
                {
                    string typeMapping = null;
                    if (selectedType == "Физ. лицо") typeMapping = "Физ. лицо";
                    else if (selectedType == "Юр. лицо") typeMapping = "Юр. лицо";
                    else if (selectedType == "ИП") typeMapping = "ИП";
                    if (typeMapping != null) combined = combined.Where(c => c.Type == typeMapping);
                }

                totalPages = (int)Math.Ceiling(combined.Count() / (double)pageSize);
                if (totalPages == 0) totalPages = 1;
                if (currentPage > totalPages) currentPage = totalPages;
                var clients = combined.OrderBy(c => c.Id).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                ClientsGrid.ItemsSource = clients;
                TxtPageInfo.Text = $"Стр. {currentPage} из {totalPages}";
                UpdatePaginationButtons();
            }
        }

        private void UpdatePaginationButtons()
        {
            BtnPrevPage.IsEnabled = currentPage > 1;
            BtnNextPage.IsEnabled = currentPage < totalPages;
            BtnPage1.Visibility = totalPages >= 1 ? Visibility.Visible : Visibility.Collapsed;
            BtnPage2.Visibility = totalPages >= 2 ? Visibility.Visible : Visibility.Collapsed;
            BtnPage3.Visibility = totalPages >= 3 ? Visibility.Visible : Visibility.Collapsed;
            BtnPage1.Content = "1";
            BtnPage2.Content = "2";
            BtnPage3.Content = "3";
            if (totalPages >= 1) { BtnPage1.Background = currentPage == 1 ? (System.Windows.Media.Brush)FindResource("PrimaryBrush") : System.Windows.Media.Brushes.Transparent; BtnPage1.Foreground = currentPage == 1 ? System.Windows.Media.Brushes.White : (System.Windows.Media.Brush)FindResource("PrimaryBrush"); }
            if (totalPages >= 2) { BtnPage2.Background = currentPage == 2 ? (System.Windows.Media.Brush)FindResource("PrimaryBrush") : System.Windows.Media.Brushes.Transparent; BtnPage2.Foreground = currentPage == 2 ? System.Windows.Media.Brushes.White : (System.Windows.Media.Brush)FindResource("PrimaryBrush"); }
            if (totalPages >= 3) { BtnPage3.Background = currentPage == 3 ? (System.Windows.Media.Brush)FindResource("PrimaryBrush") : System.Windows.Media.Brushes.Transparent; BtnPage3.Foreground = currentPage == 3 ? System.Windows.Media.Brushes.White : (System.Windows.Media.Brush)FindResource("PrimaryBrush"); }
        }

        private void OpenClientCard()
        {
            dynamic selected = ClientsGrid.SelectedItem;
            if (selected != null)
            {
                ClientCardWindow card = new ClientCardWindow(selected.Id);
                card.Show();
            }
            else
                MessageBox.Show("Выберите клиента для просмотра карточки.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}