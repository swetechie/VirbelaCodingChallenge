using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to tagged items in the Scene which are to be distance checked
/// against the Player.
/// </summary>
/// <remarks>
/// ExecuteInEditMode Attribute has been added to ensure the changing color
/// functionality can be tested without entering play mode as well.
/// </remarks>
[ExecuteInEditMode]
public class Item : MonoBehaviour, IDistanceChecked
{
    /// <summary>
    /// Gets or sets the color to display when object is closest one to the Player.
    /// </summary>
    public Color ClosestColor { get; set; }

    /// <summary>
    /// Field to expose the special color for the closest object to the Player as an option to users in the Unity Editor.
    /// </summary>
    public Color SpecialColor;

    /// <summary>
    /// Gets or sets the default color of the object.
    /// </summary>
    public Color DefaultColor { get; set; }

    /// <summary>
    /// Field to expose the base color as an option to users in the Unity Editor.
    /// </summary>
    public Color BaseColor;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        ClosestColor = SpecialColor;
        DefaultColor = BaseColor;
    }
}
