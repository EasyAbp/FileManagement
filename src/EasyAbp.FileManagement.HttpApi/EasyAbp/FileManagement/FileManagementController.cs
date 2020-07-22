﻿using EasyAbp.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.FileManagement
{
    public abstract class FileManagementController : AbpController
    {
        protected FileManagementController()
        {
            LocalizationResource = typeof(FileManagementResource);
        }
    }
}
