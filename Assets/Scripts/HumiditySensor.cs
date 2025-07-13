// HumiditySensor.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HumiditySensor : MonoBehaviour
{
    /// <summary>
    /// Returns true if the waste GameObject is tagged as dry.
    /// </summary>
    public bool IsDry(GameObject waste)
    {
        // Placeholder logic: we treat any tag containing "Waste_Dry" as dry
        return waste.tag.Contains("Dry");
    }
}
