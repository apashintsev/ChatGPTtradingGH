#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatGPTtrading.API/ChatGPTtrading.API.csproj", "ChatGPTtrading.API/"]
COPY ["ChatGPT.Application/ChatGPT.Application.csproj", "ChatGPT.Application/"]
COPY ["ChatGPTtrading.Domain/ChatGPTtrading.Domain.csproj", "ChatGPTtrading.Domain/"]
COPY ["ChatGPTtrading.Infrastructure/ChatGPTtrading.Infrastructure.csproj", "ChatGPTtrading.Infrastructure/"]
RUN dotnet restore "ChatGPTtrading.API/ChatGPTtrading.API.csproj"
COPY . .
WORKDIR "/src/ChatGPTtrading.API"
RUN dotnet build "ChatGPTtrading.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatGPTtrading.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatGPTtrading.API.dll"]