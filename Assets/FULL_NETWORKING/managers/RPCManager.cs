using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class RPCManager
{
    private Dictionary<GameObject, List<RPCInfo>> methods = new Dictionary<GameObject, List<RPCInfo>>();
    public RPCManager()
    {
        GameObject[] objetosEnEscena = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject objeto in objetosEnEscena)
        {
            NetworkBehaviour[] scripts = objeto.GetComponents<NetworkBehaviour>();
            
            foreach (NetworkBehaviour script in scripts)
            {
                MethodInfo[] metodos = script.GetType().GetMethods();
                
                List<RPCInfo> metodosConRPC = new List<RPCInfo>();
                
                foreach (MethodInfo metodo in metodos)
                {
                    if (metodo.GetCustomAttributes(typeof(ClientRPCAttribute), true).Length > 0)
                    {
                        metodosConRPC.Add(new RPCInfo(metodo, script));
                    }
                }

                methods.Add(objeto, metodosConRPC.ToList());
            }
        }
    }
    
    // This is to add a new RPC method to the list
    public void AddRPC(GameObject gameObject)
    {
        MethodInfo[] metodos = gameObject.GetComponents<NetworkBehaviour>().SelectMany(x => x.GetType().GetMethods()).ToArray();
        
        List<RPCInfo> metodosConRPC = new List<RPCInfo>();
        
        foreach (MethodInfo metodo in metodos)
        {
            if (metodo.GetCustomAttributes(typeof(ClientRPCAttribute), true).Length > 0)
            {
                metodosConRPC.Add(new RPCInfo(metodo, gameObject.GetComponent<NetworkBehaviour>()));
            }
        }

        methods.Add(gameObject, metodosConRPC.ToList());
    }
    
    // Call RPC
    public void CallRPC(string methodName, DataParameterInfo[] parameters)
    {
        bool aux = false;
        foreach (KeyValuePair<GameObject, List<RPCInfo>> pair in methods)
        {
            foreach (RPCInfo rpcInfo in pair.Value)
            {
                if (rpcInfo.methodInfo.Name == methodName)
                {
                    aux = true;
                    
                    
                    rpcInfo.methodInfo.Invoke(rpcInfo.target, parseParameters(parameters));
                }
            }
        }

        if (!aux)
        {
            Debug.LogWarning("The rpc " + methodName + " doesn't exist");
        }
    }

    private object[] parseParameters(DataParameterInfo[] parameters)
    {
        List<object> parametersList = new List<object>();
        
        foreach (DataParameterInfo parameter in parameters)
        {
            Type type = Type.GetType(parameter.type);
            parametersList.Add(Convert.ChangeType(parameter.value, type));
        }
        
        return parametersList.ToArray();
    }

    public class RPCInfo
    {
        public MethodInfo methodInfo;
        public object target;
        
        public RPCInfo(MethodInfo methodInfo, object target)
        {
            this.methodInfo = methodInfo;
            this.target = target;
        }
        
        // We use this method instead of ToString() because ToString() can only be called from the main thread
        // and this is executing in a background thread
        public string toText()
        {
            return methodInfo.Name;
        }
    }
}
