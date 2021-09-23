FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
RUN dotnet build "DZ2_Highload.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DZ2_Highload.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY config.config .
COPY httptest ./httptest
ENTRYPOINT ["dotnet", "DZ2_Highload.dll"]
