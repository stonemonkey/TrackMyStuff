param (
	[ValidateNotNullOrEmpty()]
	[string]$server = "seq",			# service defined in docker-compose.yml

	[ValidateNotNullOrEmpty()]
	[string]$port = "5340",				# port exposed for local consumers (debug) and frontend access

	[switch]$rm = $false
)

$seqServerUrl = "";
$seqPort = "";

if (-not($rm)) {
	$seqServerUrl = "http://"+$server;	# uses port 80 for consumers running inside containers network
	$seqPort = $port;
}
# docker-compose variables
$env:SEQ_PORT = $seqPort;
$env:SEQ_SERVER_URL = $seqServerUrl;
