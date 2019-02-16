FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY CasaDoCodigo.Basket/Basket.API.csproj CasaDoCodigo.Basket/
COPY CasaDoCodigo.Mensagens/CasaDoCodigo.Mensagens.csproj CasaDoCodigo.Mensagens/
RUN dotnet restore CasaDoCodigo.Basket/Basket.API.csproj
COPY . .
WORKDIR /src/CasaDoCodigo.Basket
RUN dotnet build Basket.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Basket.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Basket.API.dll"]
