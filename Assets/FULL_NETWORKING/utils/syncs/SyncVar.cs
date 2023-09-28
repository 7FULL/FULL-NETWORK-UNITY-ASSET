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
                    }, new SyncVarData(type, id, value.ToString(), localID));
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
}

[System.Serializable]
public class SyncVarData: Data
{
    public string type;
    public string value;
    public int id;
    public int localID;
    
    public SyncVarData(Type type, int id, string value, int localID)
    {
        this.type = type.ToString();
        this.id = id;
        this.value = value;
        this.localID = localID;
    }

    public override string ToString()
    {
        return "{\n" +
               "\t\t type: " + type + ",\n" +
               "\t\t id: " + id + ",\n" +
               "\t\t value: " + value + "\n" +
               "\t\t localID: " + localID + "\n" +
               "\t}";
    }
}