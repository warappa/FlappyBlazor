FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY ./FlappyBlazor/Client/*.csproj ./FlappyBlazor/Client/
COPY ./FlappyBlazor/Server/*.csproj ./FlappyBlazor/Server/
COPY ./FlappyBlazor/Shared/*.csproj ./FlappyBlazor/Shared/
COPY ./FlappyBlazor.sln ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
