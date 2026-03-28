# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Sao chép file csproj và restore dependencies
COPY ["Auth/dotnet-auth.csproj", "Auth/"]
RUN dotnet restore "Auth/dotnet-auth.csproj"

# Sao chép toàn bộ mã nguồn và build ứng dụng
COPY . .
WORKDIR "/src/Auth"
RUN dotnet build "dotnet-auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "dotnet-auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5263
ENV ASPNETCORE_URLS=http://+:5263
ENTRYPOINT ["dotnet", "dotnet-auth.dll"]
