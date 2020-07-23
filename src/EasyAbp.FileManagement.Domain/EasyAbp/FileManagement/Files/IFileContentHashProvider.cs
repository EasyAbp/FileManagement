namespace EasyAbp.FileManagement.Files
{
    public interface IFileContentHashProvider
    {
        string GetHashString(byte[] fileContent);
    }
}