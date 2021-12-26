using EasyAbp.FileManagement.Localization;
using Volo.Abp.AspNetCore.Components;

namespace EasyAbp.FileManagement.Blazor.Server.Host
{
    public abstract class FileManagementComponentBase : AbpComponentBase
    {
        protected FileManagementComponentBase()
        {
            LocalizationResource = typeof(FileManagementResource);
        }
    }
}
