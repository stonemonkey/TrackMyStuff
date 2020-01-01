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
1. Open a console and change dir to ```/src```
2. Run ```..\scripts\set-env.ps1```
3. Run ```docker-compose up -d```

If everything went ok:
- The API is accessible at http://localhost:5000/devicestatus/1234
- The RabbitMq message queue is accessible at http://localhost:5000
