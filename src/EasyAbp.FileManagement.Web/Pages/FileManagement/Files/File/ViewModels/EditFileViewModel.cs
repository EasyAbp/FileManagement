using System.ComponentModel.DataAnnotations;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Files.File.ViewModels
{
    public class EditFileViewModel
    {
        [Required]
        [Display(Name = "FileFileName")]
        public string FileName { get; set; }

        [Display(Name = "FileMimeType")]
        public string MimeType { get; set; }
    }
}