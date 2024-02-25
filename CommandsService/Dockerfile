# Specify image want to pull from DockerHub to start build (sdk image)
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
# Set working directory (destination)
WORKDIR /app


# Copy csproj into app working directory
COPY *.csproj ./
# Restore as distinct layers
RUN dotnet restore


# Copy everything else from source
COPY . ./
# Build app and publish a release
RUN dotnet publish -c Release -o out


# Build runtime image 
    # (pull .NET sdk image which we're using to build main part of app 
    # but make it smaller (multi-part build), just use runtime image 
    # and finalize build using that to package app without sdk in image)
FROM mcr.microsoft.com/dotnet/aspnet:7.0


# Set working directory
WORKDIR /app
# Copy output from build
COPY --from=build-env /app/out .
# Sets what we want to run when the image is run
ENTRYPOINT ["dotnet", "CommandsService.dll"]