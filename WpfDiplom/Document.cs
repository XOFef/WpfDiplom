using System;

namespace WpfDiplom.Data.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Паспорт, Договор, Справка
        public byte[] FileData { get; set; } // реальные файлы лучше хранить в файловой системе, здесь для простоты - byte[]
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public int UploadedByUserId { get; set; }

        public virtual Client Client { get; set; }
        public virtual User UploadedBy { get; set; }
    }
}