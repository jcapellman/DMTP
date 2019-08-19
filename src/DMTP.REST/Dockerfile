FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["DMTP.REST/DMTP.REST.csproj", "DMTP.REST/"]
COPY ["DMTP.lib/DMTP.lib.csproj", "DMTP.lib/"]
RUN dotnet restore "DMTP.REST/DMTP.REST.csproj"
COPY . .
WORKDIR "/src/DMTP.REST"
RUN dotnet build "DMTP.REST.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DMTP.REST.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DMTP.REST.dll"]