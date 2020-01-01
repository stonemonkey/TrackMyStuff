param (
	[ValidateNotNullOrEmpty()]
	[string]$server = "mysqldata",		# service name defined in docker-compose.yml

	[ValidateNotNullOrEmpty()]
	[string]$port = "5306",				# port exposed for local consumers (debug) and management access
	
	[ValidateNotNullOrEmpty()]
	[string]$user = "root",

	[ValidateNotNullOrEmpty()]
	[string]$password = "Password123!",

	[switch]$rm = $false
)

$mySqlPort = "";
$mySqlPassword= "";
$apiConnectionString = "";
$devConnectionString = "";
if (-not($rm)) {
	$mySqlPort = $port;
	$mySqlPassword = $password;
	$apiConnectionString = "Server=$server;Port=3306;Database=trackmystuffapidb;Uid=$user;Pwd=$mySqlPassword;";
	$devConnectionString = "Server=$server;Port=3306;Database=trackmystuffdevdb;Uid=$user;Pwd=$mySqlPassword;";
}
# docker-compose variables
$env:MYSQL_PORT = $mySqlPort;
$env:MYSQL_ROOT_PASSWORD = $mySqlPassword;
$env:API_CONNECTION_STRING = $apiConnectionString;
$env:DEV_CONNECTION_STRING = $devConnectionString;
