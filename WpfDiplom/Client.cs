using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Xml.Linq;

namespace WpfDiplom.Data.Entities
{
    public abstract class Client
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Individual", "Legal", "Entrepreneur"
        public string Status { get; set; } // Активен, Заблокирован, На проверке
        public DateTime RegDate { get; set; }
        public int? ManagerId { get; set; } // ID пользователя-менеджера
        public int BranchId { get; set; }

        public virtual User Manager { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<ClientHistory> History { get; set; }
    }
}