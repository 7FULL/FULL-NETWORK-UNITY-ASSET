using UnityEngine;

public class SyncVar<T>: NetworkObject
{
    private T value;

    public SyncVar(T value)
    {
        this.value = value;
        SyncManager.AddSyncVar(this);
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
                    }, new SyncVarData(this.value.ToString(),value.ToString(), ID));
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
        OnChange?.Invoke(oldValue, newValue);
    }
    
    public override string ToString()
    {
        return value.ToString();
    }
}