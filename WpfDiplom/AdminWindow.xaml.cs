using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WpfDiplom.Data.Context;
using WpfDiplom.Data.Entities;

namespace WpfDiplom
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
            LoadRolesCombo();
            LoadAuditFilterUsers();
            LoadDictionary();
        }

        // ==================== ПОЛЬЗОВАТЕЛИ ====================
        private void LoadUsers()
        {
            using (var db = new AppDbContext())
            {
                var users = db.Users
                    .Include(u => u.Role)
                    .Include(u => u.Branch)
                    .Select(u => new
                    {
                        u.Id,
                        u.FullName,
                        u.Position,
                        Role = u.Role != null ? u.Role.Name : "",
                        Branch = u.Branch != null ? u.Branch.Name : "",
                        u.IsActive
                    })
                    .ToList();
                UsersGrid.ItemsSource = users;
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditUserWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadUsers();
                App.LogAction("AddUser", $"Добавлен пользователь {dialog.UserLogin}", "", "");
            }
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = UsersGrid.SelectedItem;
            if (selected != null)
            {
                var dialog = new AddEditUserWindow((int)selected.Id);
                if (dialog.ShowDialog() == true)
                {
                    LoadUsers();
                    App.LogAction("EditUser", $"Пользователь ID={selected.Id}", "", "");
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = UsersGrid.SelectedItem;
            if (selected != null)
            {
                if (MessageBox.Show("Сбросить пароль пользователю?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var user = db.Users.Find((int)selected.Id);
                        if (user != null)
                        {
                            user.PasswordHash = ComputeHash("123");
                            db.SaveChanges();
                            App.LogAction("ResetPassword", $"Пользователь {user.Login}", "", "Пароль сброшен на '123'");
                            MessageBox.Show("Пароль сброшен на '123'.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnBlockUser_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = UsersGrid.SelectedItem;
            if (selected != null)
            {
                string action = selected.IsActive ? "заблокировать" : "разблокировать";
                if (MessageBox.Show($"{action.ToUpper()} пользователя?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var user = db.Users.Find((int)selected.Id);
                        if (user != null)
                        {
                            user.IsActive = !user.IsActive;
                            db.SaveChanges();
                            LoadUsers();
                            string msg = user.IsActive ? "разблокирован" : "заблокирован";
                            App.LogAction("BlockUser", $"Пользователь {user.Login}", "", msg);
                            MessageBox.Show($"Пользователь {msg}.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ==================== РОЛИ И ПРАВА ====================
        private void LoadRolesCombo()
        {
            using (var db = new AppDbContext())
            {
                var roles = db.Roles.ToList();
                CmbRoles.ItemsSource = roles;
                CmbRoles.DisplayMemberPath = "Name";
                CmbRoles.SelectedValuePath = "Id";
                if (roles.Any())
                    CmbRoles.SelectedIndex = 0;
                LoadRightsForSelectedRole();
            }
        }

        private void LoadRightsForSelectedRole()
        {
            if (CmbRoles.SelectedItem == null) return;
            int roleId = (int)CmbRoles.SelectedValue;

            using (var db = new AppDbContext())
            {
                var permissions = db.Permissions.Where(p => p.RoleId == roleId).ToDictionary(p => p.Module);
                ChkClientsView.IsChecked = permissions.ContainsKey("ClientsView") && permissions["ClientsView"].IsAllowed;
                ChkClientsEdit.IsChecked = permissions.ContainsKey("ClientsEdit") && permissions["ClientsEdit"].IsAllowed;
                ChkClientsDelete.IsChecked = permissions.ContainsKey("ClientsDelete") && permissions["ClientsDelete"].IsAllowed;
                ChkReports.IsChecked = permissions.ContainsKey("Reports") && permissions["Reports"].IsAllowed;
                ChkAdmin.IsChecked = permissions.ContainsKey("Admin") && permissions["Admin"].IsAllowed;
            }
        }

        private void CmbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRightsForSelectedRole();
        }

        private void BtnSaveRights_Click(object sender, RoutedEventArgs e)
        {
            if (CmbRoles.SelectedItem == null) return;
            int roleId = (int)CmbRoles.SelectedValue;
            string roleName = (CmbRoles.SelectedItem as Role)?.Name ?? "";

            using (var db = new AppDbContext())
            {
                var modules = new[] { "ClientsView", "ClientsEdit", "ClientsDelete", "Reports", "Admin" };
                foreach (var module in modules)
                {
                    var perm = db.Permissions.FirstOrDefault(p => p.RoleId == roleId && p.Module == module);
                    if (perm == null)
                    {
                        perm = new Permission { RoleId = roleId, Module = module };
                        db.Permissions.Add(perm);
                    }
                    bool isAllowed = false;
                    switch (module)
                    {
                        case "ClientsView": isAllowed = ChkClientsView.IsChecked == true; break;
                        case "ClientsEdit": isAllowed = ChkClientsEdit.IsChecked == true; break;
                        case "ClientsDelete": isAllowed = ChkClientsDelete.IsChecked == true; break;
                        case "Reports": isAllowed = ChkReports.IsChecked == true; break;
                        case "Admin": isAllowed = ChkAdmin.IsChecked == true; break;
                    }
                    perm.IsAllowed = isAllowed;
                }
                db.SaveChanges();
                App.LogAction("ChangeRights", $"Роль {roleName}", "", "Права изменены");
                MessageBox.Show("Права сохранены.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ==================== СПРАВОЧНИКИ ====================
        private void LoadDictionary()
        {
            string dictType = (CmbDictionaryType.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(dictType)) return;

            string mappedType = MapDictionaryType(dictType);
            using (var db = new AppDbContext())
            {
                var items = db.DictionaryItems.Where(d => d.DictionaryType == mappedType).ToList();
                DictionaryGrid.ItemsSource = items;
            }
        }

        private void CmbDictionaryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDictionary();
        }

        private void BtnAddDict_Click(object sender, RoutedEventArgs e)
        {
            string dictType = (CmbDictionaryType.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(dictType)) return;

            var dialog = new InputDialog("Добавление записи в справочник \"" + dictType + "\"");
            if (dialog.ShowDialog() == true)
            {
                string mappedType = MapDictionaryType(dictType);
                using (var db = new AppDbContext())
                {
                    var item = new DictionaryItem
                    {
                        DictionaryType = mappedType,
                        Code = dialog.Code,
                        Name = dialog.Name
                    };
                    db.DictionaryItems.Add(item);
                    db.SaveChanges();
                    App.LogAction("AddDictionary", $"Справочник {dictType}, код {dialog.Code}, наименование {dialog.Name}", "", "");
                    LoadDictionary();
                }
            }
        }

        private void BtnEditDict_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = DictionaryGrid.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new InputDialog("Редактирование записи", selected.Code, selected.Name);
            if (dialog.ShowDialog() == true)
            {
                using (var db = new AppDbContext())
                {
                    var item = db.DictionaryItems.Find(selected.Id);
                    if (item != null)
                    {
                        string oldName = item.Name;
                        item.Code = dialog.Code;
                        item.Name = dialog.Name;
                        db.SaveChanges();
                        App.LogAction("EditDictionary", $"Справочник {MapDictionaryType((CmbDictionaryType.SelectedItem as ComboBoxItem)?.Content.ToString())}, ID={selected.Id}", oldName, dialog.Name);
                        LoadDictionary();
                    }
                }
            }
        }

        private void BtnDeleteDict_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = DictionaryGrid.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить запись \"{selected.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var item = db.DictionaryItems.Find(selected.Id);
                    if (item != null)
                    {
                        string dictType = (CmbDictionaryType.SelectedItem as ComboBoxItem)?.Content.ToString();
                        db.DictionaryItems.Remove(item);
                        db.SaveChanges();
                        App.LogAction("DeleteDictionary", $"Справочник {dictType}, ID={selected.Id}", selected.Name, "");
                        LoadDictionary();
                    }
                }
            }
        }

        private string MapDictionaryType(string displayType)
        {
            switch (displayType)
            {
                case "Отделения": return "Branch";
                case "Типы документов": return "DocumentType";
                case "Источники привлечения": return "Source";
                case "Статусы клиентов": return "ClientStatus";
                case "Орг.-правовые формы": return "OrgForm";
                default: return displayType;
            }
        }

        // ==================== АУДИТ ====================
        private void LoadAuditFilterUsers()
        {
            using (var db = new AppDbContext())
            {
                var users = db.Users.Select(u => new { u.Id, u.FullName }).ToList();
                var list = users.Select(u => new { u.Id, u.FullName }).ToList();
                list.Insert(0, new { Id = 0, FullName = "Все" });
                CmbAuditUser.ItemsSource = list;
                CmbAuditUser.SelectedValuePath = "Id";
                CmbAuditUser.DisplayMemberPath = "FullName";
                CmbAuditUser.SelectedIndex = 0;
            }
        }

        private void BtnAuditSearch_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var query = db.AuditLogs.Include(a => a.User).AsQueryable();

                int userId = (int)CmbAuditUser.SelectedValue;
                if (userId != 0)
                    query = query.Where(a => a.UserId == userId);

                if (DtAuditFrom.SelectedDate.HasValue)
                    query = query.Where(a => a.DateTime >= DtAuditFrom.SelectedDate.Value);

                if (DtAuditTo.SelectedDate.HasValue)
                {
                    var endDate = DtAuditTo.SelectedDate.Value.Date.AddDays(1);
                    query = query.Where(a => a.DateTime < endDate);
                }

                var logs = query.OrderByDescending(a => a.DateTime)
                    .Select(a => new
                    {
                        a.DateTime,
                        User = a.User != null ? a.User.FullName : "",
                        a.Action,
                        a.Object,
                        a.OldValue,
                        a.NewValue
                    })
                    .ToList();

                AuditGrid.ItemsSource = logs;
            }
        }

        private void BtnAuditClear_Click(object sender, RoutedEventArgs e)
        {
            CmbAuditUser.SelectedIndex = 0;
            DtAuditFrom.SelectedDate = null;
            DtAuditTo.SelectedDate = null;
            AuditGrid.ItemsSource = null;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string ComputeHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}