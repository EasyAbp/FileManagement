using System.Threading.Tasks;
using EasyAbp.FileManagement.Common;
using Microsoft.AspNetCore.Authorization;

namespace EasyAbp.FileManagement.Files
{
    public abstract class FileOperationAuthorizationHandler : AuthorizationHandler<FileContainerAuthorizationRequirement, File>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            FileContainerAuthorizationRequirement requirement, File resource)
        {
            throw new System.NotImplementedException();
        }
        
        protected abstract Task<bool> HasCreatePermissionAsync(AuthorizationHandlerContext context, File resource);
        
        protected abstract Task<bool> HasUpdatePermissionAsync(AuthorizationHandlerContext context, File resource);
         
        protected abstract Task<bool> HasMovePermissionAsync(AuthorizationHandlerContext context, File resource);
         
        protected abstract Task<bool> HasDeletePermissionAsync(AuthorizationHandlerContext context, File resource);
    }
}