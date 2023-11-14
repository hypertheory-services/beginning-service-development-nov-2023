#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#  a fairly "slim" base image that has the .NET runtime stuff in it.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base 
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BusinessClockApi/BusinessClockApi.csproj", "BusinessClockApi/"]
RUN dotnet restore "BusinessClockApi/BusinessClockApi.csproj"
COPY . .
WORKDIR "/src/BusinessClockApi"
RUN dotnet build "BusinessClockApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BusinessClockApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BusinessClockApi.dll"]