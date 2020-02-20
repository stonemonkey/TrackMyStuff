# TrackMyStuff
Experimental API for IoT device tracking based on microservices arhitecture, NET Core 3.1 and Docker containers with:
- [Rabbit MQ](https://www.rabbitmq.com/) (message broker)
- [MariaDB](https://mariadb.org/) (as a MySQL data storage)
- [Serilog](https://serilog.net/) & [Seq](https://datalust.co/seq/) (for structured centralized logging)


## Getting started
The simplest way to spin up and run everything is to use docker-compose CLI. When developing is also usefull to be able to debug a service while all the rest of the infrastructure is up and runing.

Prerequisites:
- Clone this repository
- [Install Visual Studio Code](https://code.visualstudio.com/download)
- [Install Docker for Desktop](https://www.docker.com/products/docker-desktop)
- [Install PowerShell Core](https://github.com/PowerShell/PowerShell) (6 or 7-preview for the moment)
- Add VSCode path in environment variables (if not there): Win key -> Edit the system environment variables -> Environment Variables... -> User variables for <user> -> Path -> Edit... -> New -> e.g. `C:\Users\costin.morariu\AppData\Local\Programs\Microsoft VS Code\bin`
- Add PowerShell Core in environment variables (if not there): Win key -> Edit the system environment variables -> Environment Variables... -> System variables -> Path -> Edit... -> New -> e.g. `C:\Program Files\PowerShell\7-preview\preview`


### Launching in docker-compose
1. Open PowerShell core console (Win key + `run` -> `pwsh`) and change dir to `\src` relative to the repository root
2. Run `..\scripts\set-env.ps1`. This will set all the environment variables used to configure the containers and the services. See `\src\docker-compose.yml`.
3. Run `docker-compose up -d`. The first time will pull all base images, expect to take some time.


### Debugging in Visual Studio Code
1. Open PowerShell Core console (Win key + `run` -> `pwsh`) and change dir to `\src` relative to the repository root
2. Run `..\scripts\set-env.ps1`
3. Run `docker-compose up -d`. This will pull all needed base images (first time), build service images and start all of them.
4. Run `docker stop src_apigw_1`. This is the service that's going to be debugged from VSCode. 
5. Change dir to `\TrackMyStuff.ApiGateway`
6. Run `dotnet build`
7. Open repository root dir in VSCode (`cd ..\..; code .`)
8. Put a breakpoint in `\src\TrackMyStuff.ApiGateway\Controllers\DeviceStatusController.cs`, `Get` method
9. Debug and Run `ApiGateway`. See `\.vscode\launch.json` for the debugger configuration.
10. Open `http://localhost:5000/devicestatus/1234` in browser. The breakpoint should be hit.

The configuration values for the service under debug is taken now from the `appsettings.Development.json`.


### Once everything is up and running,
You should be able to access any of the services at the following URLs, from your dev machine:
- The ApiGateway (src_apigw_1) is accessible at http://localhost:5000 and the following endpoints are available:
  * GET /devicestatus/_deviceId_ for reading the status of a device
  * POST /heartbeat for adding devices heartbeat
  * GET /hc for reading healthy check result of the service
  * GET /liveness for reading the availability of the service
- The DevicesService (src_devsrv_1) is not directly accessible
- Infrastructure
  * MariaDB Server (src_mysqldata_1): connect using favorite MySQL client application to `localhost:5306` with `Uid=root;Pwd=Password123!`
  * RabbitMQ (src_rabbitmq_1): http://localhost:15672 or http://10.0.75.1:15672 (login with username=guest, password=guest)
  * Seq (src_seq_1): http://localhost:5340 or http://10.0.75.1:5340

Run `docker-compose down` in `\src` dir to stop and remove the containers. This is going to preserve database. To remove docker volume used by the database run `docker-compose down --volume`. 
