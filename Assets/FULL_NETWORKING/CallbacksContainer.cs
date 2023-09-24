public class CallbacksContainer
{
    ICallbacks[] callbacks;
    
    public CallbacksContainer(ICallbacks[] callbacks)
    {
        this.callbacks = callbacks;
    }
    
    public void OnConnected()
    {
        foreach (ICallbacks callback in callbacks)
        {
            if (callback is IConnectionCallbacks)
            {
                ((IConnectionCallbacks) callback).OnConnected();
            }
        }
    }
    
    public void OnDisconnected()
    {
        foreach (ICallbacks callback in callbacks)
        {
            if (callback is IConnectionCallbacks)
            {
                ((IConnectionCallbacks) callback).OnDisconnected();
            }
        }
    }
}