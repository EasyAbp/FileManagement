using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Directories
{
    public class BasicDirectoryOperationAuthorizationHandler : DirectoryOperationAuthorizationHandler, ISingletonDependency
    {
        protected override async Task<bool> HasCreatePermissionAsync(AuthorizationHandlerContext context, Directory resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasUpdatePermissionAsync(AuthorizationHandlerContext context, Directory resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasMovePermissionAsync(AuthorizationHandlerContext context, Directory resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasDeletePermissionAsync(AuthorizationHandlerContext context, Directory resource)
        {
            throw new System.NotImplementedException();
        }
    }
}