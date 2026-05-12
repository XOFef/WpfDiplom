using System.Windows;

namespace WpfDiplom
{
    public partial class ClientListWindow : Window
    {
        public ClientListWindow()
        {
            InitializeComponent();
            BtnViewCard.Click += (s, e) => OpenClientCard();
        }

        private void OpenClientCard()
        {
            // В реальном приложении здесь будет ID выбранного клиента
            ClientCardWindow card = new ClientCardWindow();
            card.Show();        // немодальное окно
        }
    }
}