# push the image to ACR
$rgName="RG-Shared"
$acrName="acrzl"

# push the image to ACR, Queues a quick build
# --image: The name and tag of the image using the format: '-t repo/image:tag'. Multiple tags are supported by passing -t multiple times.
# --file: The relative path of the the docker file to the source code root folder. Default to 'Dockerfile'.
az acr build --file MS-Updates\Dockerfile `
             --image ms-updates-api:first `
             --registry $acrName .
