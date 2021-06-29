# Application Owner System
A reference implementation of a system for app owners, that can react to events, fetch data and update app status.



## Architecture

The system is implemented as Azure Functions using .Net 5

It uses Azure Blob Storage to store data.


### Events Receiver

The events receiver is a webhook that receives events from Altinn Events.

It will put the received Event in a Queue based on Azure Queue Storage.

The function is protected by a Azure Function code. This need to be present in URI in requests from Altinn events

The endpoint for subscription need to include it. 
Example: https://aos-ttdt22-function.azurewebsites.net/api/eventsreceiver?code=ffQqMrbvLoNEiySae0EfApmost8LfBeqdYY/AXa13KSyf8Rjsp1U9w==

Azure creates the code when setting up the functions.

### Events Processor

Events processor is responsible to

- Download Instance document
- Download all data and store to Azure Storage Account

Put event on the confirmation queue

### Events Confirmation

- Calls App to confirm that data is downloaded

## Authentication

The system uses Maskinporten to authenticate the application owner

## Configuration

As part of this project you find a PowerShell script to deploy

### Prerequistes
- Org is registred with a client in maskinporten and you have the clientId
- You have the certificate for that client with password
- The client is an application owner in Altinn

From deployment folder run in Powershell. Replace values matching your environment
Example: #  .\provision_application_owner_system.ps1 -subscription Altinn-TTD-Application-Owner-System -aosEnvironment [INSERT NAME ON ENVIRONMENT MAX 5 letters] -maskinportenclient [INSERT MASKINPORTEN CLIENTID] -maskinportenclientcert [PATH TO CERT] -maskinportenclientcertpwd [INSERT PASSOWORD FOR CERT] -maskinportenuri https://ver2.maskinporten.no -platformuri https://platform.tt02.altinn.no/ -appsuri https://ttd.apps.tt02.altinn.no/
