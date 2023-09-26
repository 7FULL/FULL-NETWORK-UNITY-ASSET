using UnityEngine;

public class NetworkBehaviourLite: MonoBehaviour
{
    #region NetworkBehaviour

    private bool isConnected;
    
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
}