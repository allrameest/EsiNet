#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["samples/Sample.Gateway/Sample.Gateway.csproj", "samples/Sample.Gateway/"]
RUN dotnet restore "samples/Sample.Gateway/Sample.Gateway.csproj"
COPY . .
WORKDIR "/src/samples/Sample.Gateway"
RUN dotnet build "Sample.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.Gateway.dll"]