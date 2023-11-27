using System;
using System.Threading.Tasks;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Files.Dtos;
using EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EasyAbp.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class RenameModalModel : FileManagementPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public RenameFileViewModel ViewModel { get; set; }

        
    private readonly IFileAppService _service;

    public RenameModalModel(IFileAppService service)
    {
        _service = service;
    }

    public virtual async Task OnGetAsync()
    {
        var dto = await _service.GetAsync(Id);
        ViewModel = ObjectMapper.Map<FileInfoDto, RenameFileViewModel>(dto);
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        var dto = new UpdateFileInfoInput
        {
            FileName = ViewModel.FileName,
        };
            
        await _service.UpdateInfoAsync(Id, dto);

        return NoContent();
    }
}