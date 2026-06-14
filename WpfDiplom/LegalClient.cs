namespace WpfDiplom.Data.Entities
{
    public class LegalClient : Client
    {
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string OrgForm { get; set; } // ООО, АО, ПАО
        public string Ogrn { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string LegalAddress { get; set; }
    }
}