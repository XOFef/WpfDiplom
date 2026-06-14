using System;

namespace WpfDiplom.Data.Entities
{
    public class EntrepreneurClient : Client
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Inn { get; set; }
        public string Ogrnip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public virtual PassportData Passport { get; set; }
    }
}