using System;

namespace EasyAbp.FileManagement.Files
{
    public class IncorrectParentException : ApplicationException
    {
        public IncorrectParentException(File parent) : base($"The inputted parent (id: {parent?.Id}) entity is incorrect.")
        {
        }
    }
}