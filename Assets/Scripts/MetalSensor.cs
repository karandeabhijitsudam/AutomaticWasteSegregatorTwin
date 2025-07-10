// MetalSensor.cs
using UnityEngine;

/// <summary>
/// Encapsulates metal detection logic.
/// Replace tag-based logic with hardware integration as needed.
/// </summary>
public class MetalSensor : MonoBehaviour
{
    /// <summary>
    /// Returns true if the waste GameObject is metal.
    /// </summary>
    public bool IsMetal(GameObject waste)
    {
        // Placeholder implementation: tag-based
        return waste.gameObject.tag.Contains("Waste_Metal");
    }
}