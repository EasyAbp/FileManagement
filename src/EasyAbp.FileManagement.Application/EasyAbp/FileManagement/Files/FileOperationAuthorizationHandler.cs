using System.Threading.Tasks;
using EasyAbp.FileManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace EasyAbp.FileManagement.Files
{
    public abstract class FileOperationAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, FileOperationInfoModel>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, FileOperationInfoModel resource)
        {
            switch (requirement.Name)
            {
                case FileManagementPermissions.File.Default:
                    await HandleGetInfoAsync(context, requirement, resource);
                    break;
                case FileManagementPermissions.File.GetDownloadInfo:
                    await HandleGetDownloadInfoAsync(context, requirement, resource);
                    break;
                case FileManagementPermissions.File.Create:
                    await HandleCreateAsync(context, requirement, resource);
                    break;
                case FileManagementPermissions.File.Update:
                    await HandleUpdateAsync(context, requirement, resource);
                    break;
                case FileManagementPermissions.File.Move:
                    await HandleMoveAsync(context, requirement, resource);
                    break;
                case FileManagementPermissions.File.Delete:
                    await HandleDeleteAsync(context, requirement, resource);
                    break;
            }
        }

        protected abstract Task HandleGetInfoAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, FileOperationInfoModel resource);

        protected abstract Task HandleGetDownloadInfoAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, FileOperationInfoModel resource);

        protected abstract Task HandleCreateAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, FileOperationInfoModel resource);

        protected abstract Task HandleUpdateAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, FileOperationInfoModel resource);

        protected abstract Task HandleMoveAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, FileOperationInfoModel resource);

        protected abstract Task HandleDeleteAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, FileOperationInfoModel resource);
    }
}