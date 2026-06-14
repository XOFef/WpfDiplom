using System;

namespace WpfDiplom.Data.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; } // Расчетный, Сберегательный, Кредитный
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
        public string Status { get; set; } // Активен, Закрыт

        public virtual Client Client { get; set; }
    }
}