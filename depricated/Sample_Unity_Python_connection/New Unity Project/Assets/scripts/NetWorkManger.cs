/*
NetWorkManager.cs
this is a basic network manager class,
it will connect to a server located on the local host at port 13000,
it will then send a hello message, expecting to recieve one back, then it will request a destination vector for the ball to travel to
once the ball has reached it's destination it will ask the server for a new destination.

WARNING: the getMessageFromServer and sendMessageToServer are both blocking,*/

using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;


public class NetWorkManger : MonoBehaviour {

	public Vector3 destination;
	public NewBehaviourScript player;
	
	TcpClient client;
	bool connected;
	NetworkStream stream;
	
	
	// Use this for initialization
	void Start () {
		
		//call the Connect function with our server name and port number.
		connected = Connect("localhost",13000);
		if (connected){
			print("connected\n");
		}
		else{
			print("could not connect\n");
		}
		
		//send a hello message to the server.
		sendMessageToServer("hello");
		string response = getMessageFromServer();//wait for response.
		if (response!=null){
			//if we got a response, say so.
			print("got response\n");
			print(response);
		}
		else{
			print("no response");
		}
		
		//request the first destination vector.
		sendMessageToServer("vec");
		response = getMessageFromServer();
		Vector3 vec = vector3FromString(response);
		player.destination = vec;
		
	}
	
	// Update is called once per frame
	void Update () {
		//is the player at the destination location?
		if (player.atLocation==true){
			
			
			//send vec to the server,
			sendMessageToServer("vec");
			//get response
			string response = getMessageFromServer();
			//try and parse response as vector3
			Vector3 vec = vector3FromString(response);
			//set destination
			player.destination = vec;
			//print the new destination vector
			print(vec.ToString("F4"));
			print("");
			
			//set atLocation to false.
			player.atLocation=false;
		}
	}
	
	//takes in a string and sends it to the server.
	void sendMessageToServer(string message){
		if (connected){
			//convert the string to an array of bytes.
			Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
			//send the message over the stream.
			stream.Write(data,0,data.Length);
		}
	}
	
	//waits for the server to send data and returns it as a string.
	string getMessageFromServer(){
		
		if (connected){		
			
			//allocate some memory for the data.
			Byte[] data;
			data= new Byte[256];
			
			//try and read in bytes from the stream from the server.
			Int32 numBytesRead = stream.Read(data,0,data.Length);
			//decode the bytes to a string
			String responseData = System.Text.Encoding.ASCII.GetString(data,0,numBytesRead);
			
			
			//return the string.
			return responseData;
		}
		else{
			return null;
		}
	}
	
	
	//this function takes in a string for a server name and a port number and tries
	//to connect to it.
	bool Connect (String server, Int32 port){
		try{
			//try and connect to the server. It's suspiciously easy.....
			client = new TcpClient(server, port);
			//get the byte stream for the socket to make things easier.
			stream = client.GetStream();
		}
		catch{
			return false;
		}
		return true;
	}
	//close the socket and stream.
	void closeConnect(){
		stream.Close();
		client.Close();
	}
	
	
	//takes in a string and tries to convert it to a vector 3.
	public Vector3 vector3FromString(string rString){
		string[] temp = rString.Substring(0,rString.Length-2).Split(',');
		float x = float.Parse(temp[0]);
		float y = float.Parse(temp[1]);
		float z = float.Parse(temp[2]);
		Vector3 rValue = new Vector3(x,y,z);
		return rValue;
	}

	
	
}
