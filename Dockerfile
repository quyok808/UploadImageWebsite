# Sử dụng image cơ sở từ Microsoft .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy file .sln và các file .csproj từ thư mục con
COPY *.sln .
COPY UploadImageWebsite/*.csproj ./UploadImageWebsite/
RUN dotnet restore

# Copy toàn bộ mã nguồn còn lại và build
COPY . .
RUN dotnet publish -c Release -o out

# Sử dụng image runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Tạo thư mục Uploads
RUN mkdir -p /app/Uploads

# Copy file đã build từ bước build
COPY --from=build /app/out .

# Cấu hình cổng mà ứng dụng sẽ chạy
EXPOSE 7223

# Lệnh khởi động ứng dụng
ENTRYPOINT ["dotnet", "UploadImageWebsite.dll"]