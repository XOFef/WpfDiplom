using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfDiplom.Data.Context;
using WpfDiplom.Data.Entities;

namespace WpfDiplom
{
    public partial class AddEditClientWindow : Window
    {
        private int? editingClientId = null;

        public AddEditClientWindow(int? clientId = null)
        {
            InitializeComponent();
            if (clientId.HasValue)
            {
                editingClientId = clientId;
                LoadClientData(clientId.Value);
                Title = "Редактирование клиента";
            }
        }

        private void LoadClientData(int clientId)
        {
            using (var db = new AppDbContext())
            {
                var client = db.Clients
                    .Include(c => (c as IndividualClient).Passport)
                    .Include(c => (c as EntrepreneurClient).Passport)
                    .FirstOrDefault(c => c.Id == clientId);
                if (client == null) return;

                if (client is IndividualClient ind)
                {
                    RbIndividual.IsChecked = true;
                    LastName.Text = ind.LastName;
                    FirstName.Text = ind.FirstName;
                    MiddleName.Text = ind.MiddleName;
                    BirthDate.SelectedDate = ind.BirthDate;
                    BirthPlace.Text = ind.BirthPlace;
                    Citizenship.SelectedItem = ind.Citizenship == "Российская Федерация" ? Citizenship.Items[0] : Citizenship.Items[1];
                    Gender.SelectedItem = ind.Gender == "Мужской" ? Gender.Items[0] : Gender.Items[1];
                    Phone.Text = ind.Phone;
                    Email.Text = ind.Email;

                    if (ind.Passport != null)
                    {
                        DocType.SelectedItem = ind.Passport.DocumentType == "Паспорт РФ" ? DocType.Items[0] : DocType.Items[1];
                        DocSeries.Text = ind.Passport.Series;
                        DocNumber.Text = ind.Passport.Number;
                        DocIssueDate.SelectedDate = ind.Passport.IssueDate;
                        DocIssuer.Text = ind.Passport.Issuer;
                        DocCode.Text = ind.Passport.IssuerCode;
                    }
                }
                else if (client is LegalClient legal)
                {
                    RbLegal.IsChecked = true;
                    FullNameLegal.Text = legal.FullName;
                    ShortName.Text = legal.ShortName;
                    Ogrn.Text = legal.Ogrn;
                    InnLegal.Text = legal.Inn;
                    Kpp.Text = legal.Kpp;
                    RegDateLegal.SelectedDate = legal.RegDate;
                    LegalAddress.Text = legal.LegalAddress;

                    if (legal.OrgForm == "ООО")
                        OrgForm.SelectedItem = OrgForm.Items[0];
                    else if (legal.OrgForm == "АО")
                        OrgForm.SelectedItem = OrgForm.Items[1];
                    else if (legal.OrgForm == "ПАО")
                        OrgForm.SelectedItem = OrgForm.Items[2];
                    else
                        OrgForm.SelectedItem = OrgForm.Items[0];
                }
                else if (client is EntrepreneurClient ent)
                {
                    RbEntrepreneur.IsChecked = true;
                    LastNameIP.Text = ent.LastName;
                    FirstNameIP.Text = ent.FirstName;
                    MiddleNameIP.Text = ent.MiddleName;
                    BirthDateIP.SelectedDate = ent.BirthDate;
                    InnIP.Text = ent.Inn;
                    Ogrnip.Text = ent.Ogrnip;
                    PhoneIP.Text = ent.Phone;
                    EmailIP.Text = ent.Email;

                    if (ent.Passport != null)
                    {
                        DocType.SelectedItem = ent.Passport.DocumentType == "Паспорт РФ" ? DocType.Items[0] : DocType.Items[1];
                        DocSeries.Text = ent.Passport.Series;
                        DocNumber.Text = ent.Passport.Number;
                        DocIssueDate.SelectedDate = ent.Passport.IssueDate;
                        DocIssuer.Text = ent.Passport.Issuer;
                        DocCode.Text = ent.Passport.IssuerCode;
                    }
                }
                ClientTypeChanged(null, null);
            }
        }

