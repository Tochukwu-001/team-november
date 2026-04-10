FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["LMS.Api/LMS.Api.csproj", "LMS.Api/"]
COPY ["LMS.Domain/LMS.Domain.csproj", "LMS.Domain/"]
COPY ["LMS.Application/LMS.Application.csproj", "LMS.Application/"]
COPY ["LMS.Infrastructure/LMS.Infrastructure.csproj", "LMS.Infrastructure/"]

RUN dotnet restore "./LMS.Api/LMS.Api.csproj"

COPY . .
WORKDIR "/src/LMS.Api"

RUN dotnet build "./LMS.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LMS.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LMS.Api.dll"]