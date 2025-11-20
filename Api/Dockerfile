# Stage 1: Base - Dùng cho môi trường runtime cuối cùng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 42069
EXPOSE 42070

# Stage 2: Build - Biên dịch và build toàn bộ solution
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Sao chép file solution (.sln) và tất cả các file project (.csproj) trước tiên
# SỬA LẠI TÊN FILE .SLN CHO ĐÚNG VỚI DỰ ÁN CỦA BẠN
COPY ["OnlineWebAndService.sln", "."]
COPY ["Api/Api.csproj", "Api/"]
COPY ["DataAccess.EFCore/DataAccess.EFCore.csproj", "DataAccess.EFCore/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Tools/Tools.csproj", "Tools/"]
# Nếu bạn có project Domain riêng, hãy thêm dòng COPY cho nó ở đây

# Chạy restore trên file solution. Đây là cách làm đúng.
# Nó sẽ đọc "bản đồ" và restore cho tất cả các project.
RUN dotnet restore "OnlineWebAndService.sln"

# Sao chép toàn bộ source code còn lại
COPY . .

# Build toàn bộ solution
# WORKDIR không cần thiết ở đây, vì build từ solution sẽ tự tìm các project
RUN dotnet build "OnlineWebAndService.sln" -c $BUILD_CONFIGURATION --no-restore

# Stage 3: Publish - Chỉ publish project Api
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Chỉ định rõ project cần publish
RUN dotnet publish "Api/Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-build

# Stage 4: Final - Tạo image cuối cùng, gọn nhẹ để chạy
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]