using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// Any MonoBehaviour that implements the ISaveable interface will be saved in the scene, and loaded back.
/// </summary>
public interface ISaveable
{
    // The Save ID is a unique string that identifies a component in the save data.
    // It's used for finding that object again when the game is loaded.
    string SaveID { get; }

    // The SavedData is the content that will be written to disk.
    // It's asked for when the game is saved.
    JsonData SavedData { get; }

    // LoadFromData is called when the game is being loaded.
    // The object is provided with the data that was read,
    // and is expected to use that information to restore its previous state.
    void LoadFromData(JsonData data);
}

public static class SavingService
{
    // To avoid problems caused by typos,
    // we'll store the names of the strings we use to store and look up items in the JSON as constant strings.
    private const string ACTIVE_SCENE_KEY = "activeScene";
    private const string SCENES_KEY = "scenes";
    private const string OBJECTS_KEY = "objects";
    // (Use an unexpected character "$" for the Save ID here to reduce the chance of collisions).
    private const string SAVEID_KEY = "$saveID";

    private const string POINTS_STRING_KEY = "points";

    public static List<string> LOADED_POINTS = new List<string>();

    // A reference to the delegate that runs after the scene loads,
    // which performs the object state restoration.
    static UnityEngine.Events.UnityAction<Scene, LoadSceneMode>
        LoadObjectsAfterSceneLoad;

    /// <summary>
    /// Saves the game, and writes it to a file called fileName in the app's persistent data directory
    /// </summary>
    public static void SaveGame(string fileName)
    {
        // Create the JsonData that we will eventually write to the disk
        var result = new JsonData();

        // Find All MonoBehaviours by first finding every MonoBehaviou,
        // and filtering it to only include those that are ISaveable.
        var allSaveableObjects = Object
            .FindObjectsOfType<MonoBehaviour>()
            .OfType<ISaveable>();

        // Check to see if we have any objects to save
        if (allSaveableObjects.Count() > 0)
        {
            // Create the JsonData that will store the list of objects.
            var savedObjects = new JsonData();

            // Iterate over every object we want to save
            foreach (var saveableObject in allSaveableObjects)
            {
                // Get the object's saved data
                var data = saveableObject.SavedData;

                // We expect this to be an object
                // (JSON's term for a dictionary)
                // because we need to include the object's Save ID
                if (data.IsObject)
                {
                    // Record the Save ID for this object
                    data[SAVEID_KEY] = saveableObject.SaveID;

                    // Add the object's save data to the collection
                    savedObjects.Add(data);
                }
                else
                {
                    // Provide a helpful warning that we can't save this object
                    var behaviour = saveableObject as MonoBehaviour;

                    Debug.LogWarningFormat(behaviour, "{0}'s save data is not a dictionary. The object was therefore not saved", behaviour.name);
                }
            }

            // Store the collection of saved objects in the result
            result[OBJECTS_KEY] = savedObjects;
        }
        else
        {
            // We have no objects to save. Give a nice warning.
            Debug.LogWarningFormat("The scene did not include any saveable objects.");
        }

        // Next, we need to record what scenes are open.
        // Unity lets you have multiple scenes open at the same time,
        // so we need to store all of them, as well as which scene is the "active" scene
        // (the scene that new objects are added to, and which controls the lighting settings for the game).

        // Create a JsonData that will store the list of open scenes,
        var openScenes = new JsonData();

        // Ask the scene manager how many scenes are open, and for each one store the scene's name.
        var sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            openScenes.Add(scene.name);
        }

        // Store the list of open scenes
        result[SCENES_KEY] = openScenes;

        // Store the name of the active scene
        result[ACTIVE_SCENE_KEY] = SceneManager.GetActiveScene().name;

        // First check to see if TestSimulation.pointsStringList contains any points at all.
        var pointCount = TestSimulation.pointsStringList.Count();

        // If TestSimulation.pointsStringList does NOT contain any points/strings output an warning message.
        if (pointCount == 0)
        {
            Debug.LogWarning("There is no points in pointsStringList");
        }
        else
        {
            // If there are points/strings in TestSimulation.pointsStringList, convert them to json for storage in file.
            var points = new JsonData();
            for (int i = 0; i < pointCount; i++)
            {
                var point = TestSimulation.pointsStringList[i];
                points.Add(point);
            }

            // Store the points/strings.
            result[POINTS_STRING_KEY] = points;
        }

        // We've now finished generating the save data, and it's now time to write it to the disk.

        // Figure out where to put the file by combining the persistent data path with the filename,
        // that this method recieved as a parameter.
        var outputPath = Path.Combine(Application.persistentDataPath, fileName);

        // Create a JsonWriter, and configure it to 'pretty-print' the data.
        // This is optional (you could just call result.ToJson() with no JsonWriter parameter and receive a string),
        // but this way the resulting JSON is easier to read and understand, which is helpful while developing.
        var writer = new JsonWriter();
        writer.PrettyPrint = true;

        // Convert the save data to JSON text
        result.ToJson(writer);

        // Write the JSON text to the disk
        File.WriteAllText(outputPath, writer.ToString());

        // Notify where to find the save game file.
        Debug.LogFormat("Wrote saved game to {0}", outputPath);