        private void ClientTypeChanged(object sender, RoutedEventArgs e)
        {
            if (PanelIndividual == null || PanelLegal == null || PanelEntrepreneur == null || PanelPassport == null)
                return;

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
            string inn = null;
            if (RbLegal.IsChecked == true)
                inn = InnLegal.Text.Trim();
            else if (RbEntrepreneur.IsChecked == true)
                inn = InnIP.Text.Trim();

            if (string.IsNullOrEmpty(inn))
            {
                MessageBox.Show("Введите ИНН.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                bool exists = false;
                if (RbLegal.IsChecked == true)
                    exists = db.LegalClients.Any(l => l.Inn == inn && (editingClientId == null || l.Id != editingClientId));
                else if (RbEntrepreneur.IsChecked == true)
                    exists = db.EntrepreneurClients.Any(ec => ec.Inn == inn && (editingClientId == null || ec.Id != editingClientId));

                if (exists)
                    MessageBox.Show("Клиент с таким ИНН уже существует.", "Дубликат", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    MessageBox.Show("Дубликатов не найдено.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSaveAndDocs_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;
            int savedId = SaveClientToDatabase();
            if (savedId > 0)
            {
                MessageBox.Show("Клиент сохранён. Теперь можно загрузить документы.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private int SaveClientToDatabase()
        {
            using (var db = new AppDbContext())
            {
                Client client = null;
                string oldClientInfo = "";

                if (editingClientId.HasValue)
                {
                    client = db.Clients
                        .Include(c => (c as IndividualClient).Passport)
                        .Include(c => (c as EntrepreneurClient).Passport)
                        .FirstOrDefault(c => c.Id == editingClientId.Value);

                    if (client != null)
                    {
                        if (client is IndividualClient oldInd)
                            oldClientInfo = $"ФИО: {oldInd.LastName} {oldInd.FirstName} {oldInd.MiddleName}, Телефон: {oldInd.Phone}, Email: {oldInd.Email}";
                        else if (client is LegalClient oldLegal)
                            oldClientInfo = $"Наименование: {oldLegal.FullName}, ИНН: {oldLegal.Inn}";
                        else if (client is EntrepreneurClient oldEnt)
                            oldClientInfo = $"ФИО: {oldEnt.LastName} {oldEnt.FirstName} {oldEnt.MiddleName}, ИНН: {oldEnt.Inn}, Телефон: {oldEnt.Phone}";
                    }
                }

                if (RbIndividual.IsChecked == true)
                {
                    IndividualClient ind = client as IndividualClient ?? new IndividualClient();
                    ind.LastName = LastName.Text;
                    ind.FirstName = FirstName.Text;
                    ind.MiddleName = MiddleName.Text;
                    ind.BirthDate = BirthDate.SelectedDate.Value;
                    ind.BirthPlace = BirthPlace.Text;
                    ind.Citizenship = (Citizenship.SelectedItem as ComboBoxItem)?.Content.ToString();
                    ind.Gender = (Gender.SelectedItem as ComboBoxItem)?.Content.ToString();
                    ind.Phone = Phone.Text;
                    ind.Email = Email.Text;
                    ind.Type = "Individual";
                    ind.Status = "Активен";
                    ind.RegDate = editingClientId.HasValue ? ind.RegDate : DateTime.Now;
                    ind.ManagerId = App.CurrentUser.Id;
                    ind.BranchId = App.CurrentUser.BranchId;

                    if (client == null) db.Clients.Add(ind);
                    db.SaveChanges();

                    var passport = ind.Passport;
                    if (passport == null)
                    {
                        passport = new PassportData();
                        ind.Passport = passport;
                    }
                    passport.ClientId = ind.Id;
                    passport.DocumentType = (DocType.SelectedItem as ComboBoxItem)?.Content.ToString();
                    passport.Series = DocSeries.Text;
                    passport.Number = DocNumber.Text;
                    passport.IssueDate = DocIssueDate.SelectedDate.Value;
                    passport.Issuer = DocIssuer.Text;
                    passport.IssuerCode = DocCode.Text;

                    if (passport.Id == 0)
                        db.Passports.Add(passport);
                    else
                        db.Entry(passport).State = EntityState.Modified;

                    db.SaveChanges();

                    string newInfo = $"ФИО: {ind.LastName} {ind.FirstName} {ind.MiddleName}, Телефон: {ind.Phone}, Email: {ind.Email}";
                    if (editingClientId.HasValue)
                        App.LogAction("EditClient", $"Клиент ID={ind.Id}", oldClientInfo, newInfo);
                    else
                        App.LogAction("CreateClient", $"Клиент ID={ind.Id}, {newInfo}", "", "");

                    return ind.Id;
                }
                else if (RbLegal.IsChecked == true)
                {
                    LegalClient legal = client as LegalClient ?? new LegalClient();
                    legal.FullName = FullNameLegal.Text;
                    legal.ShortName = ShortName.Text;
                    legal.OrgForm = (OrgForm.SelectedItem as ComboBoxItem)?.Content.ToString();
                    legal.Ogrn = Ogrn.Text;
                    legal.Inn = InnLegal.Text;
                    legal.Kpp = Kpp.Text;
                    legal.RegDate = RegDateLegal.SelectedDate ?? DateTime.Now;
                    legal.LegalAddress = LegalAddress.Text;
                    legal.Type = "Legal";
                    legal.Status = "Активен";
                    legal.ManagerId = App.CurrentUser.Id;
                    legal.BranchId = App.CurrentUser.BranchId;

                    if (client == null) db.Clients.Add(legal);
                    db.SaveChanges();

                    string newInfo = $"Наименование: {legal.FullName}, ИНН: {legal.Inn}";
                    if (editingClientId.HasValue)
                        App.LogAction("EditClient", $"Клиент ID={legal.Id}", oldClientInfo, newInfo);
                    else
                        App.LogAction("CreateClient", $"Клиент ID={legal.Id}, {newInfo}", "", "");

                    return legal.Id;
                }
                else if (RbEntrepreneur.IsChecked == true)
                {
                    EntrepreneurClient ent = client as EntrepreneurClient ?? new EntrepreneurClient();
                    ent.LastName = LastNameIP.Text;
                    ent.FirstName = FirstNameIP.Text;
                    ent.MiddleName = MiddleNameIP.Text;
                    ent.BirthDate = BirthDateIP.SelectedDate.Value;
                    ent.Inn = InnIP.Text;
                    ent.Ogrnip = Ogrnip.Text;
                    ent.Phone = PhoneIP.Text;
                    ent.Email = EmailIP.Text;
                    ent.Type = "Entrepreneur";
                    ent.Status = "Активен";
                    ent.RegDate = editingClientId.HasValue ? ent.RegDate : DateTime.Now;
                    ent.ManagerId = App.CurrentUser.Id;
                    ent.BranchId = App.CurrentUser.BranchId;

                    if (client == null) db.Clients.Add(ent);
                    db.SaveChanges();

                    var passport = ent.Passport;
                    if (passport == null)
                    {
                        passport = new PassportData();
                        ent.Passport = passport;
                    }
                    passport.ClientId = ent.Id;
                    passport.DocumentType = (DocType.SelectedItem as ComboBoxItem)?.Content.ToString();
                    passport.Series = DocSeries.Text;
                    passport.Number = DocNumber.Text;
                    passport.IssueDate = DocIssueDate.SelectedDate.Value;
                    passport.Issuer = DocIssuer.Text;
                    passport.IssuerCode = DocCode.Text;

                    if (passport.Id == 0)
                        db.Passports.Add(passport);
                    else
                        db.Entry(passport).State = EntityState.Modified;

                    db.SaveChanges();

                    string newInfo = $"ФИО: {ent.LastName} {ent.FirstName} {ent.MiddleName}, ИНН: {ent.Inn}, Телефон: {ent.Phone}, Email: {ent.Email}";
                    if (editingClientId.HasValue)
                        App.LogAction("EditClient", $"Клиент ID={ent.Id}", oldClientInfo, newInfo);
                    else
                        App.LogAction("CreateClient", $"Клиент ID={ent.Id}, {newInfo}", "", "");

                    return ent.Id;
                }
                return 0;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;
            int id = SaveClientToDatabase();
            if (id > 0)
            {
                MessageBox.Show("Клиент сохранён.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
        }

        private bool ValidateFields()
        {
            if (RbIndividual.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(LastName.Text) || string.IsNullOrWhiteSpace(FirstName.Text) ||
                    BirthDate.SelectedDate == null || string.IsNullOrWhiteSpace(Phone.Text))
                {
                    MessageBox.Show("Заполните обязательные поля: Фамилия, Имя, Дата рождения, Телефон.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(DocSeries.Text) || string.IsNullOrWhiteSpace(DocNumber.Text) ||
                    DocIssueDate.SelectedDate == null || string.IsNullOrWhiteSpace(DocIssuer.Text))
                {
                    MessageBox.Show("Заполните паспортные данные: Серия, Номер, Дата выдачи, Кем выдан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            else if (RbLegal.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(FullNameLegal.Text) || string.IsNullOrWhiteSpace(Ogrn.Text) ||
                    string.IsNullOrWhiteSpace(InnLegal.Text) || string.IsNullOrWhiteSpace(LegalAddress.Text))
                {
                    MessageBox.Show("Заполните обязательные поля: Полное наименование, ОГРН, ИНН, Юридический адрес.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            else if (RbEntrepreneur.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(LastNameIP.Text) || string.IsNullOrWhiteSpace(FirstNameIP.Text) ||
                    BirthDateIP.SelectedDate == null || string.IsNullOrWhiteSpace(InnIP.Text) ||
                    string.IsNullOrWhiteSpace(Ogrnip.Text) || string.IsNullOrWhiteSpace(PhoneIP.Text))
                {
                    MessageBox.Show("Заполните обязательные поля: Фамилия, Имя, Дата рождения, ИНН, ОГРНИП, Телефон.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(DocSeries.Text) || string.IsNullOrWhiteSpace(DocNumber.Text) ||
                    DocIssueDate.SelectedDate == null || string.IsNullOrWhiteSpace(DocIssuer.Text))
                {
                    MessageBox.Show("Заполните паспортные данные: Серия, Номер, Дата выдачи, Кем выдан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Выберите тип клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}