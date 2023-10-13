using System;
using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class FileAlreadyExistsException : BusinessException
    {
        public FileAlreadyExistsException(string filePath) : base(message: $"The file ({filePath}) already exists")
        {
        }
        
        public FileAlreadyExistsException(string fileName, Guid? parentId) : base(message: $"The file (name: {fileName}, parentId: {parentId}) already exists")
        {
        }
    }
}