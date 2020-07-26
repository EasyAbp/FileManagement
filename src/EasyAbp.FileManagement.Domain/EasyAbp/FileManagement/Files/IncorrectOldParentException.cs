using System;

namespace EasyAbp.FileManagement.Files
{
    public class IncorrectOldParentException : ApplicationException
    {
        public IncorrectOldParentException(File parent) : base($"The inputted old parent ({parent?.Id}) entity is incorrect.")
        {
        }
    }
}