using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
public class StateObject
{
    // Client socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 1500;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();
}
public class NetworkManager
{
    

    public List<string> recievedMessges;
    public int numRecievedMessages=0;

    int portNumber = 11100;

    private static ManualResetEvent connectDone = new ManualResetEvent(false);

    public bool connected;
    public bool connecting;

    bool sendingMessage;

    private Socket socket;

    int numProcessed=0;

    
    public NetworkManager( int newPort)
    {
        portNumber = newPort;
    }
    //creates a new file using todays date and time as the name,
    public bool hasMessage()
    {
        if (recievedMessges.Count > numProcessed)
        {
            return true;
        }
        return false;
    }
    public string getMessage()
    {
        if (recievedMessges.Count > numProcessed)
        {
            string msg = recievedMessges[numProcessed];
            numProcessed++;
            return msg;
        }
        return null;
    }

    public void connectToServer()
    {
        Debug.Log("trying to connect");
        connectDone = new ManualResetEvent(false);
        connecting = true;
        sendingMessage = false;
        recievedMessges = new List<string>();
        numRecievedMessages = 0;
        connected = false;

        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress;
        if(false) //For testing
        {
            ipAddress = ipHostInfo.AddressList[0];
        }
        else
        {
            ipAddress = IPAddress.Parse("107.23.124.173");
        }
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, portNumber);
		
		string prnt_msg = "IP Address: " + ipAddress.ToString() + ":" + portNumber.ToString();
		Debug.Log(prnt_msg);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), socket);
        connectDone.WaitOne();

        if (connected)
        {
            Debug.Log("connected");
            ReceiveMessage(socket);
        }
    }

  
    public void closeConnection()
    {
        if (connected)
        {
            SendMessage("bye");
            Debug.Log("shutting down connection");
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            connected = false;
            connecting = false;
        }
    }



    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);
            


            // Signal that the connection has been made.
            connectDone.Set();
            connected = true;
            
        }
        catch (Exception e)
        {
            connectDone.Set();
            connected = false;
            Debug.Log(e.ToString());
        }
        connecting = false;
    }







    private void ReceiveMessage(Socket client)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                string entireMessage = state.sb.ToString();
                int start = 0;
                for (int i = 0; i < entireMessage.Length; i++)
                {
                    if (entireMessage[i] == '\n')
                    {
                        string message = entireMessage.Substring(start, (i) - start);
                        start = i+1;
                        recievedMessges.Add(message);
                        //log the message if logMessages is enabled.
                        
                        numRecievedMessages++;
                        numRecievedMessages++;
                        state = new StateObject();
                        state.workSocket = client;
                        bool isTrue = ((i + 1) < entireMessage.Length - 1);
                        if (isTrue==true)
                        {
                            try
                            {

                                //string remander = state.sb.ToString(i + 1, entireMessage.Length - (i+2));
                                string remander = entireMessage.Substring(i + 1);

                                Debug.Log(remander);
                                state.sb.Append(remander);
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e.ToString());
                            }

                        }



                    }
                }


            }
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }





    public void SendMessage(string data)
    {
        Debug.Log("sending message: " + data);
        while (sendingMessage == true) { }//wait
        if (connected)
        {
            sendingMessage = true;
            try
            {
                
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                socket.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), socket);
            }
            catch (Exception e)
            {
                sendingMessage = false;
                Console.WriteLine(e.ToString());
            }
        }
    }

    private void SendCallback(IAsyncResult ar)
    {
        if (connected)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                client.EndSend(ar);
                sendingMessage = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                sendingMessage = false;
            }
        }
    }

}