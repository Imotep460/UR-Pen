using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

sealed class Surrogates : ISerializationSurrogate
{
    /// <summary>
    /// Method called to serialize a Vector3 object
    /// </summary>
    public void GetObjectData(System.Object obj,
                                SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
        Debug.Log(v3);
    }

    /// <summary>
    /// Method called to deserialize a Vector3 object
    /// </summary>
    public System.Object SetObjectData(System.Object obj,
                                        SerializationInfo info, StreamingContext context,
                                        ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3)obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        v3.z = (float)info.GetValue("z", typeof(float));
        obj = v3;
        return obj; // Formatters ignore this return value // Seems to have been fixed
    }

    ///// <summary>
    ///// Include the Surrogate in your formatter like this
    ///// </summary>
    //BinaryFormatter bf = new BinaryFormatter();

    //// 1. Construct a SurrogateSelector object
    //SurrogateSelector ss = new SurrogateSelector();

    //Surrogates v3ss = new Surrogates();
    //ss.AddSurrogate(typeof(Vector3), 
    //                     new StreamingContext(StreamingContextStates.All), 
    //                     v3ss);
         
    //     // 2. Have the formatter use our surrogate selector
    //     bf.SurrogateSelector = ss;
}

/// <summary>
/// Code taken from:
/// https://answers.unity.com/questions/956047/serialize-quaternion-or-vector3.html
/// </summary>