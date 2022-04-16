using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Spawns specified items that the Player is looking for with distance checks.
/// </summary>
/// <remarks>
/// ExecuteInEditMode Attribute has been added to ensure the spawning items
/// functionality can be tested without entering play mode as well.
/// </remarks>
[ExecuteInEditMode]
public class SpawnGameItems : MonoBehaviour
{
    /// <summary>
    /// Struct for storing spawnable item positions and prefab type.
    /// </summary>
    [System.Serializable]
    struct Spawnable
    {
        /// <summary>
        /// Gets or sets the index for the prefab asset.
        /// </summary>
        public int PrefabIndex { get; set; }

        /// <summary>
        /// Gets or sets the X position to spawn object at.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y position to spawn object at.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the Z position to spawn object at.
        /// </summary>
        public float Z { get; set; }
    }

    /// <summary>
    /// Class which encapsulates all of the persistent data that
    /// our game relies on.
    /// </summary>
    [System.Serializable]
    class PersistentData
    {
        /// <summary>
        /// Player character's x position.
        /// </summary>
        public float PlayerX { get; set; }

        /// <summary>
        /// Player character's y position.
        /// </summary>
        public float PlayerY { get; set; }

        /// <summary>
        /// Player character's z position.
        /// </summary>
        public float PlayerZ { get; set; }

        /// <summary>
        /// Player character's rotation x.
        /// </summary>
        public float RotationX { get; set; }

        /// <summary>
        /// Player character's rotation y.
        /// </summary>
        public float RotationY { get; set; }

        /// <summary>
        /// Player character's rotation z.
        /// </summary>
        public float RotationZ { get; set; }

        /// <summary>
        /// Player character's rotation w.
        /// </summary>
        public float RotationW { get; set; }

        /// <summary>
        /// The list of spawned objects.
        /// </summary>
        public List<Spawnable> SerializableObjects { get; set; }

        /// <summary>
        /// Default constructor instantiates the list of spawnables.
        /// </summary>
        public PersistentData()
        {
            SerializableObjects = new List<Spawnable>();
        }
    }

    /// <summary>
    /// User-specified prefabs that the Spawner will spawn in the Scene.
    /// </summary>
    public GameObject[] Prefabs;

    /// <summary>
    /// Reference to the Player object in the Scene.
    /// </summary>
    public GameObject PlayerObj;

    /// <summary>
    /// Specifies the minimum x position to randomly spawn prefabs at.
    /// </summary>
    public float xMin = -10f;

    /// <summary>
    /// Specifies the maximum x position to randomly spawn prefabs at.
    /// </summary>
    public float xMax = 10f;

    /// <summary>
    /// Specifies the minimum z position to randomly spawn prefabs at.
    /// </summary>
    public float zMin = -10f;

    /// <summary>
    /// Specifies the maximum z position to randomly spawn prefabs at.
    /// </summary>
    public float zMax = 10f;

    /// <summary>
    /// Stores a reference to the Player script component.
    /// </summary>
    private Player _player;

    /// <summary>
    /// All of the persistent data the game needs to restore.
    /// </summary>
    private PersistentData _persistentData = new PersistentData();

    /// <summary>
    /// Relative path for serializing our spawned objects.
    /// </summary>
    const string RELATIVE_PATH = "/savedObjects.sav";

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _player = getPlayerScript();

        // Load previously spawned objects from persistent storage
        _persistentData = load(RELATIVE_PATH);

        if (_persistentData == null)
        {
            _persistentData = new PersistentData();
        }
        else
        {
            if(PlayerObj != null)
            {
                PlayerObj.transform.position = new Vector3(_persistentData.PlayerX, _persistentData.PlayerY, _persistentData.PlayerZ);
                PlayerObj.transform.rotation = new Quaternion(_persistentData.RotationX, _persistentData.RotationY, _persistentData.RotationZ, _persistentData.RotationW);
            }

            // Grab all initial items in Scene to compare against
            _player.DistCheckedItems = _player.GrabItems();
            foreach(var eachObj in _persistentData.SerializableObjects)
            {
                // If there is already an object at this saved position, it has been instantiated in the Scene already
                // when switching back and forth between play and edit modes.
                if (!IsObjectAtPosition(_player.DistCheckedItems, eachObj))
                {
                    Instantiate(Prefabs[eachObj.PrefabIndex], new Vector3(eachObj.X, eachObj.Y, eachObj.Z), Quaternion.identity);
                }
            }
        }

