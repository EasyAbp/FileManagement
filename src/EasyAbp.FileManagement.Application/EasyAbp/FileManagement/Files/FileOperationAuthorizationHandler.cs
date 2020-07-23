using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EasyAbp.FileManagement.Files
{
    public abstract class FileOperationAuthorizationHandler : AuthorizationHandler<FileOperationAuthorizationRequirement, File>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            FileOperationAuthorizationRequirement requirement, File resource)
        {
            throw new System.NotImplementedException();
        }
        
        protected abstract Task<bool> HasCreatePermissionAsync(AuthorizationHandlerContext context, File resource);
        
        protected abstract Task<bool> HasUpdatePermissionAsync(AuthorizationHandlerContext context, File resource);
         
        protected abstract Task<bool> HasMovePermissionAsync(AuthorizationHandlerContext context, File resource);
         
        protected abstract Task<bool> HasDeletePermissionAsync(AuthorizationHandlerContext context, File resource);
    }
}