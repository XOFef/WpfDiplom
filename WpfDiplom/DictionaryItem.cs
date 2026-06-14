namespace WpfDiplom.Data.Entities
{
    public class DictionaryItem
    {
        public int Id { get; set; }
        public string DictionaryType { get; set; } // "OrgForm", "DocumentType", "ClientStatus", "Source"
        public string Code { get; set; }
        public string Name { get; set; }
    }
}