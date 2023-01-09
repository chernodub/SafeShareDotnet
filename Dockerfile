FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy project spec
COPY SafeShare.csproj ./
# Restore as distinct layers
RUN dotnet restore
# Copy the codebase
COPY . ./
# Build and publish a release
RUN  dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "SafeShare.dll"]
