FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS base
WORKDIR /app
EXPOSE 5106
EXPOSE 44398

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Ordering.API/Ordering.API.csproj Ordering.API/
COPY Messages/Messages.csproj Messages/
RUN dotnet restore Ordering.API/Ordering.API.csproj
COPY . .
WORKDIR /src/Ordering.API
RUN dotnet build Ordering.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Ordering.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ordering.API.dll"]
