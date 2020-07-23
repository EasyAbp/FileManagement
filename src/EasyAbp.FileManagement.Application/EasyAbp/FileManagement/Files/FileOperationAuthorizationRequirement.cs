using Microsoft.AspNetCore.Authorization;

namespace EasyAbp.FileManagement.Files
{
    public class FileOperationAuthorizationRequirement : IAuthorizationRequirement
    {
        public string FileContainerName { get; set; }
        
        public string OperationName { get; set; }
    }
}