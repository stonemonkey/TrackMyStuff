# Setup Azure
 
az group create -n TrackMyStuff -l northeurope --subscription "Visual Studio Professional"

az acr create -n TrackMyStuffRegistry -g TrackMyStuff --subscription "Visual Studio Professional" --sku Basic --admin-enabled true

az appservice plan create -n TrackMyStuffPlan -l northeurope -g TrackMyStuff --subscription "Visual Studio Professional" --sku B1 --is-linux

docker build -t trackmystuffapi-img ./api

az acr credential show -n TrackMyStuffRegistry --subscription "Visual Studio Professional"

docker login trackmystuffregistry.azurecr.io -u TrackMyStuffRegistry -p "uT1n+3g/UrLnnmQulh8eiUuSh25b2dUQ"

docker tag trackmystuffapi-img trackmystuffregistry.azurecr.io/stonemonkey/trackmystuff:initial

docker push trackmystuffregistry.azurecr.io/stonemonkey/trackmystuff:initial

az webapp create -n TrackMyStuffApi -g TrackMyStuff -p TrackMyStuffPlan --subscription "Visual Studio Professional" -i trackmystuffregistry.azurecr.io/stonemonkey/trackmystuff:initial

az webapp config container set -n TrackMyStuffApi -g TrackMyStuff --subscription "Visual Studio Professional" -i trackmystuffregistry.azurecr.io/stonemonkey/trackmystuff:initial -r https://trackmystuffregistry.azurecr.io -u TrackMyStuffRegistry -p <password> 

az webapp log config -n TrackMyStuffApi -g TrackMyStuff --subscription "Visual Studio Professional" --web-server-logging filesystem
