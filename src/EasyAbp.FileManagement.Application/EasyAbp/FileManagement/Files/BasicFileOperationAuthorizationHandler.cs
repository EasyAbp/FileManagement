using System.Security.Principal;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.Options.Containers;
using EasyAbp.FileManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files;

public class BasicFileOperationAuthorizationHandler : FileOperationAuthorizationHandler, ISingletonDependency
{
    private readonly IFileContainerConfigurationProvider _configurationProvider;
    private readonly IPermissionChecker _permissionChecker;

    public BasicFileOperationAuthorizationHandler(
        IFileContainerConfigurationProvider configurationProvider,
        IPermissionChecker permissionChecker)
    {
        _configurationProvider = configurationProvider;
        _permissionChecker = permissionChecker;
    }

    protected override async Task HandleGetInfoAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileGetInfoOperationInfoModel resource)
    {
        await BasicCheckAsync(FileManagementPermissions.File.Default, context, requirement, resource);
    }

    protected override async Task HandleGetListAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileGetListOperationInfoModel resource)
    {
        await BasicCheckAsync(FileManagementPermissions.File.Default, context, requirement, resource);
    }

    protected override async Task HandleGetDownloadInfoAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileGetDownloadInfoOperationInfoModel resource)
    {
        await BasicCheckAsync(FileManagementPermissions.File.GetDownloadInfo, context, requirement, resource);
    }

    protected override async Task HandleCreateAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileCreationOperationInfoModel resource)
    {
        await BasicCheckAsync(FileManagementPermissions.File.Create, context, requirement, resource);
    }

    protected override async Task HandleUpdateInfoAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileUpdateInfoOperationInfoModel resource)
    {
        await BasicCheckAsync(FileManagementPermissions.File.Update, context, requirement, resource);
    }

    protected override async Task HandleMoveAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileMoveOperationInfoModel resource)
    {
        await BasicCheckAsync(FileManagementPermissions.File.Move, context, requirement, resource);
    }

    protected override async Task HandleDeleteAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileDeletionOperationInfoModel resource)
    {
        await BasicCheckAsync(FileManagementPermissions.File.Delete, context, requirement, resource);
    }

    protected virtual async Task BasicCheckAsync(string permissionName, AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, IFileOperationInfoModel resource)
    {
        await SetFailIfUserDoesNotHavePermissionAsync(permissionName, context);

        await SetSucceedIfUserIsManagerAsync(context, requirement);

        var configuration = _configurationProvider.Get<FileContainerConfiguration>(resource.FileContainerName);

        await SetFailIfUserIsNotPersonalContainerOwnerAsync(configuration, context, resource);
    }

    protected virtual async Task SetFailIfUserDoesNotHavePermissionAsync(string permissionName,
        AuthorizationHandlerContext context)
    {
        if (!await _permissionChecker.IsGrantedAsync(permissionName))
        {
            context.Fail();
        }
    }

    protected virtual async Task SetSucceedIfUserIsManagerAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement)
    {
        if (await _permissionChecker.IsGrantedAsync(FileManagementPermissions.File.Manage))
        {
            context.Succeed(requirement);
        }
    }

    protected virtual Task SetFailIfUserIsNotPersonalContainerOwnerAsync(FileContainerConfiguration configuration,
        AuthorizationHandlerContext context, IFileOperationInfoModel resource)
    {
        if (configuration.FileContainerType == FileContainerType.Private &&
            resource.OwnerUserId != context.User.FindUserId())
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}