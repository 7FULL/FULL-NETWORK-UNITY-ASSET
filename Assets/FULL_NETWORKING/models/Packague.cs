using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Packague
{
    public int clientID;
    public PackagueType packagueType;
    public string data;
    public PackagueOptions[] options;
    
    public Packague(PackagueType packagueType, PackagueOptions[] options, Data data, int clientID = -1)
    {
        this.packagueType = packagueType;
        this.clientID = clientID;
        if (clientID == -1)
        {
            this.clientID = Transport.ConnectionID;
        }  
        this.options = options;
        this.data = JsonUtility.ToJson(data);
    }
    
    public Packague(PackagueType packagueType, int clientID, PackagueOptions[] options, string data)
    {
        this.packagueType = packagueType;
        this.clientID = clientID;
        this.options = options;
        this.data = data;
    }

    public override string ToString()
    {
        // We make a better string representation of the packague
        string optionsString = "";
        for (int i = 0; i < options.Length; i++)
        {
            optionsString += options[i];
            if (i < options.Length - 1)
            {
                optionsString += ",";
            }
        }
        
        return "Packague:\n" +
               "{\n" +
               "\t packagueType: " + packagueType + ",\n" +
               "\t clientID: " + clientID + ",\n" +
               "\t options: [" + optionsString + "],\n" +
               "\t data: " + data.ToString() + "\n" +
               "}";
    }
}

[System.Serializable]
public abstract class Data
{
}

[System.Serializable]
public class PlainData : Data
{
    public string message;

    public PlainData(string message)
    {
        this.message = message;
    }
    
    // Default constructor
    public PlainData()
    {
        this.message = "";
    }
    
    public override string ToString()
    {
        return "{\n" +
               "\t\t message: " + message + "\n" +
               "\t}";
    }
}

[System.Serializable]
public class RPCData : Data
{
    public string method;
    public DataParameterInfo[] parameters;
    public int targetID;

    public RPCData(string method, DataParameterInfo[] parameters, int targetID)
    {
        this.method = method;
        this.parameters = parameters;
        this.targetID = targetID; 
    }

    public override string ToString()
    {
        string parametersString = "";
        for (int i = 0; i < parameters.Length; i++)
        {
            parametersString += parameters[i].ToString();
            if (i < parameters.Length - 1)
            {
                parametersString += ",";
            }
        }
        
        return "{\n" +
               "\t\t method: " + method + ",\n" +
               "\t\t parameters: [" + parametersString + "],\n" +
               "\t\t targetID: " + targetID + "\n" +
               "\t}";
    }
}

[System.Serializable]
public class DataParameterInfo
{
    public string type;
    public string value;

    public DataParameterInfo(Type type, string value)
    {
        this.type = type.ToString();
        this.value = value;
    }
}

[System.Serializable]
public class SyncVarData : Data
{
    public string oldValue;
    public string newValue;
    public int id;

    public SyncVarData(string oldValue, string newValue, int id)
    {
        this.oldValue = oldValue;
        this.newValue = newValue;
        this.id = id;
    }
}

[System.Serializable]
public enum PackagueType
{
    HANDSHAKE = 1,
    RPC = 2,
    TARGETRPC = 3,
    POSITION = 4,
    ROTATION = 5,
    PLAIN = 6,
    DISCONNECTION = 7,
    CONNECTION = 8,
    CHECK_PLAYERS = 9,
    REGISTER_NETWORK_OBJECT = 10,
    SYNCVAR = 11,
    CHECK_SYNCVARS = 12,
}

[System.Serializable]
public enum PackagueOptions
{
    NONE = 0,
    // Option if you want to send back the packague to the sender in case of a rpc
    DONT_SEND_BACK = 1,
}