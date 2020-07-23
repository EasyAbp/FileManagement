using Microsoft.AspNetCore.Authorization;

namespace EasyAbp.FileManagement.Common
{
    public class FileContainerAuthorizationRequirement : IAuthorizationRequirement
    {
        public string FileContainerName { get; set; }
        
        public string OperationName { get; set; }
    }
}