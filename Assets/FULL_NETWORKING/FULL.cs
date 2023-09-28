using System;
using System.Collections.Generic;
using UnityEngine;

public class FULL: MonoBehaviour
{
    // This variable is used to know is own by the client or is a remote client
    private bool _isMine;

    public void Connect()
    {
        Transport.StartClient(this);
    }
    
    public void Disconnect()
    {
        if (IsMine)
        {
            Transport.Disconnect();
        }
        else
        {
            Debug.LogWarning("You can't disconnect a remote client");
        }
    }

    public void SendRPCIDUpdate()
    {
        ConnectionID = connectionID;
        SendRPC("UpdateRPC", new object[]{connectionID}, PackagueOptions.DONT_SEND_BACK);
    }

    [ClientRPC]
    public void UpdateRPC(int id)
    {
        Debug.Log("UpdateRPC: " + id);
        ConnectionID = id;
    }

    public bool IsMine
    {
        get
        {
            if (Transport.ConnectionID == connectionID)
            {
                return true;
            }
            else
            {
                Debug.Log("IsMine: " + Transport.ConnectionID + " " + connectionID);
                return false;
            }
        }
    }

    // We get the connection ID from the transport
    private int connectionID = -1;
    
    public int ConnectionID
    {
        get => connectionID;
        set => connectionID = value;
    }

    public delegate void RPCMethodDelegate();

    public void SendRPC(string method, int targetID, DataParameterInfo[] parameters = null, PackagueOptions option = PackagueOptions.NONE)
    {
        // If the client specify an ID, then it is a target RPC
        
        PackagueType type = PackagueType.TARGETRPC;

        PackagueOptions[] options = new PackagueOptions[]{};

        if (option != 0)
        {
            options = new PackagueOptions[]{option};
        }

        RPCData data = new RPCData(method, parameters, targetID);
        
        Packague packague = new Packague(type, options, data);

        Transport.SendTCPMessague(packague);
    }
    
    public void SendRPC(string method, object[] parameters = null, PackagueOptions option = PackagueOptions.NONE)
    {
        // If the client specify an ID, then it is a target RPC
        
        PackagueType type = PackagueType.RPC;

        PackagueOptions[] options = new PackagueOptions[]{};

        if (option != 0)
        {
            options = new PackagueOptions[]{option};
        }

        Type a = method.GetType();
        
        List<DataParameterInfo> parametersList = new List<DataParameterInfo>();
        
        foreach (object parameter in parameters)
        {
            parametersList.Add(new DataParameterInfo(parameter.GetType(), parameter.ToString()));
        }

        RPCData data = new RPCData(method, parametersList.ToArray(), -1);
        
        Packague packague = new Packague(type, options, data);

        Transport.SendTCPMessague(packague);
    }
}