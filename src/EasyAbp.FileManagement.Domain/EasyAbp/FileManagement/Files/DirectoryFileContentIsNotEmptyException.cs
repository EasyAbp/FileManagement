using System;
using Volo.Abp;

namespace EasyAbp.FileManagement.Files
{
    public class DirectoryFileContentIsNotEmptyException : BusinessException
    {
        public DirectoryFileContentIsNotEmptyException() : base(message: "Content should be empty if the file is a directory.")
        {
        }
    }
}