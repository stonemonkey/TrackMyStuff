param (
	[ValidateNotNullOrEmpty()]
	[string]$mySqlPort = $env:MYSQL_PORT,
	[ValidateNotNullOrEmpty()]
	[string]$mySqlRootPassword = $env:MYSQL_ROOT_PASSWORD,
	[ValidateNotNullOrEmpty()]
	[string]$defaultConnection = $env:ConnectionStrings__ApiDbConnection,
	
	[switch]$down = $false,
	[switch]$up = $false,
		
    [switch]$rmApi = $false,
    [switch]$rmDb = $false,
    [switch]$rmNet = $false,
    [switch]$rmVol = $false,

    [switch]$createVol = $false,
    [switch]$createNet = $false,
    [switch]$runDb = $false,
    [switch]$buildApi = $false,
	[switch]$runApi = $false
)

write-host "TrackMyStuff API Service utility (c) 2019 by cosmo";

if ($down) {
	write-host "Running DOWN script";
    $rmApi = $true;
    $rmDb = $true;
    $rmNet = $true;
    
	# erases db storage from docker volume
	#$rmVol = $true; # better doing it manually
}
if ($up) {
	write-host "Running UP script";
    $createVol = $true;
    $createNet = $true;
    $runDb = $true;
    $buildApi = $true;
	$runApi = $true;
}


if ($rmApi) {
	write-host "`r`nRemoving API ...";
	docker stop trackmystuff_api_1;
	docker rm trackmystuff_api_1;
	docker rmi trackmystuff_api-img;
}
if ($rmDb) {
	write-host "`r`nRemoving database ...";
	docker stop trackmystuff_api_db_1;
	docker rm trackmystuff_api_db_1;
}
if ($rmNet) {
	write-host "`r`nRemoving network ...";
	docker network rm trackmystuff_api-net;
}
if ($rmVol) {
	write-host "`r`nRemoving volume ...";
	docker volume rm trackmystuff_api-vol;
}
if ($down) {
	$intermediates = docker images --filter "dangling=true" -q --no-trunc;
	if($intermediates) {
		write-host "`r`nRemoving all intermediate disconnected images and stopped containers ...";
		docker container prune;
		docker rmi $intermediates;
	}
}

if (-not($mySqlPort) -and $runDb) {
	throw "Invalid value for -mySqlPort parameter (probably missing env:MYSQL_PORT)!";
}
if (-not($mySqlRootPassword) -and $runDb) {
	throw "Invalid value for -mySqlRootPassword parameter (probably missing env:MYSQL_ROOT_PASSWORD)!";
}
if (-not($defaultConnection) -and ($buildApi -or $runApi)) {
	throw "Invalid value for -defaultConnection parameter (probably missing env:ConnectionStrings__DefaultConnection)!";
}


if ($createVol -or $runDb) {
	write-host "`r`nCreating the volume to persist db files localy in docker ...";
	docker volume create trackmystuff_api-vol;
}
if ($createNet) {
	write-host "`r`nCreating the network to connect the api and db containers";
	docker network create -d bridge --subnet 192.168.0.0/24 --gateway 192.168.0.1 trackmystuff_api-net;
}
if ($runDb) {
	write-host "`r`nStarting database ...";
	# binding -v "${PWD}/api/db:/var/lib/mysql" to local folder was NOT WORKING at the time
	$ports = $mySqlPort + ":3306"
	docker run -d -p $ports -v trackmystuff_api-vol:/var/lib/mysql -e MYSQL_ROOT_PASSWORD="$mySqlRootPassword" --name trackmystuff_api_db_1 mariadb;
}
if ($buildApi) {
	write-host "`r`nBuilding API container and running the database migrations ...";
	docker build --build-arg ConnectionStrings__DefaultConnection="$defaultConnection" -t trackmystuff_api-img ./api;
}
if ($runApi) {
	write-host "`r`nStarting the API ...";
	docker run -d -p 5000:80 -e ConnectionStrings__DefaultConnection="$defaultConnection" --name trackmystuff_api_1 trackmystuff_api-img;
}