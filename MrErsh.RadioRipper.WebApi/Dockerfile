#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MrErsh.RadioRipper.Client.WebApi/MrErsh.RadioRipper.WebApi.csproj", "WebApi/"]
COPY ["MrErsh.RadioRipper.Client.Dal/MrErsh.RadioRipper.Dal.csproj", "Dal/"]
COPY ["MrErsh.RadioRipper.Client.Model/MrErsh.RadioRipper.Model.csproj", "Model/"]
COPY ["MrErsh.RadioRipper.Client.RadioRipperCore/MrErsh.RadioRipper.Core.csproj", "RadioRipperCore/"]
COPY ["MrErsh.RadioRipper.IdentityDal/MrErsh.RadioRipper.IdentityDal.csproj", "MrErsh.RadioRipper.IdentityDal/"]
RUN dotnet restore "MrErsh.RadioRipper.Client.WebApi/MrErsh.RadioRipper.WebApi.csproj"
COPY . .
WORKDIR "/src/MrErsh.RadioRipper.Client.WebApi"
RUN dotnet build "MrErsh.RadioRipper.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MrErsh.RadioRipper.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MrErsh.RadioRipper.WebApi.dll"]
