using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SaveableBehavior class's OnBeforeSerialize method is called
/// when the class is being saved into the scene in the editor;
/// if SaveableBehaviour doesn't have a Save ID at this point,
/// it generates one and stores it in a variable that's private
/// (so that other classes can't access and modify the Save ID; they don't need to know it),
/// marked as a SerializeField (so that Unity stores it in the scene file), and set to HideInInspector
/// (so it doesn't appear in the Inspector, because there's no reason to edit it).
/// </summary>
public abstract class SaveableBehavior : MonoBehaviour,
    ISaveable, // Marks this class as saveable
    ISerializationCallbackReceiver // Asks Unity to run code when the scene file is saved in the editor.
{
    // This class doesn't implement SaveData or LoadFromData; that's the job of our subclass
    public abstract JsonData SavedData { get; }
    public abstract void LoadFromData(JsonData data);

    // This class _does_ implement the SaveID property; It wraps the _saveID field.
    // (We need to do this manually,
    // rather than using automatic property generation (i.e., "public string SaveID { get; set; }"),
    // because Unity won't store automatic properties when saving the scene file.
    public string SaveID
    {
        get
        {
            return _saveID;
        }
        set
        {
            _saveID = value;
        }
    }

    // The _saveID field stores the actua data that SaveID uses.
    // We mark it as serialized so that Unity editor saves it with the rest of the scene,
    // and as HideInInspector to make it not appear in the inspector
    // (there's no reason for it to be edited).
    [HideInInspector]
    [SerializeField]
    private string _saveID;

    /// <summary>
    /// OnBeforeSerialize is called when Unity is about to save this object as part of a scene file.
    /// </summary>
    public void OnBeforeSerialize()
    {
        // Check to see if we have a Save ID.
        if (_saveID == null)
        {
            // Generate a new unique one, by creating a GUID and getting its string value.
            _saveID = System.Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// OnAfterDeserialize is called when Unity has loaded this object as part of a scene file.
    /// </summary>
    public void OnAfterDeserialize()
    {
        // Nothing special to do here, but the method must exist to implement ISerializationCallbackReceiver.
    }
}
