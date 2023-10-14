using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace EasyAbp.FileManagement.Files;

public abstract class FileOperationAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, IFileOperationInfoModel>
{
    protected string[] SpecifiedFileContainerNames { get; set; }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, IFileOperationInfoModel resource)
    {
        if (!SpecifiedFileContainerNames.IsNullOrEmpty() &&
            !SpecifiedFileContainerNames.Contains(resource.FileContainerName))
        {
            return;
        }

        switch (requirement.Name)
        {
            case FileManagementPermissions.File.Default:
                switch (resource)
                {
                    case FileGetListOperationInfoModel getListModel:
                        await HandleGetListAsync(context, requirement, getListModel);
                        break;
                    case FileGetInfoOperationInfoModel getInfoModel:
                        await HandleGetInfoAsync(context, requirement, getInfoModel);
                        break;
                }

                break;
            case FileManagementPermissions.File.GetDownloadInfo:
                await HandleGetDownloadInfoAsync(context, requirement, (FileGetDownloadInfoOperationInfoModel)resource);
                break;
            case FileManagementPermissions.File.Create:
                await HandleCreateAsync(context, requirement, (FileCreationOperationInfoModel)resource);
                break;
            case FileManagementPermissions.File.Update:
                await HandleUpdateInfoAsync(context, requirement, (FileUpdateInfoOperationInfoModel)resource);
                break;
            case FileManagementPermissions.File.Move:
                await HandleMoveAsync(context, requirement, (FileMoveOperationInfoModel)resource);
                break;
            case FileManagementPermissions.File.Delete:
                await HandleDeleteAsync(context, requirement, (FileDeletionOperationInfoModel)resource);
                break;
        }
    }

    protected abstract Task HandleGetInfoAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileGetInfoOperationInfoModel resource);

    protected abstract Task HandleGetListAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileGetListOperationInfoModel resource);

    protected abstract Task HandleGetDownloadInfoAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileGetDownloadInfoOperationInfoModel resource);

    protected abstract Task HandleCreateAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileCreationOperationInfoModel resource);

    protected abstract Task HandleUpdateInfoAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileUpdateInfoOperationInfoModel resource);

    protected abstract Task HandleMoveAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileMoveOperationInfoModel resource);

    protected abstract Task HandleDeleteAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileDeletionOperationInfoModel resource);
}