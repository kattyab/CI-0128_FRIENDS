# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
# Bind to the PORT environment variable provided by Render
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 10000
ENTRYPOINT ["dotnet", "Kaizen.Server.dll"]