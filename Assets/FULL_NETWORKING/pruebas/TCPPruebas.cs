using UnityEngine;

public class TCPPruebas : MonoBehaviour
{
    public PackagueType type;
    
    public PackagueOptions[] options;

    public void SendTCPMessague(string message)
    {
        Transport.SendTCPMessague(type, message, options);
    } 
}
