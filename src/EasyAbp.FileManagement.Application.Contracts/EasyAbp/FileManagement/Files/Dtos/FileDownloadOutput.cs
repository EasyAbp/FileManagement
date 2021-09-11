using Volo.Abp.ObjectExtending;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class FileDownloadOutput : ExtensibleObject
    {
        public string FileName { get; set; }

        public string MimeType { get; set; }
        
        public byte[] Content { get; set; }
    }
}