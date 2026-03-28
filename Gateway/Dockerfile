# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Sao chép file csproj và restore dependencies
COPY ["Gateway/Gateway.csproj", "Gateway/"]
RUN dotnet restore "Gateway/Gateway.csproj"

# Sao chép toàn bộ mã nguồn và build ứng dụng
COPY . .
WORKDIR "/src/Gateway"
RUN dotnet build "Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5270
ENV ASPNETCORE_URLS=http://+:5270
ENTRYPOINT ["dotnet", "Gateway.dll"]
