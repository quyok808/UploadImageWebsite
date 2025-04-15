# Sử dụng image SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy file .csproj và restore
COPY *.csproj ./
RUN dotnet restore

# Copy mã nguồn và build
COPY . ./
RUN dotnet publish -c Release -o out

# Sử dụng image runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Mở cổng
EXPOSE 8080

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "UploadImageWebsite.dll"]