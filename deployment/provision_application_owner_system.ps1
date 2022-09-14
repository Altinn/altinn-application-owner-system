### PREREQUISITS
# AZ CLI
# HELM 3
# AZ Login
# azure-functions-core-tools https://www.npmjs.com/package/azure-functions-core-tools
# az extension add -n application-insights #Add support for Application Insight for AzureCLI
#

### Example usage for TTD in TT02
#  .\provision_application_owner_system.ps1 -subscription Altinn-TTD-Application-Owner-System -aosEnvironment [INSERT NAME ON ENVIRONMENT MAX 5 letters] -maskinportenclient [INSERT MASKINPORTEN CLIENTID] -maskinportenclientcert [PATH TO CERT] -maskinportenclientcertpwd [INSERT PASSOWORD FOR CERT] -maskinportenuri https://ver2.maskinporten.no -platformuri https://platform.tt02.altinn.no/ -appsuri https://ttd.apps.tt02.altinn.no/


param (
  [Parameter(Mandatory=$True)][string]$subscription,
  [Parameter(Mandatory=$True)][string]$aosEnvironment,
  [Parameter(Mandatory=$True)][string]$maskinportenclient,
  [Parameter(Mandatory=$True)][string]$maskinportenclientcert,
  [Parameter(Mandatory=$True)][string]$maskinportenclientcertpwd,
  [Parameter(Mandatory=$True)][string]$maskinportenuri,
  [Parameter(Mandatory=$True)][string]$appsuri,
  [Parameter(Mandatory=$True)][string]$platformuri,
  [string]$location = "norwayeast",
  [string]$resourcePrefix = "aos"
)

### Set subscription
az account set --subscription $subscription

$aosResourceGroupName = "$resourcePrefix-$aosEnvironment-rg"
$keyvaultname = "$resourcePrefix-$aosEnvironment-keyvault"
$storageAccountName = $resourcePrefix+$aosEnvironment+"storage"
$functionName = "$resourcePrefix-$aosEnvironment-function"


#### Check if resource group for AKS exists
$resourceGroupExists = az group exists --name $aosResourceGroupName
if (![System.Convert]::ToBoolean($resourceGroupExists)) {
  az group create --location $location --name $aosResourceGroupName
}

Write-Output "Create Storage Account $storageAccountName"
$storageAccount = az storage account create -n $storageAccountName -g $aosResourceGroupName -l $location --sku Standard_GRS --kind StorageV2 --access-tier Hot
$StorageID = az storage account show --name $storageAccountName --resource-group $aosResourceGroupName --query id --output tsv
$storageAccountAccountKey = az storage account keys list --account-name $storageAccountName --query [0].value -o tsv
az storage container create -n inbound --account-name $storageAccountName --account-key $storageAccountAccountKey --public-access off
az storage container create -n active-subscriptions --account-name $storageAccountName --account-key $storageAccountAccountKey --public-access off
az storage container create -n add-subscriptions --account-name $storageAccountName --account-key $storageAccountAccountKey --public-access off
az storage container create -n remove-subscriptions --account-name $storageAccountName --account-key $storageAccountAccountKey --public-access off
$storageConnectionString = az storage account show-connection-string -g $aosResourceGroupName -n $storageAccountName --query connectionString --output tsv
$blobendpoint = az storage account show --name $storageAccountName --resource-group $aosResourceGroupName --query primaryEndpoints.blob --output tsv


Write-Output "Create KeyVault"
$keyvault = az keyvault create --location $location --name $keyvaultname --resource-group $aosResourceGroupName
$KeyvaultID = az keyvault show --name $keyvaultname --resource-group $aosResourceGroupName --query id --output tsv
$vaultUri = az keyvault show --name $keyvaultname --resource-group $aosResourceGroupName --query properties.vaultUri --output tsv


Write-Output "Import Maskinporten cert"
az keyvault certificate import --vault-name $keyvaultname -n maskinportenclientcert -f $maskinportenclientcert --password $maskinportenclientcertpwd

Write-Output "Create Function App"
az functionapp create --resource-group $aosResourceGroupName --consumption-plan-location $location --runtime dotnet --functions-version 4 --name $functionName --storage-account $storageAccountName
az functionapp identity assign -g $aosResourceGroupName -n $functionName
$funcprincialId = az functionapp identity show --name $functionName --resource-group $aosResourceGroupName --query principalId  --output tsv

Write-Output "Set policy"
az keyvault set-policy --name $keyvaultname --object-id $funcprincialId --secret-permissions get --certificate-permissions list

Write-Output "Set config"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "AltinnApplicationOwnerSystemSettings:AccountKey=$storageAccountAccountKey"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "AltinnApplicationOwnerSystemSettings:AccountName=$storageAccountName"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "AltinnApplicationOwnerSystemSettings:AppsBaseUrl=$appsuri"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "AltinnApplicationOwnerSystemSettings:BlobEndpoint=$blobendpoint"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "QueueStorageSettings:ConnectionString=$storageConnectionString"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "AltinnApplicationOwnerSystemSettings:PlatformBaseUrl=$platformuri"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "AltinnApplicationOwnerSystemSettings:MaskinportenBaseAddress=$maskinportenuri"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "AltinnApplicationOwnerSystemSettings:MaskinPortenClientId=$maskinportenclient"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "AltinnApplicationOwnerSystemSettings:TestMode=true"
az functionapp config appsettings set --name $functionname --resource-group $aosResourceGroupName --settings "KeyVault:KeyVaultURI=$vaultUri"


Write-Output "Publish app"
Set-Location ..\src\Altinn-application-owner-system\Functions
func azure functionapp publish $functionName --force
Set-Location ..\..\..\deployment