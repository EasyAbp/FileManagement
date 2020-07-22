using EasyAbp.FileManagement.Localization;
using Volo.Abp.Application.Services;

namespace EasyAbp.FileManagement
{
    public abstract class FileManagementAppService : ApplicationService
    {
        protected FileManagementAppService()
        {
            LocalizationResource = typeof(FileManagementResource);
            ObjectMapperContext = typeof(FileManagementApplicationModule);
        }
    }
}
