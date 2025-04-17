FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fetch.ReceiptProcessor/Fetch.ReceiptProcessor.csproj", "Fetch.ReceiptProcessor/"]
RUN dotnet restore "Fetch.ReceiptProcessor/Fetch.ReceiptProcessor.csproj"
COPY . .
WORKDIR "/src/Fetch.ReceiptProcessor"
RUN dotnet build "Fetch.ReceiptProcessor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Fetch.ReceiptProcessor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fetch.ReceiptProcessor.dll"]
