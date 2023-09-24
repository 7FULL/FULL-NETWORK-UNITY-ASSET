using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class NetworkBehaviour: MonoBehaviour, IConnectionCallbacks
{
    #region NetworkBehaviour

    private bool isConnected;

    private void Awake()
    {
        ICallbacks[] callbacks = FindObjectsOfType<MonoBehaviour>().OfType<ICallbacks>().ToArray();
        
        Transport.InitializeConnectionCallbacksContainer(callbacks);

        Transport.StartClient();
    }

    // This method is called when the client disconnects from the server
    private void OnApplicationQuit()
    {
        Transport.Disconnect();
    }
    
    // We suscribe to the OnConnected event from the transport
    private void OnEnable()
    {
        Transport.ConnectionChange += OnConnectionChange;
    }
    
    // We unsuscribe to the OnConnected event from the transport
    private void OnDisable()
    {
        Transport.ConnectionChange -= OnConnectionChange;
    }
    
    private void OnConnectionChange(bool isConnected)
    {
        this.isConnected = isConnected;
    }
    
    #endregion

    #region IConnectionCallbacks

    // This method is called when the client connects to the server
    public virtual void OnConnected()
    {
    }

    // This method is called when the client disconnects from the server
    public virtual void OnDisconnected()
    {
        
    }
    
    #endregion
}