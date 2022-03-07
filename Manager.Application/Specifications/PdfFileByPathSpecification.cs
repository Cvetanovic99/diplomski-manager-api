using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class PdfFileByPathSpecification : BaseSpecification<PdfFile>
    {
        public PdfFileByPathSpecification(string filePath)
            :base(pdf => pdf.Path == filePath)
        {
            
        }
    }
}