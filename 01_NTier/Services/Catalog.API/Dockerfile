FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50518
EXPOSE 44375

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Catalog/Catalog.API.csproj Catalog/
RUN dotnet restore Catalog/Catalog.API.csproj
COPY . .
WORKDIR /src/Catalog
RUN dotnet build Catalog.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Catalog.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
