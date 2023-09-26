using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Packague
{
    public int clientID;
    public PackagueType packagueType;
    public Data data;
    public PackagueOptions[] options;
    
    public Packague(PackagueType packagueType, int clientID, PackagueOptions[] options, Data data)
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
public class Data
{
    public string method;
    public DataParameterInfo[] parameters;
    public int targetID;

    public Data(string method, DataParameterInfo[] parameters, int targetID)
    {
        this.method = method;
        this.parameters = parameters;
        this.targetID = targetID; 
    }
    
    // Default constructor
    public Data()
    {
        this.method = "";
        this.parameters = new DataParameterInfo[]{};
        this.targetID = -1; 
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
}

[System.Serializable]
public enum PackagueOptions
{
    NONE = 0,
    // Option if you want to send back the packague to the sender in case of a target rpc
    TARGET_SEND_BACK = 1,
    // Option if you want to send back the packague to the sender in case of a rpc
    RPC_DONT_SEND_BACK = 2,
}