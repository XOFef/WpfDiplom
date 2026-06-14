using System;

namespace WpfDiplom.Data.Entities
{
    public class IndividualClient : Client
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Citizenship { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public virtual PassportData Passport { get; set; }
    }
}