namespace WpfDiplom.Data.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string Module { get; set; }   // "ClientsView", "ClientsEdit", "Reports", "Admin"
        public bool IsAllowed { get; set; }
        public virtual Role Role { get; set; }
    }
}