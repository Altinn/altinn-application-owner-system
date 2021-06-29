# Application Owner System
A reference implementation of a system for app owners, that can react to events, fetch data and update app status.



## Architecture

The system is implemented as Azure Functions using .Net 5

It uses Azure Blob Storage to store data.


### Events Receiver

The events receiver is a webhook that receives events from Altinn Events.

It will put the received Event in a Queue based on Azure Queue Storage


### Events Processor

Events processor is responsible to

- Download Instance document
- Download all data

Put event on the confirmation queue

### Events Confirmation


## Authetication

The system uses Maskinporten to authenticate the application owner and 

## Configuration