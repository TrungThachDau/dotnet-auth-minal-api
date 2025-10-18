FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY dotnet-auth/bin/Release/net9.0/publish . 
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "dotnet-auth.dll"]
