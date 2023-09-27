using UnityEngine;

public class TCPPruebas : MonoBehaviour
{
    public PackagueType type;
    
    public PackagueOptions[] options;

    public void SendTCPMessague(string message)
    {
        Transport.SendTCPMessague(new Packague(type, options, new PlainData(message)));
    } 
}
