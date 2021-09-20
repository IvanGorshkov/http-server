#FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
#WORKDIR /app
#
#FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
#WORKDIR /src
#COPY ["DZ2_Highload/DZ2_Highload.csproj", "DZ2_Highload/"]
#RUN dotnet restore "DZ2_Highload/DZ2_Highload.csproj"
#COPY . .
#WORKDIR "/src/DZ2_Highload"
#RUN dotnet build "DZ2_Highload.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "DZ2_Highload.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "DZ2_Highload.dll"]

# FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
FROM ubuntu:latest as executor

RUN apt-get update && apt-get install -y wget

RUN wget https://packages.microsoft.com/config/ubuntu/20.10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN  dpkg -i packages-microsoft-prod.deb

RUN apt-get update; \
    apt-get install -y apt-transport-https && \
    apt-get update && \
    apt-get install -y dotnet-sdk-5.0

WORKDIR /app

COPY . /app

RUN dotnet build

WORKDIR /app/bin/Debug/net5.0

ENTRYPOINT [ "/app/bin/Debug/net5.0/DZ2_Highload" ]
