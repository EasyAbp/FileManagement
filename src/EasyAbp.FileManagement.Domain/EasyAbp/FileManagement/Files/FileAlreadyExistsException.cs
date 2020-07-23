using System;
using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class FileAlreadyExistsException : BusinessException
    {
        public FileAlreadyExistsException(string filePath) : base(message: $"The file ({filePath}) already exists")
        {
        }
    }
}