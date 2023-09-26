using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class NetworkManager: MonoBehaviour, IConnectionCallbacks
{
    // SINGLETON
    public static NetworkManager Instance;

    public GameObject playerPrefab;

    public NetworkSettings settings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        
        ICallbacks[] callbacks = FindObjectsOfType<MonoBehaviour>().OfType<ICallbacks>().ToArray();
        
        Transport.InitializeConnectionCallbacksContainer(callbacks);
    }

    public void OnConnected()
    {
        Transport.SendTCPMessague(new Packague(PackagueType.CHECK_PLAYERS,Transport.ConnectionID, new PackagueOptions[]{PackagueOptions.NONE}, new Data()));
    }

    public void OnDisconnected() {}

    public void OnClientConnected(int connectionID)
    {
        // This is class is from this github: https://github.com/PimDeWitte/UnityMainThreadDispatcher/blob/master/Runtime/UnityMainThreadDispatcher.cs
        // It is just use to execute code in the main thread
        // And since this is a callback from the transport receiveMessageue, it is executed in a different thread
        MainThreadDispatcher.DispatcherInstance().Enqueue(() =>
        {
            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<FULL>().ConnectionID = connectionID;
        });
    }
}