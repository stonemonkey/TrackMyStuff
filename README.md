# TrackMyStuff
Experimental API for IoT device tracking based on microservices arhitecture, NET Core 3.1 and Docker containers with:
- [Rabbit MQ](https://www.rabbitmq.com/) (message broker)
- [MariaDB](https://mariadb.org/) (as a MySQL data storage)
- [Serilog](https://serilog.net/) & [Seq](https://datalust.co/seq/) (for structured centralized logging)

## Getting started
The simplest way to spin up and run everything is to use docker-compose CLI. When developing is also usefull to be able to debug a service while all the rest of the infrastructure is up and runing.

### Launching with docker-compose
Prerequisites:
- Clone this repository
- [Install docker-compose](https://docs.docker.com/compose/install/)
- [Install PowerShell Core 6](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-6)

Steps:
1. Open PowerShell console and change dir to `/src` relative to the repository root
2. Run `..\scripts\set-env.ps1`
3. Run `docker-compose up -d`

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

 
