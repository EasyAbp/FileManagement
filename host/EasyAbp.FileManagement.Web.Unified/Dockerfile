#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["host/EasyAbp.FileManagement.Web.Unified/EasyAbp.FileManagement.Web.Unified.csproj", "host/EasyAbp.FileManagement.Web.Unified/"]
COPY ["src/EasyAbp.FileManagement.Application/EasyAbp.FileManagement.Application.csproj", "src/EasyAbp.FileManagement.Application/"]
COPY ["src/EasyAbp.FileManagement.Domain/EasyAbp.FileManagement.Domain.csproj", "src/EasyAbp.FileManagement.Domain/"]
COPY ["src/EasyAbp.FileManagement.Domain.Shared/EasyAbp.FileManagement.Domain.Shared.csproj", "src/EasyAbp.FileManagement.Domain.Shared/"]
COPY ["src/EasyAbp.FileManagement.Application.Contracts/EasyAbp.FileManagement.Application.Contracts.csproj", "src/EasyAbp.FileManagement.Application.Contracts/"]
COPY ["host/EasyAbp.FileManagement.Host.Shared/EasyAbp.FileManagement.Host.Shared.csproj", "host/EasyAbp.FileManagement.Host.Shared/"]
COPY ["src/EasyAbp.FileManagement.Web/EasyAbp.FileManagement.Web.csproj", "src/EasyAbp.FileManagement.Web/"]
COPY ["src/EasyAbp.FileManagement.HttpApi/EasyAbp.FileManagement.HttpApi.csproj", "src/EasyAbp.FileManagement.HttpApi/"]
COPY ["src/EasyAbp.FileManagement.EntityFrameworkCore/EasyAbp.FileManagement.EntityFrameworkCore.csproj", "src/EasyAbp.FileManagement.EntityFrameworkCore/"]
RUN dotnet restore "host/EasyAbp.FileManagement.Web.Unified/EasyAbp.FileManagement.Web.Unified.csproj"
COPY . .
WORKDIR "/src/host/EasyAbp.FileManagement.Web.Unified"
RUN dotnet build "EasyAbp.FileManagement.Web.Unified.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyAbp.FileManagement.Web.Unified.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyAbp.FileManagement.Web.Unified.dll"]