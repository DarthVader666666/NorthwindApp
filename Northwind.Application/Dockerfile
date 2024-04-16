#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Northwind.Application/Northwind.Application.csproj", "Northwind.Application/"]
COPY ["Northwind.Data/Northwind.Data.csproj", "Northwind.Data/"]
COPY ["Northwind.Bll/Northwind.Bll.csproj", "Northwind.Bll/"]
COPY ["Northwind.IntegrationTests/Northwind.IntegrationTests.csproj", "Northwind.IntegrationTests/"]
COPY ["Northwind.ModuleTests/Northwind.ModuleTests.csproj", "Northwind.ModuleTests/"]

RUN dotnet restore "./Northwind.Application/./Northwind.Application.csproj"
COPY . .
WORKDIR "/src/Northwind.Application"
RUN dotnet build "./Northwind.Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Northwind.Application.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Northwind.Application.dll"]