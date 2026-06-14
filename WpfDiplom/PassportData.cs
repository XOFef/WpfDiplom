using System;

namespace WpfDiplom.Data.Entities
{
    public class PassportData
    {
        public int Id { get; set; }
        public int ClientId { get; set; } // для IndividualClient и EntrepreneurClient
        public string DocumentType { get; set; } // Паспорт РФ, Загранпаспорт
        public string Series { get; set; }
        public string Number { get; set; }
        public DateTime IssueDate { get; set; }
        public string Issuer { get; set; }
        public string IssuerCode { get; set; }

        public virtual Client Client { get; set; }
    }
}