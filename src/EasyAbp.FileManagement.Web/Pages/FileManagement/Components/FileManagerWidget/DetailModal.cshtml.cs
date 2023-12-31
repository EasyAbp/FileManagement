using System;
using System.Globalization;
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
            MimeType = dto.MimeType,
            ByteSize = HumanFileSize(dto.ByteSize),
            Hash = dto.Hash,
            Location = (await _service.GetLocationAsync(dto.Id)).Location.FilePath,
            Creator = dto.Creator?.UserName,
            Created = ToDateTimeString(dto.CreationTime),
            LastModifier = dto.LastModifier?.UserName,
            Modified = ToDateTimeString(dto.LastModificationTime ?? dto.CreationTime)
        };
    }

    protected virtual string ToDateTimeString(DateTime time)
    {
        return time.ToString(CultureInfo.CurrentUICulture);
    }

    protected virtual string HumanFileSize(long bytes, bool si = false, int dp = 1)
    {
        var thresh = si ? 1000 : 1024;

        if (bytes == 0)
        {
            return "0 B";
        }

        if (Math.Abs(bytes) < thresh)
        {
            return bytes + " B";
        }

        var units = si
            ? new string[] { "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" }
            : new string[] { "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

        var u = -1;
        var b = bytes;
        const double r = 10.0d;

        do
        {
            b /= thresh;
            u++;
        } while (Math.Round(Math.Abs(b) * r) / r >= thresh && u < units.Length - 1);

        return b.ToString("F" + dp) + " " + units[u];
    }
}