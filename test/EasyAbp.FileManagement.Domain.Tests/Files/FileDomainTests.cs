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
        dir1.HasSubdirectories.ShouldBeTrue();
        dir1.ByteSize.ShouldBe(7 + 8);
        dir2.SubFilesQuantity.ShouldBe(2);
        dir2.HasSubdirectories.ShouldBeTrue();
        dir2.ByteSize.ShouldBe(0);

        var subdirs1 = await FileRepository.GetListAsync(dir1.Id, "test", null, FileType.Directory);

        subdirs1.Count.ShouldBe(2);

        var dir11 = subdirs1.FirstOrDefault(x => x.FileName == "dir11");
        var dir12 = subdirs1.FirstOrDefault(x => x.FileName == "dir12");

        dir11.ShouldNotBeNull();
        dir12.ShouldNotBeNull();

        dir11.SubFilesQuantity.ShouldBe(1);
        dir11.HasSubdirectories.ShouldBeFalse();
        dir11.ByteSize.ShouldBe(7);
        dir12.SubFilesQuantity.ShouldBe(1);
        dir11.HasSubdirectories.ShouldBeFalse();
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
        dir1.HasSubdirectories.ShouldBeTrue();
        dir1.ByteSize.ShouldBe(8);

        dir11 = await FileRepository.GetAsync(dir11.Id);
        dir11.SubFilesQuantity.ShouldBe(0);
        dir11.HasSubdirectories.ShouldBeFalse();
        dir11.ByteSize.ShouldBe(0);

        dir2 = await FileRepository.GetAsync(dir2.Id);
        dir2.SubFilesQuantity.ShouldBe(3);
        dir2.HasSubdirectories.ShouldBeTrue();
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
        dir1.HasSubdirectories.ShouldBeTrue();
        dir1.ByteSize.ShouldBe(8 + 11);

        dir12 = await FileRepository.GetAsync(dir12.Id);
        dir12.SubFilesQuantity.ShouldBe(2);
        dir12.HasSubdirectories.ShouldBeFalse();
        dir12.ByteSize.ShouldBe(8 + 11);

        await WithUnitOfWorkAsync(async () =>
        {
            var newFile = await FileRepository.GetAsync(newFileId);
            await FileManager.DeleteAsync(newFile);
        });

        dir1 = await FileRepository.GetAsync(dir1.Id);
        dir1.SubFilesQuantity.ShouldBe(2);
        dir1.HasSubdirectories.ShouldBeTrue();
        dir1.ByteSize.ShouldBe(8);

        dir12 = await FileRepository.GetAsync(dir12.Id);
        dir12.SubFilesQuantity.ShouldBe(1);
        dir12.HasSubdirectories.ShouldBeFalse();
        dir12.ByteSize.ShouldBe(8);

        await WithUnitOfWorkAsync(async () =>
        {
            await FileManager.CreateAsync(new CreateFileModel("test", null, "new-dir", null,
                FileType.Directory, dir12, null));
        });

        dir12 = await FileRepository.GetAsync(dir12.Id);
        dir12.SubFilesQuantity.ShouldBe(2);
        dir12.HasSubdirectories.ShouldBeTrue();
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
            "test", "parent", null, FileType.Directory, 0, null, null, ownerUserId), true);

        var dir = await FileRepository.InsertAsync(new File(Guid.NewGuid(), tenantId, parent,
            "test", "dir", null, FileType.Directory, 0, null, null, ownerUserId), true);

        await Should.ThrowAsync<DbUpdateException>(() =>
            FileRepository.InsertAsync(new File(Guid.NewGuid(), tenantId, parent,
                "test", "dir", null, FileType.Directory, 0, null, null, ownerUserId), true));

        await FileManager.DeleteAsync(dir);

        await Should.NotThrowAsync(() =>
            FileRepository.InsertAsync(new File(Guid.NewGuid(), tenantId, parent,
                "test", "dir", null, FileType.Directory, 0, null, null, ownerUserId), true));

        using (softDeleteFilter.Disable())
        {
            var files = await FileRepository.GetListAsync(parent.Id, "test", ownerUserId, FileType.Directory);

            files.ShouldContain(x => x.FileName == "dir" && x.SoftDeletionToken == string.Empty);
            files.ShouldContain(x => x.FileName == "dir" && x.SoftDeletionToken != string.Empty);
        }
    }

    [Fact]
    public async Task Should_New_File_Have_LastModificationTime_Value()
    {
        var dirId = Guid.NewGuid();

        await WithUnitOfWorkAsync(() => FileRepository.InsertAsync(new File(dirId, null, null,
            "test", "dir", null, FileType.Directory, 0, null, null, null), true));

        var dir = await FileRepository.GetAsync(dirId);

        dir.LastModificationTime.ShouldNotBeNull();
        dir.LastModificationTime.ShouldNotBe(DateTime.MinValue);
    }

    [Fact]
    public async Task Should_Get_File_Location()
    {
        var dir = await FileManager.CreateAsync(new CreateFileModel("test", null, "dir", null,
            FileType.Directory, null, null));

        var subDir = await FileManager.CreateAsync(new CreateFileModel("test", null, "sub-dir", null,
            FileType.Directory, dir, null));

        var file = await FileManager.CreateAsync(new CreateFileModel("test", null, "file.txt", null,
            FileType.RegularFile, subDir, "new-content"u8.ToArray()));

        var dirLocation = await FileManager.GetFileLocationAsync(dir);
        var subDirLocation = await FileManager.GetFileLocationAsync(subDir);
        var fileLocation = await FileManager.GetFileLocationAsync(file);

        dirLocation.ShouldNotBeNull();
        dirLocation.IsDirectory.ShouldBeTrue();
        dirLocation.FileName.ShouldBe("dir");
        dirLocation.FilePath.ShouldBe("dir");
        dirLocation.ParentPath.ShouldBe("");

        subDirLocation.ShouldNotBeNull();
        subDirLocation.IsDirectory.ShouldBeTrue();
        subDirLocation.FileName.ShouldBe("sub-dir");
        subDirLocation.FilePath.ShouldBe("dir/sub-dir");
        subDirLocation.ParentPath.ShouldBe("dir");

        fileLocation.ShouldNotBeNull();
        fileLocation.IsDirectory.ShouldBeFalse();
        fileLocation.FileName.ShouldBe("file.txt");
        fileLocation.FilePath.ShouldBe("dir/sub-dir/file.txt");
        fileLocation.ParentPath.ShouldBe("dir/sub-dir");
    }

    [Fact]
    public async Task Should_Get_File_By_Path()
    {
        var dir = await FileManager.CreateAsync(new CreateFileModel("test", null, "dir", null,
            FileType.Directory, null, null));

        var subDir = await FileManager.CreateAsync(new CreateFileModel("test", null, "sub-dir", null,
            FileType.Directory, dir, null));

        var file = await FileManager.CreateAsync(new CreateFileModel("test", null, "file.txt", null,
            FileType.RegularFile, subDir, "new-content"u8.ToArray()));

        (await FileManager.GetByPathAsync("dir", "test", null)).Id.ShouldBe(dir.Id);
        (await FileManager.GetByPathAsync("dir/sub-dir", "test", null)).Id.ShouldBe(subDir.Id);
        (await FileManager.GetByPathAsync("dir/sub-dir/file.txt", "test", null)).Id.ShouldBe(file.Id);
    }

    [Fact]
    public async Task Should_Rename_From_abc_To_ABC()
    {
        var dir = await FileManager.CreateAsync(new CreateFileModel("test", null, "abc", null,
            FileType.Directory, null, null));

        await Should.NotThrowAsync(() =>
            FileManager.UpdateInfoAsync(dir, new UpdateFileInfoModel("ABC", dir.MimeType)));

        dir.FileName.ShouldBe("ABC");

        dir = await FileRepository.GetAsync(dir.Id);

        dir.FileName.ShouldBe("ABC");
    }

    [Fact]
    public async Task Should_Rename_From_abc_To_abc()
    {
        var dir = await FileManager.CreateAsync(new CreateFileModel("test", null, "abc", null,
            FileType.Directory, null, null));

        await Should.NotThrowAsync(() =>
            FileManager.UpdateInfoAsync(dir, new UpdateFileInfoModel("abc", dir.MimeType)));

        dir.FileName.ShouldBe("abc");

        dir = await FileRepository.GetAsync(dir.Id);

        dir.FileName.ShouldBe("abc");
    }
}