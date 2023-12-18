using System;
using System.ComponentModel.DataAnnotations;
using EasyAbp.FileManagement.Files;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;

public class FileDetailViewModel
{
    [HiddenInput]
    [Display(Name = "FileFileName")]
    public string FileName { get; set; }

    [DisabledInput]
    [Display(Name = "FileFileType")]
    public FileType FileType { get; set; }

    [DisabledInput]
    [Display(Name = "Location")]
    public string Location { get; set; }

    [DisabledInput]
    [Display(Name = "Creator")]
    public string Creator { get; set; }

    [DisabledInput]
    [Display(Name = "CreationTime")]
    public DateTime Created { get; set; }

    [DisabledInput]
    [Display(Name = "LastModifier")]
    public string LastModifier { get; set; }

    [DisabledInput]
    [Display(Name = "LastModificationTime")]
    public DateTime Modified { get; set; }
}