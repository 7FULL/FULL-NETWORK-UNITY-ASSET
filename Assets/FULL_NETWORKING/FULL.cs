using System;
using UnityEngine;

public class FULL: MonoBehaviour
{
    // This variable is used to know is own by the client or is a remote client
    private bool _isMine;
    
    private bool IsMine
    {
        get
        {
            if (Transport.GetConnectionID() == connectionID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // We get the connection ID from the transport
    private int connectionID;
    
    public int ConnectionID
    {
        get => connectionID;
        set => connectionID = value;
    }

    private void Start()
    {
        if (IsMine)
        {
            ConnectionID = Transport.GetConnectionID();
        }
    }

    public delegate void RPCMethodDelegate();
    
    public void SendRPC(RPCMethodDelegate method, int clientID = -1, PackagueOptions[] options = null)
    {
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

        Transport.SendTCPMessague(type , message, options);
    }
    
    public void SendRPC(string method, object[] parameters = null, int clientID = -1, PackagueOptions[] options = null)
    {
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

        Transport.SendTCPMessague(type , message, options);
    }
    
    public void SendRPC(string method, int clientID = -1, PackagueOptions[] options = null, object[] parameters = null)
    {
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

        Transport.SendTCPMessague(type , message, options);
    }
    
    public void SendRPC(string method, int clientID = -1, object[] parameters = null, PackagueOptions[] options = null)
    {
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

        Transport.SendTCPMessague(type , message, options);
    }
}