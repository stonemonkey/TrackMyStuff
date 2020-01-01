FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# copy csproj and restore
COPY TrackMyStuff.ApiGateway/TrackMyStuff.ApiGateway.csproj ./TrackMyStuff.ApiGateway/
COPY TrackMyStuff.Common/TrackMyStuff.Common.csproj ./TrackMyStuff.Common/
COPY TrackMyStuff.RabbitMq/TrackMyStuff.RabbitMq.csproj ./TrackMyStuff.RabbitMq/

RUN dotnet restore ./TrackMyStuff.ApiGateway/

# copy everything else and build the api
COPY . .
WORKDIR "./TrackMyStuff.ApiGateway"
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /src/TrackMyStuff.ApiGateway/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "TrackMyStuff.ApiGateway.dll"]
