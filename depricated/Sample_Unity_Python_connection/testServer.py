#this is just a basic python server for the proof of concept I'm uploading,
#it makes a server on the localhost at port 13000 and waits for a connection
#when it connects it listens for the client to say hello or ask for a vector,
#it then replies with a hello or a comma seperated list of floats for the vector.
import socket
import random

host = "localhost"
port = 13000
#set up the socket
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
#bind it.
s.bind((host,port))
print("listening")
s.listen(1)
conn, addr = s.accept()
print ("got connection\n")
while 1:
	#recieve some data, arbitrarily limited at 100 bytes.
    data = conn.recv(100)
    if not data:
	    break
    print("recieved: ",data)
	#decode the bytes into a string
    dataString = data.decode();
    if  dataString== "hello":
	    #if the client sent hello, send a hello back
        print("sending hello")
        conn.send(b"hello")
    if dataString == "vec":
	    #if the client sent vec, then produce a and send back 3 random numbers.
        vectorString = format("%f, %f, %f"%(random.uniform(-10,10),random.uniform(0,10),random.uniform(-10,10)))
        
        print("sending vec: ",vectorString)
        conn.send(vectorString.encode())

		
conn.close()


