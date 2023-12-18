using EasyAbp.FileManagement.Files;
using System;
using EasyAbp.FileManagement.Users;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Users.EntityFrameworkCore;

namespace EasyAbp.FileManagement.EntityFrameworkCore
{
    public static class FileManagementDbContextModelCreatingExtensions
    {
        public static void ConfigureFileManagement(
            this ModelBuilder builder,
            Action<FileManagementModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FileManagementModelBuilderConfigurationOptions(
                FileManagementDbProperties.DbTablePrefix,
                FileManagementDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            /* Configure all entities here. Example:

            builder.Entity<Question>(b =>
            {
                //Configure table & schema name
                b.ToTable(options.TablePrefix + "Questions", options.Schema);

                b.ConfigureByConvention();

                //Properties
                b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

                //Relations
                b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

                //Indexes
                b.HasIndex(q => q.CreationTime);
            });
            */

            builder.Entity<File>(b =>
            {
                b.ToTable(options.TablePrefix + "Files", options.Schema);
                b.ConfigureByConvention();

                /* Configure more properties here */
                b.Property(x => x.FileContainerName).HasMaxLength(FileManagementConsts.FileContainerNameMaxLength);
                b.Property(x => x.FileName).HasMaxLength(FileManagementConsts.File.FileNameMaxLength);
                b.Property(x => x.BlobName).HasMaxLength(FileManagementConsts.File.BlobNameMaxLength);
                b.Property(x => x.MimeType).HasMaxLength(FileManagementConsts.File.MimeTypeMaxLength);
                b.Property(x => x.Hash).HasMaxLength(FileManagementConsts.File.HashMaxLength);
                b.Property(x => x.Flag).HasMaxLength(FileManagementConsts.File.FlagMaxLength);
                b.Property(x => x.SoftDeletionToken).HasDefaultValue(string.Empty);
                b.HasIndex(x => x.BlobName);
                b.HasIndex(x => x.Hash);
                b.HasIndex(x => new { x.ParentId, x.OwnerUserId, x.FileContainerName, x.FileName });
                b.HasIndex(x => new
                        { x.FileName, x.ParentId, x.OwnerUserId, x.FileContainerName, x.TenantId, x.SoftDeletionToken })
                    .IsUnique().HasFilter(null);
            });

            builder.Entity<FileUser>(b =>
            {
                b.ToTable(options.TablePrefix + "Users", options.Schema);

                b.ConfigureByConvention();
                b.ConfigureAbpUser();
            });
        }
    }
}