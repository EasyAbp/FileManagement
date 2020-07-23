using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files
{
    public class BasicFileOperationAuthorizationHandler : FileOperationAuthorizationHandler, ISingletonDependency
    {
        protected override async Task<bool> HasCreatePermissionAsync(AuthorizationHandlerContext context, File resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasUpdatePermissionAsync(AuthorizationHandlerContext context, File resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasMovePermissionAsync(AuthorizationHandlerContext context, File resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasDeletePermissionAsync(AuthorizationHandlerContext context, File resource)
        {
            throw new System.NotImplementedException();
        }
    }
}