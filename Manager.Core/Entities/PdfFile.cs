namespace Manager.Core.Entities
{
    public class PdfFile : BaseEntity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Project Project { get; set; }

    }
}