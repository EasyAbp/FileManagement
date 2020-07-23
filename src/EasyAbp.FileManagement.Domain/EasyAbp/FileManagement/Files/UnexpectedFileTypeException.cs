using System;
using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class UnexpectedFileTypeException : BusinessException
    {
        public UnexpectedFileTypeException(Guid fileId, FileType fileType) : base(
            message: $"The type ({fileType}) of the file ({fileId}) is unexpected.")
        {
        }
        
        public UnexpectedFileTypeException(Guid fileId, FileType fileType, FileType expectedFileType) : base(
            message: $"The type ({fileType}) of the file ({fileId}) is unexpected, it should be {expectedFileType}.")
        {
        }
    }
}