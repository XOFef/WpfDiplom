using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using WpfDiplom.Data.Context;
using WpfDiplom.Data.Entities;

namespace WpfDiplom
{
    public partial class ClientCardWindow : Window
    {
        private int _clientId;
        private Client _client;

        public ClientCardWindow()
        {
            InitializeComponent();
        }

        public ClientCardWindow(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            LoadClientData();
            BtnClose.Click += (s, e) => this.Close();
            BtnEdit.Click += BtnEdit_Click;
            BtnDelete.Click += BtnDelete_Click;
            BtnAddNote.Click += BtnAddNote_Click;
            BtnOpenAccount.Click += (s, e) => MessageBox.Show("Функция открытия счёта в разработке.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadClientData()
        {
            using (var db = new AppDbContext())
            {
                _client = db.Clients
                    .Include(c => (c as IndividualClient).Passport)
                    .Include(c => (c as EntrepreneurClient).Passport)
                    .Include(c => c.Manager)
                    .Include(c => c.Branch)
                    .FirstOrDefault(c => c.Id == _clientId);
                if (_client == null)
                {
                    MessageBox.Show("Клиент не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                string fullName = GetFullName(_client);
                TxtFullNameHeader.Text = fullName;
                TxtIdAndType.Text = $"ID: {_client.Id} | {GetTypeName(_client.Type)} | {_client.Status}";

                if (_client is IndividualClient ind) FillIndividualCard(ind);
                else if (_client is LegalClient legal) FillLegalCard(legal);
                else if (_client is EntrepreneurClient ent) FillEntrepreneurCard(ent);

                TxtManager.Text = _client.Manager?.FullName ?? "—";
                TxtRegDate.Text = _client.RegDate.ToString("dd.MM.yyyy");
                TxtClientStatus.Text = _client.Status;
                if (_client.Status == "Активен") TxtClientStatus.Foreground = (System.Windows.Media.Brush)FindResource("PrimaryBrush");
                else if (_client.Status == "На проверке") TxtClientStatus.Foreground = (System.Windows.Media.Brush)FindResource("AccentBrush");
                else if (_client.Status == "Заблокирован") TxtClientStatus.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);

                LoadAccounts(db);
                LoadHistory(db);
            }
        }

        private string GetFullName(Client client)
        {
            if (client is IndividualClient ind) return $"{ind.LastName} {ind.FirstName} {ind.MiddleName}".Trim();
            if (client is LegalClient legal) return legal.FullName;
            if (client is EntrepreneurClient ent) return $"{ent.LastName} {ent.FirstName} {ent.MiddleName}".Trim();
            return "Клиент";
        }

        private string GetTypeName(string type)
        {
            switch (type) { case "Individual": return "Физ. лицо"; case "Legal": return "Юр. лицо"; case "Entrepreneur": return "ИП"; default: return type; }
        }

        private void FillIndividualCard(IndividualClient ind)
        {
            TxtLastName.Text = ind.LastName ?? "—";
            TxtFirstName.Text = ind.FirstName ?? "—";
            TxtMiddleName.Text = ind.MiddleName ?? "—";
            TxtBirthDate.Text = ind.BirthDate.ToString("dd.MM.yyyy");
            TxtBirthPlace.Text = ind.BirthPlace ?? "—";
            TxtCitizenship.Text = ind.Citizenship ?? "—";
            TxtGender.Text = ind.Gender ?? "—";
            TxtPhone.Text = ind.Phone ?? "—";
            TxtEmail.Text = ind.Email ?? "—";
            PanelPassport.Visibility = Visibility.Visible;
            if (ind.Passport != null)
            {
                TxtDocType.Text = ind.Passport.DocumentType ?? "—";
                TxtDocSeries.Text = ind.Passport.Series ?? "—";
                TxtDocNumber.Text = ind.Passport.Number ?? "—";
                TxtDocIssueDate.Text = ind.Passport.IssueDate.ToString("dd.MM.yyyy");
                TxtDocIssuer.Text = ind.Passport.Issuer ?? "—";
                TxtDocCode.Text = ind.Passport.IssuerCode ?? "—";
            }
        }

        private void FillLegalCard(LegalClient legal)
        {
            TxtLastName.Text = legal.FullName ?? "—";
            TxtFirstName.Text = legal.ShortName ?? "—";
            TxtMiddleName.Text = $"ИНН: {legal.Inn}";
            TxtBirthDate.Text = "—";
            TxtBirthPlace.Text = "—";
            TxtCitizenship.Text = "—";
            TxtGender.Text = "—";
            TxtPhone.Text = "—";
            TxtEmail.Text = "—";
            PanelPassport.Visibility = Visibility.Collapsed;
        }

        private void FillEntrepreneurCard(EntrepreneurClient ent)
        {
            TxtLastName.Text = ent.LastName ?? "—";
            TxtFirstName.Text = ent.FirstName ?? "—";
            TxtMiddleName.Text = ent.MiddleName ?? "—";
            TxtBirthDate.Text = ent.BirthDate.ToString("dd.MM.yyyy");
            TxtBirthPlace.Text = "—";
            TxtCitizenship.Text = "Российская Федерация";
            TxtGender.Text = "—";
            TxtPhone.Text = ent.Phone ?? "—";
            TxtEmail.Text = ent.Email ?? "—";
            PanelPassport.Visibility = Visibility.Visible;
            if (ent.Passport != null)
            {
                TxtDocType.Text = ent.Passport.DocumentType ?? "—";
                TxtDocSeries.Text = ent.Passport.Series ?? "—";
                TxtDocNumber.Text = ent.Passport.Number ?? "—";
                TxtDocIssueDate.Text = ent.Passport.IssueDate.ToString("dd.MM.yyyy");
                TxtDocIssuer.Text = ent.Passport.Issuer ?? "—";
                TxtDocCode.Text = ent.Passport.IssuerCode ?? "—";
            }
        }

        private void LoadAccounts(AppDbContext db)
        {
            var accounts = db.Accounts.Where(a => a.ClientId == _clientId).ToList();
            AccountsGrid.ItemsSource = accounts;
        }

        private void LoadHistory(AppDbContext db)
        {
            var history = db.ClientHistories.Where(h => h.ClientId == _clientId).OrderByDescending(h => h.Date)
                .Select(h => new { h.Date, h.Type, h.Content, AuthorName = h.Author.FullName }).ToList();
            HistoryGrid.ItemsSource = history;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new AddEditClientWindow(_clientId);
            if (editWindow.ShowDialog() == true) LoadClientData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить клиента?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var client = db.Clients.Find(_clientId);
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
                        this.Close();
                    }
                }
            }
        }

        private void BtnAddNote_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNewNote.Text))
            {
                MessageBox.Show("Введите текст заметки.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var db = new AppDbContext())
            {
                var note = new ClientHistory { ClientId = _clientId, Date = DateTime.Now, Type = "Заметка", Content = TxtNewNote.Text, AuthorId = App.CurrentUser?.Id ?? 1 };
                db.ClientHistories.Add(note);
                db.SaveChanges();
                App.LogAction("AddNote", $"Клиент ID={_clientId}", "", note.Content);
            }
            TxtNewNote.Text = "";
            using (var db = new AppDbContext()) LoadHistory(db);
            MessageBox.Show("Заметка добавлена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}