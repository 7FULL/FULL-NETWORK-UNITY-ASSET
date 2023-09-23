using System;
using UnityEngine;

public class FULL: MonoBehaviour
{
    public Transport transport;
    
    // This variable is used to know is own by the client or is a remote client
    private bool _isMine;
    
    private bool IsMine
    {
        get
        {
            if (transport.GetConnectionID() == connectionID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
    // This variable is used to know if the client is connected to the server
    public bool isConnected;
    
    // We get the connection ID from the transport
    public int connectionID;

    // This method is called from the client to connect to the server
    public void StartClient()
    {
        transport.StartClient();
        
        connectionID = transport.GetConnectionID();
    }
    
    // We suscribe to the OnConnected event from the transport
    private void OnEnable()
    {
        transport.ConnectionChange += OnConnectionChange;
    }
    
    // We unsuscribe to the OnConnected event from the transport
    private void OnDisable()
    {
        transport.ConnectionChange -= OnConnectionChange;
    }
    
    private void OnConnectionChange(bool isConnected)
    {
        this.isConnected = isConnected;
    }
    
    public delegate void RPCMethodDelegate();


    // This method is called from the client to execute a RPC
    public void SendRPC(RPCMethodDelegate method, int clientID = -1, PackagueOptions[] options = null)
    {
        if (!isConnected)
        {
            Debug.LogError("Client is not connected, connecting automatically...");
            StartClient();
        }
        
        // If the client specify an ID, then it is a target RPC
        
        PackagueType type = PackagueType.RPC;
        
        string message = "{" +
                         "method:" + method.Method.Name + ",";
        
        message += "parameters:[],";
        
        
        if (clientID != -1)
        {
            message += "targetID:" + clientID;
            type = PackagueType.TARGETRPC;
        }
        
        message += "}";

        transport.SendTCPMessague(type , message, options);
    }

    // This method is called from the clietn to execute a RPC by name
    public void SendRPC(string method, object[] parameters = null, int clientID = -1, PackagueOptions[] options = null)
    {
        if (!isConnected)
        {
            Debug.LogError("Client is not connected, connecting automatically...");
            StartClient();
        }
        
        // If the client specify an ID, then it is a target RPC
        
        PackagueType type = PackagueType.RPC;
        
        string message = '{' +
                         "method:" + method + ",";

        message += "parameters:[";
        if (parameters != null)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                message += "{";
                message += "type:" + parameters[i].GetType() + ",";
                message += "value:" + parameters[i];
                message += "},";
            }
            
        }
        message += "],";

        if (clientID != -1)
        {
            message += "targetID:" + clientID;
            type = PackagueType.TARGETRPC;
        }
        else
        {
            message += "targetID: -1";
        }
        
        message += "}";

        transport.SendTCPMessague(type , message, options);
    }
    
    public void SendRPC(string method, int clientID = -1, PackagueOptions[] options = null, object[] parameters = null)
    {
        if (!isConnected)
        {
            Debug.LogError("Client is not connected, connecting automatically...");
            StartClient();
        }
        
        // If the client specify an ID, then it is a target RPC
        
        PackagueType type = PackagueType.RPC;
        
        string message = "{" +
                         "method:" + method + ",";

        message += "parameters:[";
        if (parameters != null)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                message += "{";
                message += "type:" + parameters[i].GetType() + ",";
                message += "value:" + parameters[i];
                message += "}";
            }
            
        }
        message += "],";

        if (clientID != -1)
        {
            message += "targetID:" + clientID;
            type = PackagueType.TARGETRPC;
        }
        
        message += "}";

        transport.SendTCPMessague(type , message, options);
    }
    
    public void SendRPC(string method, int clientID = -1, object[] parameters = null, PackagueOptions[] options = null)
    {
        if (!isConnected)
        {
            Debug.LogError("Client is not connected, connecting automatically...");
            StartClient();
        }
        
        // If the client specify an ID, then it is a target RPC
        
        PackagueType type = PackagueType.RPC;
        
        string message = "{" +
                         "method:" + method + ",";

        message += "parameters:[";
        if (parameters != null)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                message += "{";
                message += "type:" + parameters[i].GetType() + ",";
                message += "value:" + parameters[i];
                message += "}";
            }
            
        }
        message += "],";

        if (clientID != -1)
        {
            message += "targetID:" + clientID;
            type = PackagueType.TARGETRPC;
        }
        
        message += "}";

        transport.SendTCPMessague(type , message, options);
    }
}