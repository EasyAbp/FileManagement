using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files
{
    public class BasicFileOperationAuthorizationHandler : FileOperationAuthorizationHandler, ISingletonDependency
    {
        protected override async Task<bool> HasGetInfoPermissionAsync(AuthorizationHandlerContext context, FileOperationInfoModel resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasDownloadPermissionAsync(AuthorizationHandlerContext context, FileOperationInfoModel resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasCreatePermissionAsync(AuthorizationHandlerContext context, FileOperationInfoModel resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasUpdatePermissionAsync(AuthorizationHandlerContext context, FileOperationInfoModel resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasMovePermissionAsync(AuthorizationHandlerContext context, FileOperationInfoModel resource)
        {
            throw new System.NotImplementedException();
        }

        protected override async Task<bool> HasDeletePermissionAsync(AuthorizationHandlerContext context, FileOperationInfoModel resource)
        {
            throw new System.NotImplementedException();
        }
    }
}