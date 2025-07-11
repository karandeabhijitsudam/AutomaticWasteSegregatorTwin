// SortingController.cs
using UnityEngine;

public class SortingController : MonoBehaviour
{
    [Tooltip("Drag your ConveyorBelt component here")]
    public ConveyorBelt belt;

    [Tooltip("Drag your Flapper component here")]
    public Flapper flapper;

    [Tooltip("Drag your MetalSensor component here")]
    public MetalSensor metalSensor;

    /// <summary>
    /// Called by any ProximitySensor when it sees Waste_*.
    /// </summary>
    public void OnWasteDetected(Position pos, GameObject waste)
    {
        Debug.Log($"[SortingController] Detected {waste.name} at {pos}");

        if (pos == Position.Pos1)
        {
            belt.StopMoving();
            Debug.Log("[SortingController] Belt stopped at Pos1");

            // Use MetalSensor to branch
            if (metalSensor.IsMetal(waste))
            {
                Debug.Log("[SortingController] Metal detected");
                flapper.Eject(waste);
                Debug.Log("[SortingController] Metal waste ejected");
            }
            else
            {
                Debug.Log("[SortingController] Non-metal waste, continue to Pos2");
                // Resume belt in both cases
                belt.StartMoving();

            }

            
        }

        if (pos == Position.Pos2)
        {
            belt.StopMoving();
            Debug.Log("[SortingController] Belt stopped at Pos2");
        }


    }
}