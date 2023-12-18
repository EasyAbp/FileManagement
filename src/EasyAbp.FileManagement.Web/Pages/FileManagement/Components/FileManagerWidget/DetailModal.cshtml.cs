using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class DetailModalModel : FileManagementPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public FileDetailViewModel ViewModel { get; set; }

    private readonly IFileAppService _service;

    public DetailModalModel(IFileAppService service)
    {
        _service = service;
    }

    public virtual async Task OnGetAsync()
    {
        var dto = await _service.GetAsync(Id);
        ViewModel = new FileDetailViewModel
        {
            FileName = dto.FileName,
            FileType = dto.FileType,
            Location = (await _service.GetLocationAsync(dto.Id)).Location,
            Creator = dto.Creator?.UserName,
            Created = dto.CreationTime,
            LastModifier = dto.LastModifier?.UserName,
            Modified = dto.LastModificationTime ?? dto.CreationTime
        };
    }
}