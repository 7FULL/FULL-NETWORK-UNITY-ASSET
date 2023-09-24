public interface ICallbacks {}

public interface IConnectionCallbacks: ICallbacks
{
    
    // This method is called when the client connects to the server
    void OnConnected();
    
    // This method is called when the client disconnects from the server
    void OnDisconnected();
}