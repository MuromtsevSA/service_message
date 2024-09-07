
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
Expose 5000
EXPOSE 32782

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["service_message/service_message.csproj", "service_message/"]
RUN dotnet restore "service_message/service_message.csproj"
COPY . .
WORKDIR "/src/service_message"
RUN dotnet build "service_message.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "service_message.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "service_message.dll"]
