##################################################################
# Shared Arguments
##################################################################
ARG app_name=Saas.Permissions.Service
ARG app_path=Saas.Permissions/Saas.Permissions.Service

##################################################################
# Stage 0: Set the Base Primary Image of the App
##################################################################
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app

EXPOSE 80
EXPOSE 443

##################################################################
# Stage 1: Set the Base Development Image of the App
##################################################################
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG app_name 
ARG app_path
WORKDIR /app

# Copy all csproj files as separate layers
COPY $app_path/*.csproj ./$app_path/

# Restore packages for each project
WORKDIR /app/$app_path/
RUN dotnet restore

# Copy everything else and build
WORKDIR /app

COPY ./$app_path ./$app_path/
WORKDIR /app/$app_path/

RUN dotnet publish -c Release -o out

##################################################################
# Stage 2: Copy Into Base Runtime image
##################################################################
FROM base as final
ARG app_name
ENV dll_name="${app_name}.dll"
ARG app_path

COPY --from=build /app/$app_path/out .

EXPOSE 443
ENTRYPOINT ["/bin/sh", "-c", "dotnet $dll_name"]