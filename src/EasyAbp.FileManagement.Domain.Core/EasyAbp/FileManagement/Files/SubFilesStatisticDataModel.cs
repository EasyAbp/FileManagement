namespace EasyAbp.FileManagement.Files;

public class SubFilesStatisticDataModel
{
    public int SubFilesQuantity { get; set; }

    public bool HasSubdirectories { get; set; }

    public long ByteSize { get; set; }

    public SubFilesStatisticDataModel(int subFilesQuantity, bool hasSubdirectories, long byteSize)
    {
        SubFilesQuantity = subFilesQuantity;
        HasSubdirectories = hasSubdirectories;
        ByteSize = byteSize;
    }
}