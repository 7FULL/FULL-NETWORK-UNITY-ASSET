using UnityEngine;

[CreateAssetMenu(fileName = "NetworkSettings", menuName = "FULL_NETWORKING/NetworkSettings", order = 0)]
public class NetworkSettings: ScriptableObject
{
    public int MAX_MESSAGE_SIZE;
}