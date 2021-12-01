using System.Collections;
using UnityEngine;

/* This class is to be used within a list or array so that each element in that list/array
 * is an object of this class. Each element will therefore contain one Object, and a Double
 * value associated with it.
 */

// Make this class Serializable.
[System.Serializable]
public class WeightedObject
{
    #region Fields
    [Tooltip("The object selected by this choice.")]
    public GameObject value;

    [Tooltip("The chance of selecting this value.")]
    public float chance = 1.0f;
    #endregion Fields


    #region Methods
    // LEAVE EMPTY
    #endregion Methods
}
