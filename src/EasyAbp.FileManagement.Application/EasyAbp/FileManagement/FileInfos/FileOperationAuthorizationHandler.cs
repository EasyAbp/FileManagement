using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EasyAbp.FileManagement.FileInfos
{
    public abstract class FileOperationAuthorizationHandler : AuthorizationHandler<FileOperationAuthorizationRequirement, FileInfo>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            FileOperationAuthorizationRequirement requirement, FileInfo resource)
        {
            throw new System.NotImplementedException();
        }
        
        protected abstract Task<bool> HasCreatePermissionAsync(AuthorizationHandlerContext context, FileInfo resource);
        
        protected abstract Task<bool> HasUpdatePermissionAsync(AuthorizationHandlerContext context, FileInfo resource);
         
        protected abstract Task<bool> HasMovePermissionAsync(AuthorizationHandlerContext context, FileInfo resource);
         
        protected abstract Task<bool> HasDeletePermissionAsync(AuthorizationHandlerContext context, FileInfo resource);
    }
}