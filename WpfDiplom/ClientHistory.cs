using System;

namespace WpfDiplom.Data.Entities
{
    public class ClientHistory
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // "Заметка", "Звонок", "Встреча"
        public string Content { get; set; }
        public int AuthorId { get; set; }

        public virtual Client Client { get; set; }
        public virtual User Author { get; set; }
    }
}