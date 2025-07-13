// SortingController.cs
using UnityEngine;

public class SortingController : MonoBehaviour
{
    [Tooltip("Drag your ConveyorBelt component here")]
    public ConveyorBelt belt;

    [Tooltip("Drag your Flapper component here")]
    public Flapper firstFlapper;   // for Pos1
    public Flapper secondFlapper;  // for Pos3

    [Tooltip("Drag your MetalSensor component here")]
    public MetalSensor metalSensor;

    [Tooltip("Drag your BoxController component here")]
    public BoxController box; 

    [Tooltip("Drag your HumiditySensor component here")]
    public HumiditySensor humiditySensor;  

    // State for the current item
    private GameObject _currentWaste;
    private WasteType  _currentType;

    


    void OnEnable()
    {
        belt.OnEndReached += HandleEndReached;
    }

    void OnDisable()
    {
        belt.OnEndReached -= HandleEndReached;
    }

    /// <summary>
    /// Called by any ProximitySensor when it sees Waste_*.
    /// </summary>
    public void OnWasteDetected(Position pos, GameObject waste)
    {
        Debug.Log($"[SortingController] Detected {waste.name} at {pos}");

        // POS1: metal vs non-metal
        if (pos == Position.Pos1 && _currentWaste == null)
        {
            _currentWaste = waste;

            belt.StopMoving();
            Debug.Log("[SortingController] Belt stopped at Pos1");

            bool isMetal = metalSensor.IsMetal(waste);
            _currentType = isMetal ? WasteType.Metal : WasteType.Wet; 

            // Use MetalSensor to branch
            if (isMetal)
            {
                Debug.Log("[SortingController] Metal detected");
                firstFlapper.Eject(waste, "first");
                Debug.Log("[SortingController] Metal waste ejected");
                Reset();
            }
            else
            {
                Debug.Log("[SortingController] Non-metal waste, continue to Pos2");
                // Resume belt in both cases
                belt.StartMoving();

            }

            
        }
        // POS2: dry vs wet (only for non-metal)
        else if (pos == Position.Pos2 && waste == _currentWaste && _currentType != WasteType.Metal)
        {
            belt.StopMoving();
            Debug.Log("[SortingController] Belt stopped at Pos2");

            box.Lower();
            
            bool isDry = humiditySensor.IsDry(waste);
            _currentType = isDry ? WasteType.Dry : WasteType.Wet;

            if (isDry)
            {
                Debug.Log("[SortingController] Dry detected — heading to Pos3");
                //belt.StartMoving();
            }
            else
            {
                Debug.Log("[SortingController] Wet detected — heading to end bin");
                //belt.StartMoving(); 
                // you can catch PosEnd in its own branch later
            }

            // Subscribe to be notified when Raise finishes
            box.RaisedComplete += OnBoxRaised;
            box.Raise();
            //belt.StartMoving();
        }
        // POS3: only stop if it’s dry
        else if (pos == Position.Pos3 && waste == _currentWaste && _currentType == WasteType.Dry)
        {
            belt.StopMoving();
            Debug.Log("[SortingController] Belt stopped at Pos3");

            secondFlapper.Eject(waste, "second");

        }


    }

    private void OnBoxRaised()
    {
        // Unsubscribe, so it's only fired once
        box.RaisedComplete -= OnBoxRaised;

        Debug.Log("[SortingController] Box raise complete — restarting belt");
        belt.StartMoving();

        // if (_currentType == WasteType.Dry)
        // {
        //     // after dry eject we should clean up
        //     Reset();
        // }
        // if wet, Reset() could also be called when it leaves the End sensor
    }

    private void HandleEndReached()
    {
        // only drop if it's the same item and it was wet
        if (_currentType == WasteType.Wet && _currentWaste != null)
        {
            Debug.Log("[Controller] End of belt for wet waste → releasing");
            var rb = _currentWaste.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            // turn off the belt gameobject so it no longer carries the waste
            belt.gameObject.SetActive(false);

            Reset();
        }
    }

    private void Reset()
    {
        _currentWaste = null;
        belt.ResetPosition();
        belt.gameObject.SetActive(true);
        Debug.Log("[Controller] Reset to Idle");
    }


}