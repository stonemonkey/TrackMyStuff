# TrackMyStuff
Experimental API for IoT device tracking based on microservices arhitecture, NET Core 3.1 and Docker containers with:
- [Rabbit MQ](https://www.rabbitmq.com/) (message broker)
- [MariaDB](https://mariadb.org/) (as a MySQL data storage)
- [Serilog](https://serilog.net/) & [Seq](https://datalust.co/seq/) (for structured centralized logging)

## Getting started
The simplest way to spin up and run everything is to use docker-compose CLI. When developing is also usefull to be able to debug a service while all the rest of the infrastructure is up and runing.

### Launching in docker-compose
Prerequisites:
- Clone this repository
- [Install docker-compose](https://docs.docker.com/compose/install/)
- [Install PowerShell Core 6](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-6)

Steps:
1. Open PowerShell console and change dir to `\src` relative to the repository root
2. Run `..\scripts\set-env.ps1`. This will set all the environment variables used to configure the the containers and the services. See `\src\docker-compose.yml`.
3. Run `docker-compose up -d`. On the first run this will download base images for NET Core 3.1, RabbitMQ, MariaDB and Seq containers.

Once the containers are up and running,
```
Starting src_rabbitmq_1  ... done
Starting src_mysqldata_1 ... done
Starting src_seq_1       ... done
Starting src_apigw_1     ... done
Starting src_devsrv_1    ... done
```
You should be able to access any of the services at the following URLs, from your dev machine:
- The ApiGateway (src_apigw_1) is accessible at http://localhost:5000 and the following endpoints are available:
  * GET /devicestatus/_deviceId_ for reading the status of a device
  * POST /heartbeat for adding devices heartbeat
  * GET /hc for reading healthy check result of the service
  * GET /liveness for reading the availability of the service
- The DevicesService (src_devsrv_1) is not directly accessible
- Infrastructure
  * MariaDB Server (src_mysqldata_1): connect using favorite MySQL client application to `localhost:5306` with `Uid=root;Pwd=Password123!`
  * RabbitMQ (src_rabbitmq_1): http://10.0.75.1:15672/ (login with username=guest, password=guest)
  * Seq (src_seq_1): http://10.0.75.1:5340

Run `docker-compose down` in `\src` dir to stop and remove the containers. This is going to preserve database. To remove docker volume used by the database run `docker-compose down --volume`. 

### Debugging in Visual Studio Code
Prerequisites:
- [Install Visual Studio Code](https://code.visualstudio.com/download)

Steps:
1. Open PowerShell console and change dir to `\src` relative to the repository root
2. Run `..\scripts\set-env.ps1`
3. Run `docker-compose up -d src_mysqldata_1 src_rabbitmq_1 src_seq_1 src_devsrv_1`. Exclude the service you want to debug from the list. In this case `src_apigw_1` is going to be debugged.
4. Change dir to `\TrackMyStuff.ApiGateway`
5. Run `dotnet build`
6. Open repository root dir in VSCode
7. Put a breakpoint in `\src\TrackMyStuff.ApiGateway\Controllers\DeviceStatusController.cs`, `Get` method
8. Debug and Run `ApiGateway`. See `\.vscode\launch.json` for the debugger configuration.
9. Open `http://localhost:5000/devicestatus/1234` in browser. The breakpoint should be hit.

The configuration values for the service under debug is taken now from the `appsettings.Development.json`.
