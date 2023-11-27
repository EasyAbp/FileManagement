namespace EasyAbp.FileManagement;

public static class FileManagementConsts
{
    public static char DirectorySeparator { get; set; } = '/';

    public static int DirectoryMaxSubResourceCount { get; set; } = 99999; // todo

    public static int FileContainerNameMaxLength { get; set; } = 64;

    public static class File
    {
        public static int FileNameMaxLength { get; set; } = 255;

        public static int BlobNameMaxLength { get; set; } = 64;

        public static int MimeTypeMaxLength { get; set; } = 32;

        public static int HashMaxLength { get; set; } = 32;

        public static int FlagMaxLength { get; set; } = 64;
    }
}