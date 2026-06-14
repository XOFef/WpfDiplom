using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using WpfDiplom.Data.Context;
using WpfDiplom.Data.Entities;

namespace WpfDiplom
{
    public partial class AddEditUserWindow : Window
    {
        private int? _editingUserId;
        public string UserLogin { get; private set; }

        public AddEditUserWindow(int? id = null)
        {
            InitializeComponent();
            _editingUserId = id;
            LoadRolesAndBranches();
            if (id.HasValue)
            {
                LoadUserData(id.Value);
                TitleText.Text = "✏ Редактирование пользователя";
            }
            else
            {
                TitleText.Text = "➕ Новый пользователь";
            }
        }

        private void LoadRolesAndBranches()
        {
            using (var db = new AppDbContext())
            {
                var roles = db.Roles.ToList();
                CmbRole.ItemsSource = roles;
                CmbRole.DisplayMemberPath = "Name";
                CmbRole.SelectedValuePath = "Id";
                if (roles.Any()) CmbRole.SelectedIndex = 0;

                var branches = db.Branches.ToList();
                CmbBranch.ItemsSource = branches;
                CmbBranch.DisplayMemberPath = "Name";
                CmbBranch.SelectedValuePath = "Id";
                if (branches.Any()) CmbBranch.SelectedIndex = 0;
            }
        }

        private void LoadUserData(int id)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(id);
                if (user == null) return;
                TxtLogin.Text = user.Login;
                TxtFullName.Text = user.FullName;
                TxtPosition.Text = user.Position;
                CmbRole.SelectedValue = user.RoleId;
                CmbBranch.SelectedValue = user.BranchId;
                ChkIsActive.IsChecked = user.IsActive;
                UserLogin = user.Login;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLogin.Text) || string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                MessageBox.Show("Заполните логин и ФИО.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                User user;
                if (_editingUserId.HasValue)
                    user = db.Users.Find(_editingUserId.Value);
                else
                {
                    user = new User();
                    db.Users.Add(user);
                }

                user.Login = TxtLogin.Text;
                user.FullName = TxtFullName.Text;
                user.Position = TxtPosition.Text;
                user.RoleId = (int)CmbRole.SelectedValue;
                user.BranchId = (int)CmbBranch.SelectedValue;
                user.IsActive = ChkIsActive.IsChecked == true;

                if (!_editingUserId.HasValue)
                    user.PasswordHash = ComputeHash("123");

                db.SaveChanges();
                UserLogin = user.Login;
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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