param (
	[ValidateNotNullOrEmpty()]
	#[string]$server = "192.168.0.1",	# db consumer runs inside container, 
    [string]$server = "localhost",		# db consumer runs localy (debug)

	[ValidateNotNullOrEmpty()]
	[string]$port = "5306",
	
	[ValidateNotNullOrEmpty()]
	[string]$user = "root",

	[ValidateNotNullOrEmpty()]
	[string]$password = "Password123!",

	[ValidateNotNullOrEmpty()]
	[string]$apiDb = "trackmystuffapidb",
	
	[ValidateNotNullOrEmpty()]
	[string]$devDb = "trackmystuffdevdb",

	[switch]$rm = $false
)

$mySqlPort = "";
$mySqlPassword= "";
$apiConnectionString = "";
$devConnectionString = "";
if (-not($rm)) {
	$mySqlPort = $port;
	$mySqlPassword = $password;
	$apiConnectionString = "Server=$server;Port=$mySqlPort;Database=$apiDb;Uid=$user;Pwd=$mySqlPassword;";
	$devConnectionString = "Server=$server;Port=$mySqlPort;Database=$devDb;Uid=$user;Pwd=$mySqlPassword;";
}
# docker-compose variables
$env:MYSQL_PORT = $mySqlPort;
$env:MYSQL_ROOT_PASSWORD = $mySqlPassword;
# application variables
$env:ConnectionStrings__ApiConnection = $apiConnectionString;
$env:ConnectionStrings__DevConnection = $devConnectionString;
