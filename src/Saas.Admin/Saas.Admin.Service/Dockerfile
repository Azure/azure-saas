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
COPY Saas.Authorization/Saas.AspNetCore.Authorization/*.csproj ./Saas.Authorization/Saas.AspNetCore.Authorization/
COPY Saas.Admin/Saas.Admin.Service/*.csproj ./Saas.Admin/Saas.Admin.Service/

# Restore packages for each project
WORKDIR /app/Saas.Authorization/Saas.AspNetCore.Authorization/
RUN dotnet restore
WORKDIR /app/Saas.Admin/Saas.Admin.Service/
RUN dotnet restore

# Copy everything else and build
WORKDIR /app

COPY Saas.Authorization/Saas.AspNetCore.Authorization/ ./Saas.Authorization/Saas.AspNetCore.Authorization/
COPY Saas.Admin/Saas.Admin.Service/ ./Saas.Admin/Saas.Admin.Service/

WORKDIR /app/Saas.Admin/Saas.Admin.Service/

RUN dotnet publish -c Release -o out

##################################################################
# Stage 2: Copy Into Base Runtime image
##################################################################
FROM base as final

COPY --from=build /app/Saas.Admin/Saas.Admin.Service/out .

ENTRYPOINT ["dotnet", "Saas.Admin.Service.dll"]