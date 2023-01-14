using System.Threading.Tasks;
using EasyAbp.FileManagement.Options.Containers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.FileManagement.Files;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(BasicFileOperationAuthorizationHandler))]
public class MyBasicFileOperationAuthorizationHandler : BasicFileOperationAuthorizationHandler
{
    public MyBasicFileOperationAuthorizationHandler(IFileContainerConfigurationProvider configurationProvider,
        IPermissionChecker permissionChecker) : base(configurationProvider, permissionChecker)
    {
    }

    protected override async Task HandleCreateAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, FileOperationInfoModel resource)
    {
        if (resource.FileContainerName == "AnonymousUpload")
        {
            return;
        }

        await base.HandleCreateAsync(context, requirement, resource);
    }
}