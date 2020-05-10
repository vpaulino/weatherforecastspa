# Introduction 
this project consists on the presentation of several Microsoft .Net technologies and sdks on how to use them to build reactive UIs. 
This POC continues the  weatherforecast angular template from visual studio.  


# Getting Started

## Solution applications
### Application Frontend
This aspnet core project is the application that serves the angular frontend and the API to support the angular frontend. IT has an HTTP API with ApiControllers
and the SignalR.Hub<T> with a Contract that allow client application receive notification using the signalR subscription channels. 
the use cases supported by the UI are: 

1. List weather forecasts
2. List recomendations regarding the weatherforecast
3. Insert and Edit forecasts to locations
4. Notificationbar regarding actions that are happening in the applications

### Application.Azure.TemperatureFunction 
This is the azure function of this POC that support the background work of generating recomendations depending on the weather forecast submited by the users. 

#### Input binding
This input biding that trigger the execution of this azure function are messages being  published to the storage queue

### Output binding
The outputbinding of this azure function is the service bus topic.that means the result of this execution is going to be published to the service bus. 



## Configure services on azure
To have this up and running you will need to create a service bus on azure. Associate to that service bus namespace you will need to:
1. create a topic
2. create a subscription on the previous topic.
3. create a connection policy to enable only subscription and another one for publishing to that topic
5. Create the storage account on azure
5.1 Create the storage account queue. this queue is used on the demo to send a message to the azure function Application.Azure.TemperatureFunction

## Configure settings on applications

### Configure Application.Frontend

4. Add the connection string to the Application.Frontend the correct approach should be to use secrets of aspnet core, secrets will live only in your computer. to configure that data on the secrets :
4.1 Right click the aspnet core Application.Frontend csproj on solution explorer and select Manage Secrets. a json file will open add there the sections:

```
"ConnectionStrings": {

    "QueuePublisherConnectionSettings": {
      "ConnectionString": "",
      "Path": "appevents-queue"
    },

    "TopicSubscriberConnectionSettings": {
      "ConnectionString": "",
      "Path": "sdtappevents-sb-topics",
      "SubscriptionName": "sbfrontend-sdtappevents"
    }
  }
```

### Configure Azure function
Azure functions are the serverless service that azure provides. they should be used to execute background work in a stateless way. To execute background work 
maintaining state we should use Durable functions. The azure function in this sample is to simulate as it is calculating recomendations regarding weather forecast received 
and publish the result to the service bus topic in the configuration. To the Application.Frontend received the notification from the azure function they should use the same servicebus topic
Azure Functions starts to execute when the input trigger on any action is triggered. 
This azure function has one input binding to storage queues. When a message is published to que storage queue work starts with that message.
The  output binding  is the place to were the return of the function is going to be sent. In our use case is going to be published a message to  service bus.
 the configuration should live inside of a json section called values.

```
"Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AppEventsQueue": "appevents-queue",
    "ServiceBusConnection": "",
    "AppEvents-Topic": "sdtappevents-sb-topics"
  }
```




# Build and Test
Make sure you have the multiple projects start up selected with the aspnet core and the azure function. 

# Contribute
The rules to contribute to this project are the following:
1. Use Azure Services to support the frontend requirements. 
2. to submit a contribution to this project you should open a task describing what is the work that you want to do. 
3. The task should be submited and be scoped to rule 1 and 2.  