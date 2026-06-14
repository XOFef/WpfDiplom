using System;

namespace WpfDiplom.Data.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } // "Login", "CreateClient", "EditClient", "DeleteClient"
        public string Object { get; set; } // "Client 123", "User 456"
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public virtual User User { get; set; }
    }
}