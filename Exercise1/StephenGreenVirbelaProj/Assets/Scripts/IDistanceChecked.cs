using UnityEngine;

/// <summary>
/// Interface that all scripts attached to objects to be distance checked in the game must implement. All 
/// distance checked objects must have a base/default color to display when they are not
/// the closest item to the Player as well as a closest color to display if they are the closest.
/// </summary>
public interface IDistanceChecked
{
    /// <summary>
    /// Color to display when object is closest one to the Player.
    /// </summary>
    public Color ClosestColor { get; set; }

    /// <summary>
    /// Base color of the object.
    /// </summary>
    public Color DefaultColor { get; set; }
}
