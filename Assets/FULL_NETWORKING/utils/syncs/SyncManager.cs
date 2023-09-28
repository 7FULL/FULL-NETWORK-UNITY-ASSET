using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SyncManager
{
    // We use this to store the SyncVars
    private static List<SyncVar<object>> syncVars = new List<SyncVar<object>>();

    public static void AddSyncVar(SyncVar<object> syncVar)
    {
        // We check if the syncVar is already in the list
        foreach (SyncVar<object> syncVar1 in syncVars)
        {
            if (syncVar1.ToString() == syncVar.ToString())
            {
                return;
            }
        }
        
        // We check if the localID is already in use
        bool localIDInUse = false;
        
        foreach (SyncVar<object> syncVar1 in syncVars)
        {
            if (syncVar1.localID == syncVars.Count)
            {
                localIDInUse = true;
            }
        }
        
        if (!localIDInUse)
        {
            syncVar.localID = syncVars.Count;
        }
        else
        {
            syncVar.localID = syncVars.Count+1;
        }

        syncVars.Add(syncVar);
    }

    public static void RemoveSyncVar(SyncVar<object> syncVar)
    {
        syncVars.Remove(syncVar);
    }

    public static void Initialize()
    {
        //TODO: Only register the syncVars that don't have an ID
        SyncVarData[] syncVarsData = new SyncVarData[syncVars.Count];

        // We generate SyncVarData
        for (int i = 0; i < syncVars.Count; i++)
        {
            syncVarsData[i] = new SyncVarData( syncVars[i].type, syncVars[i].id, syncVars[i].Value.ToString(), syncVars[i].localID);
        }

        Transport.RegisterSyncVars(syncVarsData);
    }
    
    public static void registerSyncVars(SyncVar<object>[] syncVarsInfos)
    {
        syncVars = syncVarsInfos.ToList();
    }
    
    [Obsolete]
    public static void UpdateSyncVar(int id, string value, int localID)
    {
        // We check if the syncVar is already in the list if so we updated it and activate the OnChange event
        foreach (SyncVar<object> syncVar in syncVars)
        {
            if (syncVar.id == id)
            {
                syncVar.InvokeOnChange(value, syncVar.Value);
                syncVar.Value = (object) Convert.ChangeType(value, syncVar.type);
                return;
            }
        }
        
        // If the syncVar is not in the list we add it
        SyncVar<object> syncVar1 = new SyncVar<object>(Type.GetType(value), id, value, localID);
        
        syncVars.Add(syncVar1);
    }
    
    public static void UpdateSyncVar(SyncVarData syncVarData)
    {
        // We check if the syncVar is already in the list if so we updated it and activate the OnChange event
        foreach (SyncVar<object> syncVar in syncVars)
        {
            if (syncVar.id == syncVarData.id)
            {
                syncVar.InvokeOnChange(syncVarData.value, syncVar.Value);
                syncVar.Value = (object) Convert.ChangeType(syncVarData.value, syncVar.type);
                return;
            }
        }
        
        // If the syncVar is not in the list we add it
        SyncVar<object> syncVar1 = new SyncVar<object>(Type.GetType(syncVarData.type), syncVarData.id, syncVarData.value, syncVarData.localID);

        syncVars.Add(syncVar1);
    }

    // Get
    public static SyncVar<object> GetSyncVar(int id)
    {
        foreach (SyncVar<object> syncVar in syncVars)
        {
            if (syncVar.id == id)
            {
                return syncVar;
            }
        }

        return null;
    }
    
    // Get by local ID
    public static SyncVar<object> GetSyncVarByLocalID(int localID)
    {
        foreach (SyncVar<object> syncVar in syncVars)
        {
            if (syncVar.localID == localID)
            {
                return syncVar;
            }
        }

        return null;
    }
    
    // Get all
    public static List<SyncVar<object>> GetSyncVars()
    {
        return syncVars;
    }

}