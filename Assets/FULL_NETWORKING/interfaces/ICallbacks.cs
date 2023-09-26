public interface ICallbacks {}

public interface IConnectionCallbacks: ICallbacks
{
    
    // This method is called when the client connects to the server
    void OnConnected();
    
    // This method is called when the client disconnects from the server
    void OnDisconnected();
    
    // This meethos is called when a client connects to the server
    void OnClientConnected(int connectionID);
}