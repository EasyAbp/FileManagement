using System;
using System.ComponentModel.DataAnnotations;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;

public class MoveFileViewModel
{
    [Display(Name = "FileParentId")]
    public Guid? NewParentId { get; set; }
        
    [Required]
    [Display(Name = "FileNewFileName")]
    public string NewFileName { get; set; }
}