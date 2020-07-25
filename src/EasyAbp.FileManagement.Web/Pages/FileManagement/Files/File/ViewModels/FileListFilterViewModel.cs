using System;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels
{
    public class FileListFilterViewModel
    {
        public string FileContainerName { get; set; }
        
        public Guid? OwnerUserId { get; set; }
    }
}