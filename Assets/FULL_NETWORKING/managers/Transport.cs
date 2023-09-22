    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using UnityEngine;
    public class Transport: MonoBehaviour
    { 
        private TcpClient client;
        private NetworkStream stream;
        private string serverIP = "localhost"; // Reemplaza con la dirección IP de tu servidor
        private int serverPort = 3000;  //Reemplaza con el puerto de tu servidor
     
        private Thread receiveThread;
     
        private bool isConnected = false;
        
        private RPCManager rpcManager = new RPCManager();

        private void Awake()
        {
            StartServer();
        }

        private void StartServer()
        {
            try
            {
                client = new TcpClient(serverIP, serverPort);
                stream = client.GetStream();
                
                receiveThread = new Thread(ReceiveTCPMessage);
                receiveThread.IsBackground = true;
                receiveThread.Start();
                
                isConnected = true;
            }
            catch (Exception e)
            {
                Debug.LogError("Error: "+e);
                throw;
            }
        }

        public void SendTCPMessague(PackagueType type, string message, PackagueOptions[] options = null)
        {
            if (client == null || !client.Connected)
            {
                isConnected = false;
                StartServer();
            }
            
            try
            {
                if (options == null)
                {
                    options = new PackagueOptions[]{};
                }
                
                Packague packague = new Packague(type,0,options , message);
                byte[] data = Encoding.UTF8.GetBytes(packague.ToJson());
                stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
                if (client.Connected)
                {
                    isConnected = false;
                }
            }
        }
        
        // This method is called when the client receives a message from the server
        private void ReceiveTCPMessage()
        {
            while (isConnected) 
            {
                if (!client.Connected)
                {
                    isConnected = false;
                }
                else
                {
                    List<byte> responseBuffer = new List<byte>();
                
                    // Max message size is 1024 bytes (1 KB) 
                    byte[] buffer = new byte[NetworkingSettings.MAX_MESSAGE_SIZE];
                    int bytesRead;

                    do 
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            responseBuffer.AddRange(buffer.Take(bytesRead));
                        }
                    } while (bytesRead == buffer.Length);

                    string receivedMessage = Encoding.UTF8.GetString(responseBuffer.ToArray());

                    Packague packagueReceived = Packague.FromJson(receivedMessage);
                    
                    Debug.Log(packagueReceived);

                    // If the message is not from the server, we ignore it because it is another client's message
                    if (packagueReceived.ClientID != 0)
                    {
                        return;
                    }

                    switch (packagueReceived.PackagueType)
                    {
                        case PackagueType.HANDSHAKE:
                            
                            break;

                        case PackagueType.RPC:
                            // We execute the RPC
                            
                            rpcManager.CallRPC(packagueReceived.Data, null);
                            break;

                        case PackagueType.TARGETRPC:

                            break;

                        case PackagueType.PLAIN:

                            break;

                        default:

                            break;

                    }
                }
            }
        }
        
        // This method is called when the client disconnects from the server
        private void OnApplicationQuit()
        {
            if (client != null)
            {
                client.Close();
            }
        }
    }