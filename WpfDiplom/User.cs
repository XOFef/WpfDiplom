using System;
using System.Collections.Generic;
using System.Data;

namespace WpfDiplom.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; } // храним хеш пароля
        public string FullName { get; set; }
        public string Position { get; set; }
        public int RoleId { get; set; }
        public int BranchId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Role Role { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
    }
}