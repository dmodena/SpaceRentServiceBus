This is a .Net 8 solution with an example implementation of the Publish/Subscriber pattern.

For this application, the solution was designed using Azure Service Bus topics, but there is another version implemented purely in C#, which can be found [here](https://github.com/dmodena/SpaceRentPubSub).

My intent was to mimic a possible workflow for private property renting, with the following steps:
1. A user (tenant) books a stay for a particular property
    - Granted that all information was already validated, and the rent paid, the system receives a BookingRequest through an API
2. The payload received is enriched with information from the owner and the tenant
    - I simulate retrieving data from the database to create a new object, Booking
3. The new object is sent to a topic in Azure Service Bus
    - This provides several benefits due to its asynchronous nature, reliability and flexibility
4. Two separate services are subscribed and react to this event
    1. The first is called DateBlocker, and it simulates blocking the dates for that particular property, to prevent other users from trying to book the same dates
    2. The second is called OwnerNotifier, and it mimics a service that could send an email to notify the owner of the property about the rental

The solution is divided into separate projects, for the API, console applications, and class libraries.

Author: Douglas Modena  
License: MIT  
2024
