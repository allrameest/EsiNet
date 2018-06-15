FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY samples/Sample.Gateway/Sample.Gateway.csproj samples/Sample.Gateway/
COPY src/EsiNet.AspNetCore/EsiNet.AspNetCore.csproj src/EsiNet.AspNetCore/
COPY src/EsiNet/EsiNet.csproj src/EsiNet/
RUN dotnet restore samples/Sample.Gateway/Sample.Gateway.csproj
COPY . .
WORKDIR /src/samples/Sample.Gateway
RUN dotnet build Sample.Gateway.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Sample.Gateway.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sample.Gateway.dll"]
