using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace EasyAbp.FileManagement.Files;

public class FileDomainTests : FileManagementDomainTestBase
{
    public FileDomainTests()
    {
    }

    [Fact]
    public async Task Should_Update_Statistic_Data()
    {
        var dirs = await FileRepository.GetListAsync(null, "test", null, FileType.Directory);

        dirs.Count.ShouldBe(2);

        var dir1 = dirs.FirstOrDefault(x => x.FileName == "dir1");
        var dir2 = dirs.FirstOrDefault(x => x.FileName == "dir2");

        dir1.ShouldNotBeNull();
        dir2.ShouldNotBeNull();

        dir1.SubFilesQuantity.ShouldBe(2);
        dir1.ByteSize.ShouldBe(7 + 8);
        dir2.SubFilesQuantity.ShouldBe(2);
        dir2.ByteSize.ShouldBe(0);

        var dirs1 = await FileRepository.GetListAsync(dir1.Id, "test", null, FileType.Directory);

        dirs1.Count.ShouldBe(2);

        var dir11 = dirs1.FirstOrDefault(x => x.FileName == "dir11");
        var dir12 = dirs1.FirstOrDefault(x => x.FileName == "dir12");

        dir11.ShouldNotBeNull();
        dir12.ShouldNotBeNull();

        dir11.SubFilesQuantity.ShouldBe(1);
        dir11.ByteSize.ShouldBe(7);
        dir12.SubFilesQuantity.ShouldBe(1);
        dir12.ByteSize.ShouldBe(8);

        var files11 = await FileRepository.GetListAsync(dir11.Id, "test", null);

        files11.Count.ShouldBe(1);

        var file1 = files11.FirstOrDefault(x => x.FileName == "file1.txt");

        file1.ShouldNotBeNull();

        await WithUnitOfWorkAsync(async () =>
        {
            await FileManager.MoveAsync(file1, new MoveFileModel(dir2, file1.FileName, file1.MimeType));
        });

        dir1 = await FileRepository.GetAsync(dir1.Id);
        dir1.SubFilesQuantity.ShouldBe(2);
        dir1.ByteSize.ShouldBe(8);

        dir11 = await FileRepository.GetAsync(dir11.Id);
        dir11.SubFilesQuantity.ShouldBe(0);
        dir11.ByteSize.ShouldBe(0);

        dir2 = await FileRepository.GetAsync(dir2.Id);
        dir2.SubFilesQuantity.ShouldBe(3);
        dir2.ByteSize.ShouldBe(7);

        Guid newFileId = default;

        await WithUnitOfWorkAsync(async () =>
        {
            var newFile = await FileManager.CreateAsync(new CreateFileModel("test", null, "new-file.txt", null,
                FileType.RegularFile, dir12, "new-content"u8.ToArray()));

            newFileId = newFile.Id;
        });

        dir1 = await FileRepository.GetAsync(dir1.Id);
        dir1.SubFilesQuantity.ShouldBe(2);
        dir1.ByteSize.ShouldBe(8 + 11);

        dir12 = await FileRepository.GetAsync(dir12.Id);
        dir12.SubFilesQuantity.ShouldBe(2);
        dir12.ByteSize.ShouldBe(8 + 11);

        await WithUnitOfWorkAsync(async () =>
        {
            var newFile = await FileRepository.GetAsync(newFileId);
            await FileManager.DeleteAsync(newFile);
        });

        dir1 = await FileRepository.GetAsync(dir1.Id);
        dir1.SubFilesQuantity.ShouldBe(2);
        dir1.ByteSize.ShouldBe(8);

        dir12 = await FileRepository.GetAsync(dir12.Id);
        dir12.SubFilesQuantity.ShouldBe(1);
        dir12.ByteSize.ShouldBe(8);
    }

    [Fact]
    public async Task Should_SoftDeletionToken_Work()
    {
        var tenantId = Guid.NewGuid(); // since SQLite doesn't support unique index with null values.
        var ownerUserId = Guid.NewGuid(); // since SQLite doesn't support unique index with null values.

        var softDeleteFilter = GetRequiredService<IDataFilter<ISoftDelete>>();
        var currentTenant = GetRequiredService<ICurrentTenant>();

        using var changeTenant = currentTenant.Change(tenantId);

        // since SQLite doesn't support unique index with null values.
        var parent = await FileRepository.InsertAsync(new File(Guid.NewGuid(), tenantId, null,
            "test", "parent", null, FileType.Directory, 0, 0, null, null, ownerUserId), true);

        var dir = await FileRepository.InsertAsync(new File(Guid.NewGuid(), tenantId, parent,
            "test", "dir", null, FileType.Directory, 0, 0, null, null, ownerUserId), true);

        await Should.ThrowAsync<DbUpdateException>(() =>
            FileRepository.InsertAsync(new File(Guid.NewGuid(), tenantId, parent,
                "test", "dir", null, FileType.Directory, 0, 0, null, null, ownerUserId), true));

        await FileManager.DeleteAsync(dir);

        await Should.NotThrowAsync(() =>
            FileRepository.InsertAsync(new File(Guid.NewGuid(), tenantId, parent,
                "test", "dir", null, FileType.Directory, 0, 0, null, null, ownerUserId), true));

        using (softDeleteFilter.Disable())
        {
            var files = await FileRepository.GetListAsync(parent.Id, "test", ownerUserId, FileType.Directory);

            files.ShouldContain(x => x.FileName == "dir" && x.SoftDeletionToken == string.Empty);
            files.ShouldContain(x => x.FileName == "dir" && x.SoftDeletionToken != string.Empty);
        }
    }
}