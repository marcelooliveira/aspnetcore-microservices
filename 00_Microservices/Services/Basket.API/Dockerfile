FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Basket.API/Basket.API.csproj Basket.API/
COPY Messages/Messages.csproj Messages/
RUN dotnet restore Basket.API/Basket.API.csproj
COPY . .
WORKDIR /src/Basket.API
RUN dotnet build Basket.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Basket.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Basket.API.dll"]
