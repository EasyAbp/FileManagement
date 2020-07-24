using System;

using System.ComponentModel.DataAnnotations;
using EasyAbp.FileManagement.Files;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels
{
    public class CreateFileViewModel
    {
        [Display(Name = "FileFileContainerName")]
        public string FileContainerName { get; set; }

        [Display(Name = "FileFileName")]
        public string FileName { get; set; }

        [Display(Name = "FileMimeType")]
        public string MimeType { get; set; }

        [Display(Name = "FileFileType")]
        public FileType FileType { get; set; }

        [Display(Name = "FileParentId")]
        public Guid? ParentId { get; set; }

        [Display(Name = "FileDisplayName")]
        public string DisplayName { get; set; }
        
        [Display(Name = "FileOwnerUserId")]
        public Guid? OwnerUserId { get; set; }
    }
}