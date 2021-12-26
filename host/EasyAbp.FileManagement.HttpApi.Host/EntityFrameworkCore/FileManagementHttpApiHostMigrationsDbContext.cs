﻿using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.FileManagement.EntityFrameworkCore
{
    public class FileManagementHttpApiHostMigrationsDbContext : AbpDbContext<FileManagementHttpApiHostMigrationsDbContext>
    {
        public FileManagementHttpApiHostMigrationsDbContext(DbContextOptions<FileManagementHttpApiHostMigrationsDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureFileManagement();
        }
    }
}
