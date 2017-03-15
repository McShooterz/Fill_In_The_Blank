This is a simple client/server test for unity and python.

the server and client will both connect to a localhost at port 13000,
the client will request random vectors from the server, the client "player"
will then move to these designated vectors, when the player reaches the point
the client asks for a new vector from the server.

It's a very simple application, but can easily be built upon.

WARNING the networkmanager calls to read data to and from the server are currently blocking