using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.FileInfos
{
    public class BasicFileOperationAuthorizationHandler : FileOperationAuthorizationHandler, ISingletonDependency
    {
        protected override async Task<bool> HasCreatePermissionAsync(AuthorizationHandlerContext context, FileInfo resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasUpdatePermissionAsync(AuthorizationHandlerContext context, FileInfo resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasMovePermissionAsync(AuthorizationHandlerContext context, FileInfo resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasDeletePermissionAsync(AuthorizationHandlerContext context, FileInfo resource)
        {
            throw new System.NotImplementedException();
        }
    }
}