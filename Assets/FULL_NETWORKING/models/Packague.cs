using System;
using System.Collections.Generic;
using UnityEngine;

public class Packague
{
    private int clientID;
    private PackagueType packagueType;
    private Data data;
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

    public Data Data
    {
        get => data;
        set => data = value;
    }

    public Packague(PackagueType packagueType, int clientID, PackagueOptions[] options, Data data)
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
        
        json += "\"data\":" + Data.ToJson() ;
        json += "}";
        
        return json;
    }
    
    public static Packague FromJson(string json)
    {
        string data = json.Split("data")[1];

        Data dataParsed = Data.FromJson(data);

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
            dataParsed
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
               "\t data: " + data.ToString() + "\n" +
               "}";
    }
}

public class Data
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
                string[] jsonSplitted = json.Split("parameters");

                string method = json.Split("method")[1];
                
                method = method.Split(",")[0];
                
                List<object> parameters = new List<object>();
                
                string parametersString = jsonSplitted[1].Split("targetID")[0];

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

                if (method.StartsWith(":"))
                {
                    method = method.Substring(1);
                }
                
                if (method.StartsWith("\":\""))
                {
                    method = method.Substring(3);
                }
                
                if (method.EndsWith("\""))
                {
                    method = method.Substring(0, method.Length - 1);
                }

                string targetIDString = jsonSplitted[1].Split("targetID")[1];
                
                targetIDString = targetIDString.Substring(0, targetIDString.Length - 1);

                // We take the first : from targetID
                targetIDString = targetIDString.Substring(1);

                if (targetIDString.StartsWith(":"))
                {
                    targetIDString = targetIDString.Substring(1);
                }

                if (targetIDString.EndsWith("}"))
                {
                    targetIDString = targetIDString.Substring(0, targetIDString.Length - 1);
                }

                int targetID = int.Parse(targetIDString);

                return new Data(method, parameters.ToArray(), targetID);
            }
            
            public override string ToString()
            {
                string parametersString = "";
                for (int i = 0; i < parameters.Length; i++)
                {
                    parametersString += parameters[i];
                    if (i < parameters.Length - 1)
                    {
                        parametersString += ",";
                    }
                }
                
                return "{\n" +
                       "\t method: " + method + ",\n" +
                       "\t parameters: " + parametersString + ",\n" +
                       "\t targetID: " + targetID + "\n" +
                       "}";
            }
            
            public string ToJson()
            {
                string json = "{";
                json += "\"method\":\"" + method + "\",";
                
                json += "\"parameters\":[";
                for (int i = 0; i < parameters.Length; i++)
                {
                    json += "{";
                    json += "type:" + parameters[i].GetType() + ",";
                    json += "value:" + parameters[i];
                    json += "}";
                    
                }
                json += "],";
                
                json += "\"targetID\":" + targetID;
                
                json += "}";
                
                return json;
            }
        }

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
}

public enum PackagueOptions
{
    // Option if you want to send back the packague to the sender in case of a target rpc
    TARGET_SEND_BACK = 1,
    // Option if you want to send back the packague to the sender in case of a rpc
    RPC_DONT_SEND_BACK = 2,
}