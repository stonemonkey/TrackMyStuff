FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore
COPY *.csproj ./api/
RUN dotnet restore ./api/

# copy everything else and build the api
COPY . ./api/
WORKDIR /app/api
RUN dotnet publish -c Release -o out

# restore tools and generate db migrations
RUN dotnet tool restore
ARG ConnectionStrings__DefaultConnection
RUN dotnet ef database update

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/api/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "TrackMyStuff.Api.dll"]
