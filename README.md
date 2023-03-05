# Kaspersky Security Center integration (Unity)
Using the Kaspersky Security Center integration on the Unity engine.

This project was developed as part of a study assignment.

Development periods:
+ April 2022 (~10 days)
+ October - November 2022 (~20 days)

User interface in Russian.

# Short description

This application allows you to create objects that can be named, moved and connected to each other.

Types of objects:
+ PC
+ Camera
+ Landline phone
+ Switch

The main feature of the project is support for integration with Kaspersky Security Center. You can connect to administration servers and get information about agents. Received agents can be associated with PC-type objects. Thanks to this, you will be able to get a visual representation of the agents.

There was a 3D model of some building with rooms in the scene. It has been removed for security reasons.

For unknown reasons, Unity doesn't support COM objects well (I've been getting crashes while creating some COM objects). In this regard, I created a separate application on the .NET platform - COM objects provider. Both applications communicate using named pipes.

The project is presented for informational purposes only. If you would like to use the material from this project in any way, please contact me.

All rights reserved.