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
            if (syncVar1.toText() == syncVar.toText())
            {
                return;
            }
        }

        syncVars.Add(syncVar);
    }

    public static void GenerateUniqueIDS()
    {
        foreach (SyncVar<object> syncVar in syncVars)
        {
            syncVar.GenerateUniqueID();
        }
    }
    
    public static void GenerateClientID(SyncVar<object> syncVar)
    {
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
    }

    public static void RemoveSyncVar(SyncVar<object> syncVar)
    {
        syncVars.Remove(syncVar);
    }

    public static void Initialize()
    {
        // First we check for duplicate syncVars
        for (int i = 0; i < syncVars.Count; i++)
        {
            for (int j = 0; j < syncVars.Count; j++)
            {
                if (i != j)
                {
                    if (syncVars[i].toText() == syncVars[j].toText())
                    {
                        syncVars.RemoveAt(j);
                    }
                }
            }
        }
        
        // We generate the clientID for each syncVar
        for (int i = 0; i < syncVars.Count; i++)
        {
            GenerateClientID(syncVars[i]);
        }

        List<SyncVarData> syncVarsData = new List<SyncVarData>();

        // We generate SyncVarData
        for (int i = 0; i < syncVars.Count; i++)
        {
            if (syncVars[i].id == -1)
            {
                syncVarsData.Add(new SyncVarData( syncVars[i].type, syncVars[i].id, syncVars[i].Value.ToString(), syncVars[i].localID, syncVars[i].uniqueID));
            }
        }

        Transport.RegisterSyncVars(syncVarsData.ToArray());
        
        printSyncVars();
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
        printSyncVars();
        
        // We check if the syncVar is already in the list if so we updated it and activate the OnChange event
        foreach (SyncVar<object> syncVar in syncVars)
        {
            if (syncVar.uniqueID == syncVarData.uniqueID)
            {
                // We check if the value is the same except for the id in that case we update the id
                if (syncVar.id != syncVarData.id)
                {
                    syncVar.id = syncVarData.id;
                    
                    Debug.Log("ID updated");
                    return;
                }
                
                syncVar.InvokeOnChange(syncVarData.value, syncVar.Value);
                syncVar.Value = (object) Convert.ChangeType(syncVarData.value, syncVar.type);
                return;
            }
        }
        
        // If the syncVar is not in the list we add it
        SyncVar<object> syncVar1 = new SyncVar<object>(Type.GetType(syncVarData.type), syncVarData.id, syncVarData.value, syncVarData.localID);
        
        GenerateClientID(syncVar1);

        syncVars.Add(syncVar1);
        
        printSyncVars();
    }

    public static void printSyncVars()
    {
        // We print the syncVars
        string syncVarsString = "";
        
        foreach (SyncVar<object> syncVar2 in syncVars)
        {
            syncVarsString += syncVar2.toText() + "\n";
        }
        
        Debug.Log("SyncVars: \n" + syncVarsString);
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

    public static void checkSyncVars()
    {
        Transport.SendTCPMessague(new Packague(PackagueType.CHECK_SYNCVARS, new PackagueOptions[]{PackagueOptions.NONE}, new PlainData()));
    }

}