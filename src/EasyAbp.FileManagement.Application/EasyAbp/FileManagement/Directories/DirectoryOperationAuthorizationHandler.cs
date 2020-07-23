using System.Threading.Tasks;
using EasyAbp.FileManagement.Common;
using Microsoft.AspNetCore.Authorization;

namespace EasyAbp.FileManagement.Directories
{
    public abstract class DirectoryOperationAuthorizationHandler : AuthorizationHandler<FileContainerAuthorizationRequirement, Directory>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            FileContainerAuthorizationRequirement requirement, Directory resource)
        {
            throw new System.NotImplementedException();
        }
        
        protected abstract Task<bool> HasCreatePermissionAsync(AuthorizationHandlerContext context, Directory resource);
        
        protected abstract Task<bool> HasUpdatePermissionAsync(AuthorizationHandlerContext context, Directory resource);
         
        protected abstract Task<bool> HasMovePermissionAsync(AuthorizationHandlerContext context, Directory resource);
         
        protected abstract Task<bool> HasDeletePermissionAsync(AuthorizationHandlerContext context, Directory resource);
    }
}