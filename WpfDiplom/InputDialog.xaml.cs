using System.Windows;

namespace WpfDiplom
{
    public partial class InputDialog : Window
    {
        public string Code { get; private set; }
        public string Name { get; private set; }

        public InputDialog(string title, string codeDefault = "", string nameDefault = "")
        {
            InitializeComponent();
            Title = title;
            TxtCode.Text = codeDefault;
            TxtName.Text = nameDefault;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Code = TxtCode.Text.Trim();
            Name = TxtName.Text.Trim();
            if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Код и наименование обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}