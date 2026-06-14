using Microsoft.EntityFrameworkCore;
using System;
using WpfDiplom.Data.Entities;

namespace WpfDiplom.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<IndividualClient> IndividualClients { get; set; }
        public DbSet<LegalClient> LegalClients { get; set; }
        public DbSet<EntrepreneurClient> EntrepreneurClients { get; set; }
        public DbSet<PassportData> Passports { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ClientHistory> ClientHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<DictionaryItem> DictionaryItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BankStandard.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .ToTable("Clients")
                .HasDiscriminator<string>("Type")
                .HasValue<IndividualClient>("Individual")
                .HasValue<LegalClient>("Legal")
                .HasValue<EntrepreneurClient>("Entrepreneur");

            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<LegalClient>().HasIndex(l => l.Inn).IsUnique();
            modelBuilder.Entity<EntrepreneurClient>().HasIndex(e => e.Inn).IsUnique();

            modelBuilder.Entity<PassportData>()
                .HasOne(p => p.Client)
                .WithOne()
                .HasForeignKey<PassportData>(p => p.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Client>()
                .HasOne(c => c.Manager)
                .WithMany()
                .HasForeignKey(c => c.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Branch>().HasData(
                new Branch { Id = 1, Name = "Центральное" },
                new Branch { Id = 2, Name = "Северное" },
                new Branch { Id = 3, Name = "Южное" }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Оператор" },
                new Role { Id = 2, Name = "Менеджер" },
                new Role { Id = 3, Name = "Руководитель" },
                new Role { Id = 4, Name = "Администратор" }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, RoleId = 4, Module = "ClientsView", IsAllowed = true },
                new Permission { Id = 2, RoleId = 4, Module = "ClientsEdit", IsAllowed = true },
                new Permission { Id = 3, RoleId = 4, Module = "ClientsDelete", IsAllowed = true },
                new Permission { Id = 4, RoleId = 4, Module = "Reports", IsAllowed = true },
                new Permission { Id = 5, RoleId = 4, Module = "Admin", IsAllowed = true },
                new Permission { Id = 6, RoleId = 2, Module = "ClientsView", IsAllowed = true },
                new Permission { Id = 7, RoleId = 2, Module = "ClientsEdit", IsAllowed = true },
                new Permission { Id = 8, RoleId = 2, Module = "Reports", IsAllowed = true },
                new Permission { Id = 9, RoleId = 1, Module = "ClientsView", IsAllowed = true },
                new Permission { Id = 10, RoleId = 1, Module = "ClientsEdit", IsAllowed = false },
                new Permission { Id = 11, RoleId = 1, Module = "ClientsDelete", IsAllowed = false },
                new Permission { Id = 12, RoleId = 1, Module = "Reports", IsAllowed = false },
                new Permission { Id = 13, RoleId = 1, Module = "Admin", IsAllowed = false },
                new Permission { Id = 14, RoleId = 3, Module = "ClientsView", IsAllowed = true },
                new Permission { Id = 15, RoleId = 3, Module = "ClientsEdit", IsAllowed = true },
                new Permission { Id = 16, RoleId = 3, Module = "ClientsDelete", IsAllowed = true },
                new Permission { Id = 17, RoleId = 3, Module = "Reports", IsAllowed = true },
                new Permission { Id = 18, RoleId = 3, Module = "Admin", IsAllowed = false }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Login = "admin",
                    PasswordHash = ComputeHash("123"),
                    FullName = "Администратор",
                    Position = "Системный администратор",
                    RoleId = 4,
                    BranchId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<DictionaryItem>().HasData(
                new DictionaryItem { Id = 1, DictionaryType = "Branch", Code = "001", Name = "Центральное" },
                new DictionaryItem { Id = 2, DictionaryType = "Branch", Code = "002", Name = "Северное" },
                new DictionaryItem { Id = 3, DictionaryType = "Branch", Code = "003", Name = "Южное" },
                new DictionaryItem { Id = 4, DictionaryType = "DocumentType", Code = "01", Name = "Паспорт РФ" },
                new DictionaryItem { Id = 5, DictionaryType = "DocumentType", Code = "02", Name = "Загранпаспорт" },
                new DictionaryItem { Id = 6, DictionaryType = "ClientStatus", Code = "ACT", Name = "Активен" },
                new DictionaryItem { Id = 7, DictionaryType = "ClientStatus", Code = "BLK", Name = "Заблокирован" },
                new DictionaryItem { Id = 8, DictionaryType = "ClientStatus", Code = "CHK", Name = "На проверке" },
                new DictionaryItem { Id = 9, DictionaryType = "OrgForm", Code = "OOO", Name = "ООО" },
                new DictionaryItem { Id = 10, DictionaryType = "OrgForm", Code = "AO", Name = "АО" },
                new DictionaryItem { Id = 11, DictionaryType = "OrgForm", Code = "PAO", Name = "ПАО" },
                new DictionaryItem { Id = 12, DictionaryType = "Source", Code = "ADV", Name = "Реклама" },
                new DictionaryItem { Id = 13, DictionaryType = "Source", Code = "REF", Name = "Сарафанное радио" },
                new DictionaryItem { Id = 14, DictionaryType = "Source", Code = "PART", Name = "Партнёрская программа" },
                new DictionaryItem { Id = 15, DictionaryType = "Source", Code = "WEB", Name = "Сайт банка" },
                new DictionaryItem { Id = 16, DictionaryType = "Source", Code = "APP", Name = "Мобильное приложение" },
                new DictionaryItem { Id = 17, DictionaryType = "Source", Code = "OFF", Name = "Офис банка" },
                new DictionaryItem { Id = 18, DictionaryType = "Source", Code = "CONS", Name = "Консультация специалиста" },
                new DictionaryItem { Id = 19, DictionaryType = "Source", Code = "SOC", Name = "Социальные сети" }
            );
        }

        private string ComputeHash(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}