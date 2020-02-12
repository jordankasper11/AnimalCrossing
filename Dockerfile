FROM node:13 as node-build
WORKDIR /usr/src/app
COPY AnimalCrossing.Angular/package*.json ./
RUN npm install
COPY ./AnimalCrossing.Angular/. .
RUN npm run ng build -- --prod

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS dotnet-build
WORKDIR /src
COPY AnimalCrossing.sln ./
COPY AnimalCrossing.Web/*.csproj ./AnimalCrossing.Web/
COPY AnimalCrossing.Web.Tests/*.csproj ./AnimalCrossing.Web.Tests/
RUN dotnet restore
COPY . .
WORKDIR "/src/."
RUN dotnet build "AnimalCrossing.sln" -c Release -o /app/build

FROM dotnet-build AS dotnet-publish
RUN dotnet publish "AnimalCrossing.sln" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY ./AnimalCrossing.Web/images/ ./images/
COPY --from=dotnet-publish /app/publish .
COPY --from=node-build /usr/src/app/dist/AnimalCrossingAngular/ ./wwwroot/.
ENTRYPOINT ["dotnet", "AnimalCrossing.Web.dll"]