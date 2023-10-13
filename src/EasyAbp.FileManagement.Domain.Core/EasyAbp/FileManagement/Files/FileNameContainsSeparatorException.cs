using System;
using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class FileNameContainsSeparatorException : BusinessException
    {
        public FileNameContainsSeparatorException(string fileName, char separator) : base(
            message: $"The file name ({fileName}) should not contains the separator ({separator}).")
        {
        }
    }
}