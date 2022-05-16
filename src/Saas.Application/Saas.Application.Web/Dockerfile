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
WORKDIR /app

# Copy all csproj files as separate layers
COPY Saas.Application/Saas.Application.Web/*.csproj ./Saas.Application/Saas.Application.Web/

# Restore packages for each project
WORKDIR /app/Saas.Application/Saas.Application.Web/
RUN dotnet restore

# Copy everything else and build
WORKDIR /app

COPY Saas.Application/Saas.Application.Web/ ./Saas.Application/Saas.Application.Web/

WORKDIR /app/Saas.Application/Saas.Application.Web/

RUN dotnet publish -c Release -o out

##################################################################
# Stage 2: Copy Into Base Runtime image
##################################################################
FROM base as final

COPY --from=build /app/Saas.Application/Saas.Application.Web/out .

ENTRYPOINT ["dotnet", "Saas.Application.Web.dll"]