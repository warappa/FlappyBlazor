FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY ./FlappyBlazor/Client/*.csproj ./FlappyBlazor/Client/
COPY ./FlappyBlazor/Server/*.csproj ./FlappyBlazor/Server/
COPY ./FlappyBlazor/Shared/*.csproj ./FlappyBlazor/Shared/
COPY ./FlappyBlazor.sln ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish FlappyBlazor/Server/FlappyBlazor.Server.csproj -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FlappyBlazor.Server.dll"]
