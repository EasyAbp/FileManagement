# FileManagement

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=%2F%2FProject%2FPropertyGroup%2FAbpVersion&url=https%3A%2F%2Fraw.githubusercontent.com%2FEasyAbp%2FFileManagement%2Fmaster%2FDirectory.Build.props)](https://abp.io)
[![NuGet](https://img.shields.io/nuget/v/EasyAbp.FileManagement.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.FileManagement.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.FileManagement.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.FileManagement.Domain.Shared)
[![GitHub stars](https://img.shields.io/github/stars/EasyAbp/FileManagement?style=social)](https://www.github.com/EasyAbp/FileManagement)

An abp application module that allows users to upload and manage their own files based on the ABP BLOB storing.

## Online Demo

We have launched an online demo for this module: [https://file.samples.easyabp.io](https://file.samples.easyabp.io)

## Installation

1. Install the following NuGet packages. ([see how](https://github.com/EasyAbp/EasyAbpGuide/blob/master/docs/How-To.md#add-nuget-packages))

    * EasyAbp.FileManagement.Application
    * EasyAbp.FileManagement.Application.Contracts
    * EasyAbp.FileManagement.Domain
    * EasyAbp.FileManagement.Domain.Shared
    * EasyAbp.FileManagement.EntityFrameworkCore
    * EasyAbp.FileManagement.HttpApi
    * EasyAbp.FileManagement.HttpApi.Client
    * (Optional) EasyAbp.FileManagement.MongoDB
    * (Optional) EasyAbp.FileManagement.Web

1. Add `DependsOn(typeof(FileManagementXxxModule))` attribute to configure the module dependencies. ([see how](https://github.com/EasyAbp/EasyAbpGuide/blob/master/docs/How-To.md#add-module-dependencies))

1. Add `builder.ConfigureFileManagement();` to the `OnModelCreating()` method in **MyProjectMigrationsDbContext.cs**.

1. Add EF Core migrations and update your database. See: [ABP document](https://docs.abp.io/en/abp/latest/Tutorials/Part-1?UI=MVC&DB=EF#add-database-migration).

## Usage

1. Add permissions to the roles you want.

1. Configure a BLOB container. (see [sample](https://github.com/EasyAbp/FileManagement/blob/master/host/EasyAbp.FileManagement.Web.Unified/FileManagementWebUnifiedModule.cs#L121-L132) and [doc](https://docs.abp.io/en/abp/latest/Blob-Storing))

1. Configure a file container. (see [sample](https://github.com/EasyAbp/FileManagement/blob/master/host/EasyAbp.FileManagement.Web.Unified/FileManagementWebUnifiedModule.cs#L134-L158))

1. Create a custom **FileOperationAuthorizationHandler**. (see [sample](https://github.com/EasyAbp/FileManagement/blob/master/host/EasyAbp.FileManagement.Web.Unified/CommonContainerFileOperationAuthorizationHandler.cs))

![Files](/docs/images/Files.png)
![Upload](/docs/images/Upload.png)

## Road map

- [x] Multi container.
- [x] Multi file upload.
- [x] Upload constraints.
- [x] User-space isolation.
- [x] Reuse existing BLOB resources.
- [x] Directory occupancy statistics.
- [x] Auto deleting unused BLOB resources.
- [x] Auto rename files with duplicate names.
- [ ] Container space quota control.
- [ ] Customized upload way.
- [ ] Complex file search.
- [ ] Unit tests.
