FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS prep
RUN apk update && apk add clang build-base zlib-dev

FROM prep AS build
ARG BUILD_CONFIGURATION=Release
COPY ["Kontroller.API.csproj", "src/Kontroller.API/"]
RUN dotnet restore "src/Kontroller.API/Kontroller.API.csproj"
COPY [".", "/src/Kontroller.API/"]
RUN dotnet build "/src/Kontroller.API/Kontroller.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "/src/Kontroller.API/Kontroller.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
COPY --from=publish /app/publish .
RUN pwd
RUN ls -la
ENTRYPOINT ["./Kontroller.API"]
