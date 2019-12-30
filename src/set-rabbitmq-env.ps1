param (
	[ValidateNotNullOrEmpty()]
	#[string]$host = "192.168.0.1",		# db consumer runs inside container, 
    [string]$hostName = "localhost",	# db consumer runs localy (debug)

	[ValidateNotNullOrEmpty()]
	[string]$port = "5672",

	[ValidateNotNullOrEmpty()]
	[string]$mgmtPort = "15672",
	
	[ValidateNotNullOrEmpty()]
	[string]$user = "white",

	[ValidateNotNullOrEmpty()]
	[string]$password = "Password456!",

	[switch]$rm = $false
)

$rabbitMqHost = "";
$rabbitMqPort = "";
$rabbitMqMgmtPort = "";
$rabbitMqUser= "";
$rabbitMqPassword= "";

if (-not($rm)) {
	$rabbitMqHost = $hostName;
	$rabbitMqPort = $port;
	$rabbitMqMgmtPort = $mgmtPort;
	$rabbitMqUser = $user;
	$rabbitMqPassword = $password;
}
# docker-compose variables
$env:RABBITMQ_PORT = $rabbitMqPort;
$env:RABBITMQ_MGMT_PORT = $rabbitMqMgmtPort;
$env:RABBITMQ_DEFAULT_USER = $rabbitMqUser;
$env:RABBITMQ_DEFAULT_PASS = $rabbitMqPassword;
# application variables
$env:RabbitMq__Hostnames__0 = $rabbitMqHost;
$env:RabbitMq__Port = $rabbitMqPort;
$env:RabbitMq__Username = $rabbitMqUser;
$env:RabbitMq__Password = $rabbitMqPassword;
