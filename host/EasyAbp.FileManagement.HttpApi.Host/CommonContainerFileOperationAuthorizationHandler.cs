using System;
using System.Security.Principal;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Options.Containers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace EasyAbp.FileManagement;

public class CommonContainerFileOperationAuthorizationHandler : FileOperationAuthorizationHandler, ITransientDependency
{
    private readonly IClock _clock;

    public CommonContainerFileOperationAuthorizationHandler(IClock clock)
    {
        _clock = clock;

        SpecifiedFileContainerNames = new[]
        {
            FileContainerNameAttribute.GetContainerName(typeof(CommonFileContainer)) // Only for CommonFileContainer
        };
    }

    protected override async Task HandleGetInfoAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileGetInfoOperationInfoModel resource)
    {
        context.Succeed(requirement); // Allow everyone to get the file info.
    }

    protected override async Task HandleGetListAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileGetListOperationInfoModel resource)
    {
        context.Succeed(requirement); // Allow everyone to see the files.
    }

    protected override async Task HandleGetDownloadInfoAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileGetDownloadInfoOperationInfoModel resource)
    {
        if (_clock.Now <= resource.File.CreationTime + TimeSpan.FromDays(7))
        {
            context.Succeed(requirement); // Everyone can download in 7 days from the file was uploaded.
            return;
        }

        // We don't set fail, so admin (with "manage" permission) can do it.
        // context.Fail();
    }

    protected override async Task HandleCreateAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileCreationOperationInfoModel resource)
    {
        if (context.User.FindUserId() == resource.OwnerUserId)
        {
            context.Succeed(requirement); // Owner users can upload a new file.
            return;
        }

        // We don't set fail, so admin (with "manage" permission) can do it.
        // context.Fail();
    }

    protected override async Task HandleUpdateInfoAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileUpdateInfoOperationInfoModel resource)
    {
        if (context.User.FindTenantId() == null && context.User.FindUserId() == resource.OwnerUserId)
        {
            context.Succeed(requirement); // Host-side owner users can update their uploaded files.
            return;
        }

        // We don't set fail, so admin (with "manage" permission) can do it.
        // context.Fail();
    }

    protected override async Task HandleMoveAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileMoveOperationInfoModel resource)
    {
        if (resource.File.FileType == FileType.Directory)
        {
            context.Fail(); // Directories (a special type of file) cannot be moved.
            return;
        }

        context.Succeed(requirement);
    }

    protected override async Task HandleDeleteAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileDeletionOperationInfoModel resource)
    {
        // Files cannot be deleted.

        // We don't set fail, so admin (with "manage" permission) can do it.
        // context.Fail();
    }
}