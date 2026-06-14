using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfDiplom.Data.Context;
using WpfDiplom.Data.Entities;

namespace WpfDiplom
{
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
                SeedDatabase(context);
            }
        }

        public static void LogAction(string action, string objectInfo, string oldValue = "", string newValue = "")
        {
            using (var db = new AppDbContext())
            {
                var log = new AuditLog
                {
                    DateTime = DateTime.Now,
                    UserId = CurrentUser?.Id ?? 1,
                    Action = action,
                    Object = objectInfo,
                    OldValue = oldValue,
                    NewValue = newValue
                };
                db.AuditLogs.Add(log);
                db.SaveChanges();
            }
        }

        private void SeedDatabase(AppDbContext context)
        {
            if (context.Clients.Any()) return;

            var manager = context.Users.FirstOrDefault(u => u.Login == "admin");
            var centralBranch = context.Branches.First(b => b.Name == "Центральное");
            var northBranch = context.Branches.First(b => b.Name == "Северное");
            var southBranch = context.Branches.First(b => b.Name == "Южное");

            if (manager == null) return;

            // Клиенты
            var client1 = new IndividualClient
            {
                LastName = "Иванов",
                FirstName = "Иван",
                MiddleName = "Иванович",
                BirthDate = new DateTime(1985, 3, 15),
                BirthPlace = "г. Москва",
                Citizenship = "Российская Федерация",
                Gender = "Мужской",
                Phone = "+7 (916) 123-45-67",
                Email = "ivanov@example.com",
                Type = "Individual",
                Status = "Активен",
                RegDate = new DateTime(2020, 1, 10),
                ManagerId = manager.Id,
                BranchId = centralBranch.Id
            };
            var client2 = new IndividualClient
            {
                LastName = "Петрова",
                FirstName = "Анна",
                MiddleName = "Сергеевна",
                BirthDate = new DateTime(1990, 7, 22),
                BirthPlace = "г. Санкт-Петербург",
                Citizenship = "Российская Федерация",
                Gender = "Женский",
                Phone = "+7 (921) 987-65-43",
                Email = "petrova@example.com",
                Type = "Individual",
                Status = "Активен",
                RegDate = new DateTime(2021, 3, 5),
                ManagerId = manager.Id,
                BranchId = centralBranch.Id
            };
            var client3 = new IndividualClient
            {
                LastName = "Смирнов",
                FirstName = "Алексей",
                MiddleName = "Викторович",
                BirthDate = new DateTime(1978, 11, 3),
                BirthPlace = "г. Екатеринбург",
                Citizenship = "Российская Федерация",
                Gender = "Мужской",
                Phone = "+7 (912) 345-67-89",
                Email = "smirnov@example.com",
                Type = "Individual",
                Status = "Активен",
                RegDate = new DateTime(2019, 9, 20),
                ManagerId = manager.Id,
                BranchId = northBranch.Id
            };
            var client4 = new IndividualClient
            {
                LastName = "Козлова",
                FirstName = "Елена",
                MiddleName = "Дмитриевна",
                BirthDate = new DateTime(1995, 12, 25),
                BirthPlace = "г. Новосибирск",
                Citizenship = "Российская Федерация",
                Gender = "Женский",
                Phone = "+7 (913) 111-22-33",
                Email = "kozlov@example.com",
                Type = "Individual",
                Status = "На проверке",
                RegDate = DateTime.Now.AddDays(-5),
                ManagerId = manager.Id,
                BranchId = centralBranch.Id
            };
            var client5 = new LegalClient
            {
                FullName = "Общество с ограниченной ответственностью «Ромашка»",
                ShortName = "ООО «Ромашка»",
                OrgForm = "ООО",
                Ogrn = "1027700123456",
                Inn = "7701234567",
                Kpp = "770101001",
                LegalAddress = "г. Москва, ул. Тверская, д. 1",
                Type = "Legal",
                Status = "Активен",
                RegDate = new DateTime(2018, 4, 10),
                ManagerId = manager.Id,
                BranchId = centralBranch.Id
            };
            var client6 = new LegalClient
            {
                FullName = "Акционерное общество «ТехноИнвест»",
                ShortName = "АО «ТехноИнвест»",
                OrgForm = "АО",
                Ogrn = "1027800987654",
                Inn = "7812345678",
                Kpp = "781201001",
                LegalAddress = "г. Санкт-Петербург, Невский пр., д. 100",
                Type = "Legal",
                Status = "Активен",
                RegDate = new DateTime(2017, 11, 20),
                ManagerId = manager.Id,
                BranchId = northBranch.Id
            };
            var client7 = new LegalClient
            {
                FullName = "Публичное акционерное общество «СтройМонтаж»",
                ShortName = "ПАО «СтройМонтаж»",
                OrgForm = "ПАО",
                Ogrn = "1036600456789",
                Inn = "6601234567",
                Kpp = "660101001",
                LegalAddress = "г. Екатеринбург, ул. Ленина, д. 50",
                Type = "Legal",
                Status = "Заблокирован",
                RegDate = new DateTime(2016, 2, 15),
                ManagerId = manager.Id,
                BranchId = southBranch.Id
            };
            var client8 = new EntrepreneurClient
            {
                LastName = "Кузнецов",
                FirstName = "Андрей",
                MiddleName = "Петрович",
                BirthDate = new DateTime(1982, 4, 12),
                Inn = "500112345678",
                Ogrnip = "304500112345678",
                Phone = "+7 (909) 555-44-33",
                Email = "kuznetsov@example.com",
                Type = "Entrepreneur",
                Status = "Активен",
                RegDate = new DateTime(2022, 6, 1),
                ManagerId = manager.Id,
                BranchId = centralBranch.Id
            };
            var client9 = new EntrepreneurClient
            {
                LastName = "Соколова",
                FirstName = "Мария",
                MiddleName = "Владимировна",
                BirthDate = new DateTime(1989, 8, 20),
                Inn = "780212345678",
                Ogrnip = "307780212345678",
                Phone = "+7 (921) 777-88-99",
                Email = "sokolova@example.com",
                Type = "Entrepreneur",
                Status = "Активен",
                RegDate = new DateTime(2023, 1, 15),
                ManagerId = manager.Id,
                BranchId = northBranch.Id
            };
            var client10 = new EntrepreneurClient
            {
                LastName = "Федоров",
                FirstName = "Игорь",
                MiddleName = "Сергеевич",
                BirthDate = new DateTime(1975, 3, 5),
                Inn = "660312345678",
                Ogrnip = "307660312345678",
                Phone = "+7 (922) 333-44-55",
                Email = "fedorov@example.com",
                Type = "Entrepreneur",
                Status = "На проверке",
                RegDate = DateTime.Now.AddDays(-2),
                ManagerId = manager.Id,
                BranchId = southBranch.Id
            };

            context.Clients.AddRange(client1, client2, client3, client4, client5, client6, client7, client8, client9, client10);
            context.SaveChanges();

            // Паспорта
            var dbClients = context.Clients.ToList();
            var ivanov = dbClients.OfType<IndividualClient>().First(c => c.LastName == "Иванов");
            ivanov.Passport = new PassportData { ClientId = ivanov.Id, DocumentType = "Паспорт РФ", Series = "45 12", Number = "345678", IssueDate = new DateTime(2010, 5, 20), Issuer = "ОВД «Ленинский» г. Москвы", IssuerCode = "123-456" };
            var petrova = dbClients.OfType<IndividualClient>().First(c => c.LastName == "Петрова");
            petrova.Passport = new PassportData { ClientId = petrova.Id, DocumentType = "Паспорт РФ", Series = "40 12", Number = "987654", IssueDate = new DateTime(2012, 8, 15), Issuer = "УФМС России по г. Санкт-Петербургу", IssuerCode = "789-012" };
            var smirnov = dbClients.OfType<IndividualClient>().First(c => c.LastName == "Смирнов");
            smirnov.Passport = new PassportData { ClientId = smirnov.Id, DocumentType = "Паспорт РФ", Series = "65 01", Number = "123987", IssueDate = new DateTime(2008, 2, 10), Issuer = "ОВД «Железнодорожный» г. Екатеринбурга", IssuerCode = "456-789" };
            var kozlova = dbClients.OfType<IndividualClient>().First(c => c.LastName == "Козлова");
            kozlova.Passport = new PassportData { ClientId = kozlova.Id, DocumentType = "Паспорт РФ", Series = "50 20", Number = "456123", IssueDate = new DateTime(2015, 6, 30), Issuer = "УФМС по Новосибирской области", IssuerCode = "321-654" };
            var kuznetsov = dbClients.OfType<EntrepreneurClient>().First(c => c.LastName == "Кузнецов");
            kuznetsov.Passport = new PassportData { ClientId = kuznetsov.Id, DocumentType = "Паспорт РФ", Series = "45 67", Number = "890123", IssueDate = new DateTime(2005, 9, 15), Issuer = "ОВД «Центральный» г. Москвы", IssuerCode = "567-890" };
            var sokolova = dbClients.OfType<EntrepreneurClient>().First(c => c.LastName == "Соколова");
            sokolova.Passport = new PassportData { ClientId = sokolova.Id, DocumentType = "Паспорт РФ", Series = "40 56", Number = "345098", IssueDate = new DateTime(2010, 11, 25), Issuer = "УФМС по г. Санкт-Петербургу", IssuerCode = "901-234" };
            var fedorov = dbClients.OfType<EntrepreneurClient>().First(c => c.LastName == "Федоров");
            fedorov.Passport = new PassportData { ClientId = fedorov.Id, DocumentType = "Паспорт РФ", Series = "65 78", Number = "567890", IssueDate = new DateTime(2000, 7, 10), Issuer = "ОВД «Октябрьский» г. Екатеринбурга", IssuerCode = "234-567" };
            context.SaveChanges();

            // Счета
            var accounts = new List<Account>();
            accounts.Add(new Account { ClientId = ivanov.Id, AccountNumber = "40817810123456789012", AccountType = "Расчетный", Currency = "RUB", Balance = 12500.50m, OpenDate = new DateTime(2020, 1, 15), Status = "Активен" });
            accounts.Add(new Account { ClientId = ivanov.Id, AccountNumber = "42301810123456789013", AccountType = "Сберегательный", Currency = "RUB", Balance = 500000.00m, OpenDate = new DateTime(2021, 6, 10), Status = "Активен" });
            accounts.Add(new Account { ClientId = petrova.Id, AccountNumber = "40817810234567890123", AccountType = "Расчетный", Currency = "RUB", Balance = 23500.75m, OpenDate = new DateTime(2021, 3, 10), Status = "Активен" });
            accounts.Add(new Account { ClientId = smirnov.Id, AccountNumber = "40817810345678901234", AccountType = "Расчетный", Currency = "RUB", Balance = 8750.00m, OpenDate = new DateTime(2019, 9, 25), Status = "Активен" });
            accounts.Add(new Account { ClientId = smirnov.Id, AccountNumber = "42301810345678901235", AccountType = "Кредитный", Currency = "RUB", Balance = -150000.00m, OpenDate = new DateTime(2022, 1, 20), Status = "Активен" });
            accounts.Add(new Account { ClientId = kozlova.Id, AccountNumber = "40817810456789012345", AccountType = "Расчетный", Currency = "RUB", Balance = 0.00m, OpenDate = DateTime.Now.AddDays(-5), Status = "Активен" });
            var romashka = dbClients.OfType<LegalClient>().First(c => c.Inn == "7701234567");
            accounts.Add(new Account { ClientId = romashka.Id, AccountNumber = "40702810123456789012", AccountType = "Расчетный", Currency = "RUB", Balance = 1250000.00m, OpenDate = new DateTime(2018, 4, 15), Status = "Активен" });
            var techno = dbClients.OfType<LegalClient>().First(c => c.Inn == "7812345678");
            accounts.Add(new Account { ClientId = techno.Id, AccountNumber = "40702810234567890123", AccountType = "Расчетный", Currency = "RUB", Balance = 8750000.50m, OpenDate = new DateTime(2017, 11, 25), Status = "Активен" });
            accounts.Add(new Account { ClientId = techno.Id, AccountNumber = "40702810234567890124", AccountType = "Валютный", Currency = "USD", Balance = 50000.00m, OpenDate = new DateTime(2019, 3, 10), Status = "Активен" });
            var stroy = dbClients.OfType<LegalClient>().First(c => c.Inn == "6601234567");
            accounts.Add(new Account { ClientId = stroy.Id, AccountNumber = "40702810345678901234", AccountType = "Расчетный", Currency = "RUB", Balance = 234567.89m, OpenDate = new DateTime(2016, 2, 20), Status = "Закрыт" });
            accounts.Add(new Account { ClientId = kuznetsov.Id, AccountNumber = "40802810123456789012", AccountType = "Расчетный", Currency = "RUB", Balance = 325000.00m, OpenDate = new DateTime(2022, 6, 10), Status = "Активен" });
            accounts.Add(new Account { ClientId = sokolova.Id, AccountNumber = "40802810234567890123", AccountType = "Расчетный", Currency = "RUB", Balance = 150000.50m, OpenDate = new DateTime(2023, 1, 20), Status = "Активен" });
            accounts.Add(new Account { ClientId = fedorov.Id, AccountNumber = "40802810345678901234", AccountType = "Расчетный", Currency = "RUB", Balance = 0.00m, OpenDate = DateTime.Now.AddDays(-2), Status = "Активен" });
            context.Accounts.AddRange(accounts);
            context.SaveChanges();
        }
    }
}