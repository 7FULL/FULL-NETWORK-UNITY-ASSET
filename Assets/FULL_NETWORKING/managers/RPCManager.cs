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
    public void AddRPC(GameObject gameObject, MethodInfo methodInfo)
    {
        if (methods.ContainsKey(gameObject))
        {
            methods[gameObject].Add(new RPCInfo(methodInfo, gameObject.GetComponent<NetworkBehaviour>()));
        }
        else
        {
            methods.Add(gameObject, new List<RPCInfo> {new RPCInfo(methodInfo, gameObject.GetComponent<NetworkBehaviour>())});
        }
    }
    
    // Call RPC
    public void CallRPC(string methodName, object[] parameters)
    {
        foreach (KeyValuePair<GameObject, List<RPCInfo>> pair in methods)
        {
            foreach (RPCInfo rpcInfo in pair.Value)
            {
                //Debug.Log(rpcInfo.toTExt());
                if (rpcInfo.methodInfo.Name == methodName)
                {
                    rpcInfo.methodInfo.Invoke(rpcInfo.target, parameters);
                }
            }
        }
    }
    
    public List<Type> FindNetworkBehaviourTypes()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        List<Type> networkBehaviourTypes = new List<Type>();

        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            networkBehaviourTypes.AddRange(types.Where(t => t.IsSubclassOf(typeof(NetworkBehaviour))));
        }

        return networkBehaviourTypes;
    }
    
    public List<GameObject> FindGameObjectsWithNetworkBehaviour()
    {
        List<Type> networkBehaviourTypes = FindNetworkBehaviourTypes();
        List<GameObject> gameObjectsWithNetworkBehaviour = new List<GameObject>();

        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            NetworkBehaviour[] networkBehaviours = go.GetComponents<NetworkBehaviour>();

            foreach (NetworkBehaviour nb in networkBehaviours)
            {
                if (networkBehaviourTypes.Contains(nb.GetType()))
                {
                    gameObjectsWithNetworkBehaviour.Add(go);
                    break; // No need to check this GameObject further
                }
            }
        }

        return gameObjectsWithNetworkBehaviour;
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
        public string toTExt()
        {
            return "RPCInfo: " + methodInfo.Name + " " + target;
        }
    }
}
