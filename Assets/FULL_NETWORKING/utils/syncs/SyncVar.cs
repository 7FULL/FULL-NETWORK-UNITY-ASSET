using System;
using UnityEngine;

public class SyncVar<T>: NetworkObject
{
    private T value;
    public Type type;

    public SyncVar(T value)
    {
        this.value = value;
        this.type = typeof(T);
        this.localID = -1;
        this.id = -1;
        SyncManager.AddSyncVar(new SyncVar<object>( typeof(T), id, value.ToString(), localID));
    }
    
    public SyncVar(Type type, int id, string value, int localID)
    {
        this.type = type;
        this.id = id;
        this.value = (T) Convert.ChangeType(value, type);
        this.localID = localID;
    }

    public T Value
    {
        get { return value; }
        set
        {
            if (!value.Equals(this.value))
            {
                Packague packague = new Packague(PackagueType.SYNCVAR, new PackagueOptions[]
                    {
                        PackagueOptions.DONT_SEND_BACK
                    }, new SyncVarData(type, id, value.ToString(), localID, uniqueID));
                Transport.SendTCPMessague(packague);
                InvokeOnChange(this.value, value);
                
                this.value = value;
            }
        }
    }

    // OnChange event
    public delegate void OnChangeDelegate(T oldValue, T newValue);
    
    public event OnChangeDelegate OnChange;
    
    public void InvokeOnChange(T oldValue, T newValue)
    {
        Debug.Log("InvokeOnChange: " + oldValue + " " + newValue);
        OnChange?.Invoke(oldValue, newValue);
    }
    
    // ToString
    public string toText()
    {
        return "{\n" +
               "\t type: " + type + ",\n" +
               "\t id: " + id + ",\n" +
               "\t value: " + value + "\n" +
               "\t localID: " + localID + "\n" 
                + "\t uniqueID: " + uniqueID + "\n" +
               "}";
    }
}

[System.Serializable]
public class SyncVarData: Data
{
    public string type;
    public string value;
    public int id;
    public int localID;
    public int uniqueID;
    
    public SyncVarData(Type type, int id, string value, int localID, int uniqueID)
    {
        this.type = type.ToString();
        this.id = id;
        this.value = value;
        this.localID = localID;
        this.uniqueID = uniqueID;
    }

    public override string ToString()
    {
        return "{\n" +
               "\t\t type: " + type + ",\n" +
               "\t\t id: " + id + ",\n" +
               "\t\t value: " + value + "\n" +
               "\t\t localID: " + localID + "\n" +
                "\t\t uniqueID: " + uniqueID + "\n" +
               "\t}";
    }
}