using System;
using UnityEngine;

public class NetworkManager: MonoBehaviour
{
    // SINGLETON
    public static NetworkManager Instance;

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
    }
}