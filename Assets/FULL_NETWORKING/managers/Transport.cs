    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using UnityEngine;
    public static class Transport
    { 
        private static TcpClient client;
        private static NetworkStream stream;
        private static string serverIP = "localhost"; // Reemplaza con la dirección IP de tu servidor
        private static int serverPort = 3000;  //Reemplaza con el puerto de tu servidor
     
        private static Thread receiveThread;
     
        private static bool _isConnected = false;
        
        private static int connectionID = -1;
        
        public static int ConnectionID
        {
            get => connectionID;
        }

        public static event Action<bool> ConnectionChange;
        
        public static CallbacksContainer callbacksContainer;

        public static bool isConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    ConnectionChange?.Invoke(_isConnected); // Invoca el evento cuando el valor cambia
                }
            }
        }

        private static RPCManager rpcManager = new RPCManager();
        
        // This methos initializes the ConnectionCallbacks container
        public static void InitializeConnectionCallbacksContainer(ICallbacks[] callbacks)
        {
            callbacksContainer = new CallbacksContainer(callbacks);
        }

        // This method is called from the client to connect to the server
        public static int StartClient(FULL f = null)
        {
            if (client != null)
            {
                return -1;
            }
            try
            {
                client = new TcpClient(serverIP, serverPort);
                stream = client.GetStream();
                
                receiveThread = new Thread(ReceiveTCPMessage);
                receiveThread.IsBackground = true;
                receiveThread.Start();
                
                isConnected = true;
                
                int token = -1;
                //We read the token from the server response
                while (token == -1)
                {
                    List<byte> responseBuffer = new List<byte>();
                
                    // Max message size is 1024 bytes (1 KB) 
                    byte[] buffer = new byte[NetworkManager.Instance.settings.MAX_MESSAGE_SIZE];
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

                    token = int.Parse(receivedMessage);
                    
                    connectionID = token;

                    if (f != null)
                    {
                        f.ConnectionID = token;
                    }
                }

                // We trigger the OnConnected event of the IConectionCallbacks interface
                callbacksContainer.OnConnected();
                
                Debug.Log("Connected to server");
            }
            catch (Exception e)
            {
                Debug.LogError("Error: "+e);
                throw;
            }
            
            return connectionID;
        }

        // This method is called from the client to disconnect from the server
        public static void Disconnect()
        {
            if (client != null)
            {
                client.Close();
            }
        }

        public static void SendTCPMessague(PackagueType type, string message, PackagueOptions[] options = null)
        {
            if ((client == null || !client.Connected) && isConnected)
            {
                // Restore variables
                client = null;
                stream = null;
                // We abort the thread before losing the reference
                receiveThread.Abort();
                receiveThread = null;

                isConnected = false;
                callbacksContainer.OnDisconnected();
                Debug.LogWarning("Client disconnected, trying to reconnect...");
                StartClient();
            }
            
            try
            {
                if (options == null)
                {
                    options = new PackagueOptions[]{};
                }
                
                Data dataParsed = Data.FromJson(message);
                Packague packague = new Packague(type,connectionID,options , dataParsed);
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
        private static void ReceiveTCPMessage()
        {
            while (isConnected) 
            {
                if (!client.Connected)
                {
                    isConnected = false;
                    callbacksContainer.OnDisconnected();
                }
                else
                {
                    List<byte> responseBuffer = new List<byte>();
                
                    // Max message size is 1024 bytes (1 KB) 
                    byte[] buffer = new byte[NetworkManager.Instance.settings.MAX_MESSAGE_SIZE];
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

                    /*
                    Packague:
                    {
                        packagueType: TARGETRPC,
                        clientID: 3,
                        options: [0,0,0,0,0,0],
                        data: 
                            {
                                method:RPC_MetodoEjemploConParametros,
                                parameters:
                                        [
                                            {
                                                type:System.Int32,
                                                value:1
                                            },
                                            {
                                                type:System.String,
                                                value:hola
                                            }
                                        ],
                                // In case of TARGETRPC is different from -1
                                targetID: 390221777
                            }
                    }
                    */

                    switch (packagueReceived.PackagueType)
                    {
                        case PackagueType.HANDSHAKE:
                            Debug.Log(packagueReceived.Data);
                            
                            break;
                        
                        case PackagueType.PLAIN:
                            // We trigger the OnMessageReceived event

                            break;
                        
                        case PackagueType.CONNECTION:
                            // If a clients connects 
                            
                            break;
                        
                        case PackagueType.DISCONNECTION:
                            
                            break;

                        // If the message is a RPC or a TARGETRPC, we call the method
                        default:
                            Data data = packagueReceived.Data;

                            rpcManager.CallRPC(data.method, data.parameters);

                            break;
                    }
                }
            }
        }
    }