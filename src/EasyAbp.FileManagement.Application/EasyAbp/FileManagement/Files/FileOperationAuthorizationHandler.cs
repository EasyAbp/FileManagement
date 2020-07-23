using System.Threading.Tasks;
using EasyAbp.FileManagement.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace EasyAbp.FileManagement.Files
{
    public abstract class FileOperationAuthorizationHandler : AuthorizationHandler<FileOperationAuthorizationRequirement, File>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            FileOperationAuthorizationRequirement requirement, File resource)
        {
            var hasPermission = requirement.OperationName switch
            {
                FileManagementPermissions.File.Default => await HasGetInfoPermissionAsync(context, resource),
                FileManagementPermissions.File.Download => await HasDownloadPermissionAsync(context, resource),
                FileManagementPermissions.File.Create => await HasCreatePermissionAsync(context, resource),
                FileManagementPermissions.File.Update => await HasUpdatePermissionAsync(context, resource),
                FileManagementPermissions.File.Move => await HasMovePermissionAsync(context, resource),
                FileManagementPermissions.File.Delete => await HasDeletePermissionAsync(context, resource),
                _ => false
            };

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
        
        protected abstract Task<bool> HasGetInfoPermissionAsync(AuthorizationHandlerContext context, File resource);
        
        protected abstract Task<bool> HasDownloadPermissionAsync(AuthorizationHandlerContext context, File resource);
        
        protected abstract Task<bool> HasCreatePermissionAsync(AuthorizationHandlerContext context, File resource);
        
        protected abstract Task<bool> HasUpdatePermissionAsync(AuthorizationHandlerContext context, File resource);
         
        protected abstract Task<bool> HasMovePermissionAsync(AuthorizationHandlerContext context, File resource);
         
        protected abstract Task<bool> HasDeletePermissionAsync(AuthorizationHandlerContext context, File resource);
    }
}