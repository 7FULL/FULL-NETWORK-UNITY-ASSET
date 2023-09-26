using System;
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

    public void OnConnected() {}

    public void OnDisconnected() {}

    public void OnClientConnected(int connectionID)
    {
        GameObject x = Instantiate(playerPrefab);

        x.GetComponent<FULL>().ConnectionID = connectionID;
        
        // x.GetComponent<FULL>().SendRPCIDUpdate();
    }
}