# Compile Angular app with CLI
FROM node:13 as node-build
WORKDIR /usr/src/app

COPY AnimalCrossing.Angular/package*.json ./
RUN npm install

COPY ./AnimalCrossing.Angular/. .
RUN npm run ng build -- --prod

# Compile ASP.NET Core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS dotnetcore-build
WORKDIR /app

# Copy csproj files and restore NuGet packages
COPY *.sln ./
COPY AnimalCrossing.Web/*.csproj ./AnimalCrossing.Web/
COPY AnimalCrossing.Web.Tests/*.csproj ./AnimalCrossing.Web.Tests/
RUN dotnet restore

# Copy remaining code files and build
COPY ./AnimalCrossing.Web/ ./AnimalCrossing.Web/
COPY ./AnimalCrossing.Web.Tests/ ./AnimalCrossing.Web.Tests/
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY --from=dotnetcore-build /app/out .
COPY --from=node-build /usr/src/app/dist/AnimalCrossingAngular/ ./wwwroot/.
COPY ./AnimalCrossing.Web/images/ ./images/
ENTRYPOINT ["dotnet", "AnimalCrossing.Web.dll"]