param (
	[ValidateNotNullOrEmpty()]
    [string]$server = "localhost",

	[ValidateNotNullOrEmpty()]
	[string]$port = "5340",

	[switch]$rm = $false
)

$seqServerUrl = "";
$seqPort = "";

if (-not($rm)) {
	$seqServerUrl = "http://"+$server+":"+$port;
	$seqPort = $port;
}
# docker-compose variables
$env:SEQ_PORT = $seqPort;
# application variables
$env:Serilog__SeqServerUrl = $seqServerUrl;