        StartCoroutine("autoSave");
    }

    /// <summary>
    /// Spawns a new object based on the prefabs specified in the editor.  Instructs the Player to grab the new
    /// full set of items so that the newly instantiated object will be included in the distance check.  Ensures
    /// all newly spawned objects are saved to persistent storage.
    /// </summary>
    /// <param name="context">Context for the button press linked to this method so we only execute it once per press.</param>
    /// <remarks>_player should never be null when SpawnItem is called. However, this is something that can
    /// occasionally occur when updating objects in the scene during development while ExecuteInEditMode is active,
    /// so we can add the UNITY_EDITOR directive to remove this code from production build but leave it
    /// while we continue to develop the project.
    /// </remarks>
    public void SpawnItem(InputAction.CallbackContext context)
    {
        // Check for started state so that we only spawn one object per button press
        if(context.started)
        {
            // Randomize the next object type that will be spawned to keep things interesting
            int prefabIndex = Random.Range(0, Prefabs.Length);

            // Spawn the object at a random location on the ground in our Scene
            Vector3 generatedVec = new Vector3(Random.Range(xMin, xMax), 1, Random.Range(zMin, zMax));
            Instantiate(Prefabs[prefabIndex], generatedVec, Quaternion.identity);

            // Add necessary data to serialization list so we can recreate the game state on bootup
            _persistentData.SerializableObjects.Add(new Spawnable() { PrefabIndex = prefabIndex, X = generatedVec.x, Y = generatedVec.y, Z = generatedVec.z });

#if UNITY_EDITOR
            if (_player == null)
            {
                _player = getPlayerScript();
            }
#endif
            _player.DistCheckedItems = _player.GrabItems();

            // Ensure we save data persistently whenever a new item is spawned.
            save(_persistentData, RELATIVE_PATH);
        }
    }

    /// <summary>
    /// Obtains the Player script component so we can access its public methods.
    /// </summary>
    /// <returns>The Player script component.</returns>
    private Player getPlayerScript()
    {
        if (PlayerObj == null)
        {
            PlayerObj = GameObject.FindGameObjectWithTag("Player");
        }

        return PlayerObj.GetComponent<Player>();
    }

    /// <summary>
    /// Checks if there is already a game item at the spawnable location specified.
    /// </summary>
    /// <param name="gameItems">An array of all items to check against the position.</param>
    /// <param name="pos">The specified spawnable position.</param>
    /// <returns>True if there is an object at the specified position. False otherwise.</returns>
    private bool IsObjectAtPosition(Player.GameItem[] gameItems, Spawnable pos)
    {
        for(int i = 0; i < gameItems.Length; ++i)
        {
            if(gameItems[i].tr.position.x == pos.X && gameItems[i].tr.position.y == pos.Y && gameItems[i].tr.position.z == pos.Z)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Infinitely looping coroutine that is set to autoSave our
    /// persistent data every five seconds.
    /// </summary>
    private IEnumerator autoSave()
    {
        for(;;)
        {
            updatePlayerPersistentData();
            yield return new WaitForSeconds(5f);
        }
    }

    /// <summary>
    /// Updates all the player metrics we are tracking, namely position and rotation,
    /// and stores all persistent data for later retrieval.
    /// </summary>
    private void updatePlayerPersistentData()
    {
        if(_persistentData != null && PlayerObj != null)
        {
            _persistentData.PlayerX = PlayerObj.transform.position.x;
            _persistentData.PlayerY = PlayerObj.transform.position.y;
            _persistentData.PlayerZ = PlayerObj.transform.position.z;

            _persistentData.RotationX = PlayerObj.transform.rotation.x;
            _persistentData.RotationY = PlayerObj.transform.rotation.y;
            _persistentData.RotationZ = PlayerObj.transform.rotation.z;
            _persistentData.RotationW = PlayerObj.transform.rotation.w;

            // Ensure we save data persistently whenever the player update takes place.
            save(_persistentData, RELATIVE_PATH);
        }
    }

    /// <summary>
    /// Serializes the spawned objects and player data to the persistent data path for retrieval on the next game load.
    /// </summary>
    /// <param name="saveObject">Our persistent data object.</param>
    /// <param name="relativePath">Relative path to append to the persistent data path.</param>
    private void save(PersistentData saveObject, string relativePath)
    {
        var fullPath = Application.persistentDataPath + relativePath;

        FileStream fs = new FileStream(fullPath, FileMode.Create);
        
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, saveObject);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    /// <summary>
    /// Deserializes and retrieves the spawned objects and player datafrom persistent storage.
    /// </summary>
    /// <param name="relativePath">Relative path to append to the persistent data path.</param>
    /// <returns>Our persistent data.</returns>
    private PersistentData load(string relativePath)
    {
        var fullPath = Application.persistentDataPath + relativePath;
        PersistentData loadedObject = null;

        if (!File.Exists(fullPath))
        {
            return null;
        }

        FileStream fs = new FileStream(fullPath, FileMode.Open);

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            loadedObject = (PersistentData)formatter.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }

        return loadedObject;
    }
}
