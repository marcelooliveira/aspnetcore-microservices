FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY CQRS/MVC.csproj CQRS/
COPY Messages/Messages.csproj Messages/
RUN dotnet restore CQRS/MVC.csproj
COPY . .
WORKDIR /src/CQRS
RUN dotnet build MVC.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MVC.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MVC.dll"]
