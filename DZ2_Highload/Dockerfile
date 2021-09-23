FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Debug/net5.0 App/
COPY httptest/ App/httptest
COPY config.config App/
WORKDIR /App
ENTRYPOINT ["dotnet", "DZ2_Highload.dll"]

