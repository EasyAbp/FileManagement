namespace EasyAbp.FileManagement.Files.Dtos
{
    public class FileDownloadOutput
    {
        public string FileName { get; set; }

        public string MimeType { get; set; }
        
        public byte[] Content { get; set; }
    }
}