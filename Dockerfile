FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /TelegramBot
RUN dotnet new console
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /TelegramBot
COPY --from=build /TelegramBot/out .
ENTRYPOINT ["dotnet", "app.dll"]