        // We allocated a lot of memory here,
        // Which means that there's an increased chance of the garbage collector needing to run in the future.
        // To tidy up, we'll release our reference to the saved data,
        // and then ask the garbage collector to run immdiately.
        // This will result in a slight performance hitch as the collector runs,
        // but that's fine for this case, since users expect saving the game to pause for a second.
        result = null;
        System.GC.Collect();
    }

    /// <summary>
    /// Loads the game from a given file, and restores its state.
    /// </summary>
    public static bool LoadGame(string fileName)
    {
        // Figure out where to find the file.
        var dataPath = Path.Combine(Application.persistentDataPath, fileName);

        // Ensure that a file actually exists there.
        if (File.Exists(dataPath) == false)
        {
            Debug.LogErrorFormat("No file exists at {0}", dataPath);
            return false;
        }

        // Read the data as JSON.
        var text = File.ReadAllText(dataPath);
        var data = JsonMapper.ToObject(text);

        // Ensure that we successfully read the data, and that it's an object (i.e., a JSON dictionary)
        if (data == null || data.IsObject == false)
        {
            Debug.LogErrorFormat("Data at {0} is not a JSON object", dataPath);
            return false;
        }

        // We need to know what scenes to load.
        if (!data.ContainsKey("scenes"))
        {
            Debug.LogWarningFormat("Data at {0} does not contain any scenes; therefore not loading any", dataPath);
            return false;
        }

        // Get the list of scenes
        var scenes = data[SCENES_KEY];
        int sceneCount = scenes.Count;

        if (sceneCount == 0)
        {
            Debug.LogWarningFormat("Data at {0} doesn't speciy any scenes to load.", dataPath);
            return false;
        }

        // Load each specified scene.
        for (int i = 0; i < sceneCount; i++)
        {
            var scene = (string)scenes[i];

            // If this is the first scene we're loading, load it and replace every other active scene.
            if (i == 0)
            {
                SceneManager.LoadScene(scene, LoadSceneMode.Single);
            }
            else
            {
                // Otherwise, load that scene on top of the existing ones.
                SceneManager.LoadScene(scene, LoadSceneMode.Additive);
            }
        }

        // Find the active scene, and set it
        if (data.ContainsKey(ACTIVE_SCENE_KEY))
        {
            var activeSceneName = (string)data[ACTIVE_SCENE_KEY];
            var activeScene = SceneManager.GetSceneByName(activeSceneName);

            if (activeScene.IsValid() == false)
            {
                Debug.LogErrorFormat("Data at {0} specifies an active scene that doesn't exist. Stopping loading here.", dataPath);
                return false;
            }

            //// Not excatly sure why but the line below "SceneManager.SetActiveScene(activeScene)" causes a bug
            //// "ArgumentException: SceneManager.SetActiveScene failed; scene GetPenUniverse" is not loaded",
            //// causing the GameObject to spawn in the location it is placed in, in the editor.
            //// I am not sure why this happens (might have to wait a frame between loading each scene (see above) to set ActiveScene)
            //// Until fixed "SceneManager.SetActiveScene(activeScene)" stays as a comment.
            //SceneManager.SetActiveScene(activeScene);
            Debug.Log("Scene was loaded");
        }
        else
        {
            // This is not an error, since the first scene in the list will be treated as active,
            // but it's worth warning about.
            Debug.LogWarningFormat("Data at {0} does not specify an active scene.", dataPath);
        }

        // Get the list of points.
        var points = data[POINTS_STRING_KEY];
        int pointsCount = points.Count;

        // Check if there are points in the save file.
        if (pointsCount == 0)
        {
            Debug.LogWarningFormat("Data at {0} does not contain any points.");
        }
        else
        {
            // Split the points and add them to a List of strings.
            for (int i = 0; i < pointsCount; i++)
            {
                LOADED_POINTS.Add((string)points[i]);
            }
        }

        // Find all objects in the scene and load them.
        if (data.ContainsKey(OBJECTS_KEY))
        {
            var objects = data[OBJECTS_KEY];

            // We can't update the state of the objects right away because Unity will not complete the scene
            // until some time in the future.
            // Changes we made to the objects would revert to how they're definde in the original scene.
            // As a result, we need to run the code after the scene manager reports that a scene has finished loading.

            // To do this, we create a new delegate that contains our object-loading code,
            // and store that in LoadObjectsAfterSceneLoad.
            // This delegate is added to the SceneManager's sceneLoaded event,
            // which makes it run after the scene finished loading.

            LoadObjectsAfterSceneLoad = (scene, LoadSceneMode) =>
            {
                // Find all ISaveable objects, and build a dictionary that maps their Save ID's to the object
                // (so that we can qickly look them up)
                var allLoadableObjects = Object
                    .FindObjectsOfType<MonoBehaviour>()
                    .OfType<ISaveable>()
                    .ToDictionary(o => o.SaveID, o => o);

                // Get the collection of objects we need to load
                var objectsCount = objects.Count;

                // For each item in the list...
                for (int i = 0; i < objectsCount; i++)
                {
                    // Get the saved data
                    var objectData = objects[i];

                    // Get the Save ID from that data
                    var saveID = (string)objectData[SAVEID_KEY];

                    // Attempt to find the object in the scene(s) that has that Save ID
                    if (allLoadableObjects.ContainsKey(saveID))
                    {
                        var loadableObject = allLoadableObjects[saveID];

                        // Ask the object to load from this data.
                        loadableObject.LoadFromData(objectData);
                    }
                }

                // Tidy up after ourselves; remove this delegate from the sceneLoaded event so that it isn't called next time
                SceneManager.sceneLoaded -= LoadObjectsAfterSceneLoad;

                // Release the reference to the delegate
                LoadObjectsAfterSceneLoad = null;

                // And ask the garbage collector to tidy up
                // (again, this will cause a performance hitch,
                // but users are fine with this as they're already waiting for the scene to finish loading)
                System.GC.Collect();
            };
            // Register the object-loading code to run ater the scene loads.
            SceneManager.sceneLoaded += LoadObjectsAfterSceneLoad;
        }
        return true;
    }
}