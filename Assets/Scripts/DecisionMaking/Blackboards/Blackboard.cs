using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    protected Dictionary<string, object> Info = new Dictionary<string, object>();

    public object GetFromDictionary(string key)
    {
        object ret = null;
        Info.TryGetValue(key, out ret);
        return ret;
    }

    public void AddToDictionary(string key, object value)
    {
        if (Info.ContainsKey(key))
        {
            Info[key] = value;
        }
        else
        {
            Info.Add(key, value);
        }
    }
}
