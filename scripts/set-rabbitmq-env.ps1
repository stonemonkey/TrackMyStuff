param (
	[ValidateNotNullOrEmpty()]
	[string]$hostName = "rabbitMq",		# service name defined in docker-compose.yml

	[ValidateNotNullOrEmpty()]
	[string]$port = "5672",				# port exposed for local consumers (debug)

	[ValidateNotNullOrEmpty()]
	[string]$mgmtPort = "15672",		# port exposed for frontend access
	
	[ValidateNotNullOrEmpty()]
	[string]$user = "guest",

	[ValidateNotNullOrEmpty()]
	[string]$password = "guest",

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
$env:RABBITMQ_HOST = $rabbitMqHost;
$env:RABBITMQ_PORT = $rabbitMqPort;
$env:RABBITMQ_MGMT_PORT = $rabbitMqMgmtPort;
$env:RABBITMQ_DEFAULT_USER = $rabbitMqUser;
$env:RABBITMQ_DEFAULT_PASS = $rabbitMqPassword;

