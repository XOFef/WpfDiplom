using System.Collections.Generic;

namespace WpfDiplom.Data.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // Оператор, Менеджер, Руководитель, Администратор
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}