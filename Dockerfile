# Use official .NET SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore
COPY Singer/Singer/*.csproj Singer/Singer/
RUN dotnet restore Singer/Singer/Singer.csproj

# Copy everything else
COPY . .
WORKDIR /src/Singer/Singer
RUN dotnet publish -c Release -o /app/out

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Singer.dll"]
