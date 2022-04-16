using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controls functional goals related to the Player Character.  In this game,
/// the player's primary responsibility is to check its distance against special tagged
/// objects in the Scene and highlight the closest object to itself in a special color.
/// </summary>
/// <remarks>
/// ExecuteInEditMode Attribute has been added to ensure the changing color
/// functionality can be tested without entering play mode as well.
/// </remarks>
[ExecuteInEditMode]
public class Player : MonoBehaviour
{
    /// <summary>
    /// Inner class used to organize properties of tagged
    /// items in the Scene.
    /// </summary>
    public class GameItem
    {
        /// <summary>
        /// Gets or sets the game object associated with the tagged item.
        /// </summary>
        public GameObject obj { get; set; }

        /// <summary>
        /// Gets or sets the transform associated with the tagged item.
        /// </summary>
        public Transform tr { get; set; }

        /// <summary>
        /// Gets or sets the renderer associated with the tagged item.
        /// </summary>
        public Renderer rend { get; set; }

        /// <summary>
        /// Gets or sets the distance script associated with the tagged item.
        /// </summary>
        public IDistanceChecked script { get; set; }
    }

    /// <summary>
    /// Field exposed to the Unity editor which allows users to update all of the
    /// tags for items in the scene that they want to search through which are to
    /// be distance checked items.
    /// </summary>
    public string[] TagsToCheckDistance;

    /// <summary>
    /// Stores all of the tagged items we will be checking distance on in the Scene.
    /// </summary>
    private GameItem[] _distCheckedItems;

    /// <summary>
    /// Gets or sets the items to check distance on in the Scene.
    /// </summary>
    public GameItem[] DistCheckedItems
    {
        get
        {
            return _distCheckedItems;
        }
        set
        {
            _distCheckedItems = value;
        }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _distCheckedItems = GrabItems();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    /// <remarks>_distCheckedItems should never be null by the first call to update since
    /// it is populated during Start(). However, this is something that can occasionally occur when updating
    /// objects in the scene during development while ExecuteInEditMode is active, so we can
    /// add the UNITY_EDITOR directive to remove this code from production build but leave it
    /// while we continue to develop the project.
    /// </remarks>
    void Update()
    {
#if UNITY_EDITOR
        if (_distCheckedItems == null)
        {
            _distCheckedItems = GrabItems();
        }
#endif
        UpdateItemColors();
    }

    /// <summary>
    /// Gets all of the tagged and restructured distance check items in the Scene
    /// for easy processing.
    /// </summary>
    /// <returns>The array of tagged GameItems to check distance on.</returns>
    public GameItem[] GrabItems()
    {
        List<GameObject> taggedObjects = new List<GameObject>();

        // Add all tagged objects to list based on the user-specified tags.
        foreach(var eachTag in TagsToCheckDistance)
        {
            var itemObjects = GameObject.FindGameObjectsWithTag(eachTag);
            taggedObjects.AddRange(itemObjects.ToList());
        }

        // The size of the array will not change in our game and is equal in size
        // to the selected tagged objects in our Scene.
        GameItem[] taggedItems = new GameItem[taggedObjects.Count];

        // Generate our nicely structured tagged items for easier processing later.
        for (int i = 0; i < taggedObjects.Count; ++i)
        {
            taggedItems[i] = new GameItem() { obj = taggedObjects[i], tr = taggedObjects[i].transform, rend = taggedObjects[i].GetComponent<Renderer>(), script = taggedObjects[i].GetComponent<IDistanceChecked>()};
        }

        return taggedItems;
    }

    /// <summary>
    /// Finds and returns the GameItem that is nearest to our Player's current position.
    /// </summary>
    /// <param name="items">An array of all items to check distance against the Player's.</param>
    /// <returns>The nearest tagged GameItem to our Player.</returns>
    private GameItem getNearestItem(GameItem[] items)
    {
        // Since we'll be using a greedy algorithm here, set the nearest distance
        // to infinity and declare a null game item to start.
        GameItem nearest = null;
        float nearestDistSqr = Mathf.Infinity;

        // Player's current position
        Vector3 currentPos = transform.position;

        // Iterate all tagged items to be distance checked
        for (int i = 0; i < items.Length; ++i)
        {
            // We only need squared magnitude and don't need to waste time with the 
            // expensive square root operation that happens in Distance methods.
            float distSquared = (items[i].tr.position - currentPos).sqrMagnitude;
            if (distSquared < nearestDistSqr)
            {
                // Update our nearest item and distance in the greedy fashion.
                nearest = items[i];
                nearestDistSqr = distSquared;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Changes the color of the closest tagged object to the player's current position
    /// to a special color and sets the rest of the tagged objects to their chosen base color.
    /// </summary>
    public void UpdateItemColors()
    {
        GameItem nearest = getNearestItem(_distCheckedItems);
        for (int i = 0; i < _distCheckedItems.Length; ++i)
        {
            if (nearest == _distCheckedItems[i])
            {
                // Set our nearest item to the special closest item color.
                nearest.rend.material.SetColor("_Color", nearest.script.ClosestColor);
                continue;
            }

            // Set every other item that is not nearest to the Player as their default/base color.
            _distCheckedItems[i].rend.material.SetColor("_Color", _distCheckedItems[i].script.DefaultColor);
        }
    }
}
