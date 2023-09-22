using UnityEngine;

public class Packague
{
    private int clientID;
    private PackagueType packagueType;
    private string data;
    private PackagueOptions[] options;

    public int ClientID
    {
        get => clientID;
        set => clientID = value;
    }

    public PackagueType PackagueType
    {
        get => packagueType;
        set => packagueType = value;
    }

    public string Data
    {
        get => data;
        set => data = value;
    }

    public Packague(PackagueType packagueType, int clientID, PackagueOptions[] options, string data)
    {
        this.packagueType = packagueType;
        this.clientID = clientID;
        this.options = options;
        this.data = data;
    }

    public string ToJson()
    { 
        string json = "{";
        json += "\"packagueType\":" + (int) packagueType + ",";
        json += "\"clientID\":" + clientID + ",";
        
        json += "\"options\":[";
        for (int i = 0; i < options.Length; i++)
        {
            json += (int) options[i];
            if (i < options.Length - 1)
            {
                json += ",";
            }
        }
        json += "],";
        
        json += "\"data\":\"" + data + "\"";
        json += "}";
        
        return json;
    }
    
    public static Packague FromJson(string json)
    {
        string data = json.Substring(json.IndexOf("\"data\":\"") + 8);
        data = data.Substring(0, data.IndexOf("\""));
        
        string options = json.Substring(json.IndexOf("\"options\":[") + 11);

        if (options.IndexOf("]") > 1)
        {
            options = options.Substring(0, options.IndexOf("]"));
        }
        
        string[] optionsArray = options.Split(',');
        PackagueOptions[] optionsParsed = new PackagueOptions[optionsArray.Length];
        
        Packague packague = new Packague(
            (PackagueType) int.Parse(json.Substring(json.IndexOf("\"packagueType\":") + 15, 1)),
            int.Parse(json.Substring(json.IndexOf("\"clientID\":") + 11, 1)),
            optionsParsed,
            data
        );
        
        return packague;
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
               "\t data: " + data + "\n" +
               "}";
    }
}

public enum PackagueType
{
    HANDSHAKE = 1,
    RPC = 2,
    TARGETRPC = 3,
    POSITION = 4,
    ROTATION = 5,
    PLAIN = 6
}

public enum PackagueOptions
{
    // Option if you want to send back the packague to the sender in case of a target rpc
    TARGET_SEND_BACK = 1,
    // Option if you want to send back the packague to the sender in case of a rpc
    RPC_SEND_BACK = 2,
}