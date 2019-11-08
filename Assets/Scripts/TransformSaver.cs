using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To use this script, add a TransforSaver component to your GameObject.
/// It will automatically be included in saved games, and it's state will be restored when the game is loaded.
/// </summary>
public class TransformSaver : SaveableBehavior
{
    // Store the keys we'llbe including in the saved game as constants, to avoid problems with typos.
    private const string LOCAL_POSITION_KEY = "localPosition";
    private const string LOCAL_ROTATION_KEY = "localRotation";
    private const string LOCAL_SCALE_KEY = "localScale";

    /// <summary>
    /// SerializeValue is a helper function that converts an object that Unity already knows how to serialize
    /// (like Vector3, Quaternion, and others) into a JsonData that can be included in the saved game.
    /// </summary>
    private JsonData SerializeValue(object obj)
    {
        // This is very inefficient (we're converting an object to JSON text,
        // Then immediately parsing this text back into a JSON representation),
        // but it means that we don't need to write the (de)serialization code for built-in Unity types.
        return JsonMapper.ToObject(JsonUtility.ToJson(obj));
    }

    /// <summary>
    /// DeserializeValue works in reverse: given a JsonData, it produces a value of the desired type,
    /// as long as that type is one that Unity already knows how to serialize.
    /// </summary>
    private T DeserializeValue<T>(JsonData data)
    {
        return JsonUtility.FromJson<T>(data.ToJson());
    }

    /// <summary>
    /// Provides the saved data for this component.
    /// </summary>
    public override JsonData SavedData
    {
        get
        {
            // Create the JsonData the we'll return to the saved game system.
            var result = new JsonData();

            // Store our position,
            result[LOCAL_POSITION_KEY] = SerializeValue(transform.localPosition);
            // rotation,
            result[LOCAL_ROTATION_KEY] = SerializeValue(transform.localRotation);
            // and scale
            result[LOCAL_SCALE_KEY] = SerializeValue(transform.localScale);

            return result;
        }
    }

    /// <summary>
    /// Given some loaded data, LoadFromData updates the state of the component.
    /// </summary>
    public override void LoadFromData(JsonData data)
    {
        // We can't assume that the data will contain every piece of data that we store;
        // remember ther programmers adage;
        // "be strict in what yo generate, and forgiving in what you accept."

        // Accordingly, we test to see if each item exists in the saved data.

        // Update position:
        if (data.ContainsKey(LOCAL_POSITION_KEY))
        {
            transform.localPosition = DeserializeValue<Vector3>(data[LOCAL_POSITION_KEY]);
        }

        // Update Rotation;
        if (data.ContainsKey(LOCAL_ROTATION_KEY))
        {
            transform.localRotation = DeserializeValue<Quaternion>(data[LOCAL_ROTATION_KEY]);
        }

        // Update Scale;
        if (data.ContainsKey(LOCAL_SCALE_KEY))
        {
            transform.localScale = DeserializeValue<Vector3>(data[LOCAL_SCALE_KEY]);
        }

        // Send/save the start/spawn position and start/spawn rotation to TestSimulation,
        // so the GameObject has access to the positions later.
        TestSimulation.startPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        TestSimulation.startRotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
    }
}