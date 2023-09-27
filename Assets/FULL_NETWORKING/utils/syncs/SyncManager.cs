using System.Collections.Generic;
using UnityEngine;

public static class SyncManager
{
    // We use this to store the SyncVars
    private static List<NetworkObject> syncVars = new List<NetworkObject>();

    // We use this to store the SyncLists
    private static List<object> syncList = new List<object>();
    
    // We use this to store the SyncDictionaries
    private static List<object> syncDictionary = new List<object>();

    public static void AddSyncVar(NetworkObject syncVar)
    {
        syncVars.Add(syncVar);
    }

    public static void RemoveSyncVar(NetworkObject syncVar)
    {
        syncVars.Remove(syncVar);
    }

    public static void registerSyncVars()
    {
        List<NetworkObject> syncVars2 = new List<NetworkObject>();
        foreach (NetworkObject syncVar in syncVars)
        {
            int id = Transport.RegisterNetworkObject(syncVar);
            
            // We add the id to the syncVar
            syncVar.ID = id;
        }
        
        syncVars = syncVars2;
    }
    
    // Get
    public static SyncVar<object> GetSyncVar(int id)
    {
        return (SyncVar<object>) syncVars[id];
    }
    
    // Get all
    public static List<NetworkObject> GetSyncVars()
    {
        return syncVars;
    }

}