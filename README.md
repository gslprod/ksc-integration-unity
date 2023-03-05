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

For unknown reasons, Unity doesn't support COM objects well (I've been getting crashes while creating some COM objects). In this regard, I created a separate application on the .NET platform - [COM objects provider](https://github.com/gslprod/ksc-com-objects-provider). Both applications communicate using named pipes.

The project is presented for informational purposes only. If you would like to use the material from this project in any way, please contact me.

All rights reserved.

# Screenshots

![Screenshot 1](https://user-images.githubusercontent.com/122805276/222965165-a311f2d3-9f57-4977-879b-460b7229f6b5.png)

![Screenshot 2](https://user-images.githubusercontent.com/122805276/222965184-d235a560-6a9b-45f4-bddc-01000a5b91a8.png)

![Screenshot 3](https://user-images.githubusercontent.com/122805276/222965195-5acae0ad-72ff-43ac-90cb-61aa0f4b8928.png)

![Screenshot 4](https://user-images.githubusercontent.com/122805276/222965217-de21233c-a145-45e9-b3f0-72802710e1d3.png)

![Screenshot 5](https://user-images.githubusercontent.com/122805276/222965228-9e422dc0-adc2-405c-87a5-fea3e39a0568.png)

![Screenshot 6](https://user-images.githubusercontent.com/122805276/222965232-01402b53-cba3-497e-9d6a-bdb45d7f701a.png)
