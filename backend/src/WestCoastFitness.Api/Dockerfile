FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY backend/src/WestCoastFitness.Api/WestCoastFitness.Api.csproj backend/src/WestCoastFitness.Api/
RUN dotnet restore backend/src/WestCoastFitness.Api/WestCoastFitness.Api.csproj
COPY backend/src/WestCoastFitness.Api/ backend/src/WestCoastFitness.Api/
WORKDIR /src/backend/src/WestCoastFitness.Api
RUN dotnet publish --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "WestCoastFitness.Api.dll"]
