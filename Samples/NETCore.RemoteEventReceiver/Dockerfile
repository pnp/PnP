# syntax=docker/dockerfile:1

ARG PROJECT_NAME=NETCore.RemoteEventReceiver

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
ARG PROJECT_NAME
WORKDIR /app

# Copy sln and csproj and restore as distinct layers
COPY ./*.sln ./
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*} && mv $file ${file%.*}; done
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ${PROJECT_NAME}/${PROJECT_NAME}.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
ARG PROJECT_NAME
ENV STARTUP_PROJECT_NAME=$PROJECT_NAME
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT dotnet $STARTUP_PROJECT_NAME.dll
