# المرحلة الأساسية لتشغيل التطبيق
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# المرحلة الثانية لبناء التطبيق
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY . .


RUN dotnet restore "RentConnect.Presentation.UI.csproj"


RUN dotnet publish "RentConnect.Presentation.UI.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .


ENTRYPOINT ["dotnet", "RentConnect.Presentation.UI.dll"]
