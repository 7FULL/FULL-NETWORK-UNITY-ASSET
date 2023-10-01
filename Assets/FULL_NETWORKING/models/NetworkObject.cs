public class NetworkObject
{
    public int id;
    
    public int localID;
    
    public int uniqueID;
    
    public void GenerateUniqueID()
    {
        uniqueID = UnityEngine.Random.Range(0, 1000000);
    }
}