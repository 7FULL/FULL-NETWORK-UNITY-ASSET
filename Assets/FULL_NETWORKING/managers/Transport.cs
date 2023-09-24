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
        public static int StartClient()
        {
            try
            {
                client = new TcpClient(serverIP, serverPort);
                stream = client.GetStream();
                
                receiveThread = new Thread(ReceiveTCPMessage);
                receiveThread.IsBackground = true;
                receiveThread.Start();
                
                isConnected = true;
                
                // We trigger the OnConnected event of the IConectionCallbacks interface
                callbacksContainer.OnConnected();
            }
            catch (Exception e)
            {
                Debug.LogError("Error: "+e);
                throw;
            }
            
            return GetConnectionID();
        }

        // This method is called from the client to disconnect from the server
        public static void Disconnect()
        {
            if (client != null)
            {
                client.Close();
            }
        }
        
        // This method returns the client ID
        public static int GetConnectionID()
        {
            if (!client.Connected)
            {
                return -1;
            }
            return client.Client.LocalEndPoint.GetHashCode();
        }
        
        public static void SendTCPMessague(PackagueType type, string message, PackagueOptions[] options = null)
        {
            if ((client == null || !client.Connected) && isConnected)
            {
                isConnected = false;
                callbacksContainer.OnDisconnected();
                StartClient();
            }
            
            try
            {
                if (options == null)
                {
                    options = new PackagueOptions[]{};
                }
                
                Packague packague = new Packague(type,GetConnectionID(),options , message);
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
                            Data data = Data.FromJson(packagueReceived.Data);

                            rpcManager.CallRPC(data.method , data.parameters);

                            break;
                    }
                }
            }
        }
        
        private class Data
        {
            public string method;
            public object[] parameters;
            public int targetID;

            public Data(string method, object[] parameters, int targetID)
            {
                this.method = method;
                this.parameters = parameters;
                this.targetID = targetID;
            }
            
            public static Data FromJson(string json)
            {
                string[] jsonSplitted = json.Split(",parameters:");

                string method = jsonSplitted[0].Split(':')[1];
                
                List<object> parameters = new List<object>();
                
                string parametersString = jsonSplitted[1].Split(",targetID:")[0];

                if (parametersString != "[]")
                {
                    string[] parametersSplitted = parametersString.Split('{');
                    
                    for (int i = 1; i < parametersSplitted.Length; i++)
                    {
                        string parameterType = parametersSplitted[i].Split(',')[0].Split(':')[1];
                        string parameterValue = parametersSplitted[i].Split(',')[1].Split(':')[1];
                        
                        parameterValue = parameterValue.Substring(0, parameterValue.Length - 1);
                        
                        switch (parameterType)
                        {
                            case "System.Int32":
                                parameters.Add(int.Parse(parameterValue));
                                break;
                            case "System.String":
                                parameters.Add(parameterValue);
                                break;
                            case "System.Single":
                                parameters.Add(float.Parse(parameterValue));
                                break;
                            case "System.Boolean":
                                parameters.Add(bool.Parse(parameterValue));
                                break;
                            case "System.Double":
                                parameters.Add(double.Parse(parameterValue));
                                break;
                            case "System.Char":
                                parameters.Add(char.Parse(parameterValue));
                                break;
                            case "System.Byte":
                                parameters.Add(byte.Parse(parameterValue));
                                break;
                            case "System.SByte":
                                parameters.Add(sbyte.Parse(parameterValue));
                                break;
                            case "System.UInt16":
                                parameters.Add(ushort.Parse(parameterValue));
                                break;
                            case "System.UInt32":
                                parameters.Add(uint.Parse(parameterValue));
                                break;
                            case "System.UInt64":
                                parameters.Add(ulong.Parse(parameterValue));
                                break;
                            case "System.Int16":
                                parameters.Add(short.Parse(parameterValue));
                                break;
                            case "System.Decimal":
                                parameters.Add(decimal.Parse(parameterValue));
                                break;
                            case "System.DateTime":
                                parameters.Add(DateTime.Parse(parameterValue));
                                break;
                            case "System.Guid":
                                parameters.Add(Guid.Parse(parameterValue));
                                break;
                            case "System.Object":
                                parameters.Add(parameterValue);
                                break;
                            default:
                                Debug.LogError("Type not found: " + parameterType);
                                break;
                        }
                    }
                }
                
                string targetIDString = jsonSplitted[1].Split(",targetID:")[1];
                
                targetIDString = targetIDString.Substring(0, targetIDString.Length - 1);
                
                int targetID = int.Parse(targetIDString);
                
                return new Data(method, parameters.ToArray(), targetID);
            }
        }
    }