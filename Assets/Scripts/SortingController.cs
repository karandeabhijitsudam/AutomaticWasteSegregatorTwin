// SortingController.cs
using UnityEngine;
using TMPro;



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

    [Tooltip("UI Text to display current waste type")]
    public TMP_Text wasteTypeText;  // if using TextMeshPro

    // State for the current item
    private GameObject _currentWaste;
    private WasteType  _currentType;
    private string wasteType;

    
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
                wasteType = "Metal Waste";
                
                firstFlapper.Eject(waste, "first");
                Debug.Log("[SortingController] Metal waste ejected");
            }
            else
            {
                Debug.Log("[SortingController] Non-metal waste, continue to Pos2");
                // Resume belt in both cases
                belt.StartMoving();

            }

            wasteTypeText.text = $"Waste Type: "+ wasteType;

            
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
                wasteType = "Dry Waste";
            }
            else
            {
                Debug.Log("[SortingController] Wet detected — heading to end bin");
                wasteType = "Wet Waste";
                
            }

            // Subscribe to be notified when Raise finishes
            box.RaisedComplete += OnBoxRaised;
            box.Raise();
            wasteTypeText.text = $"Waste Type: " + wasteType;

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
        }
    }

    private void Reset()
    {
        belt.ResetPosition();
        belt.gameObject.SetActive(true);
        Debug.Log("[Controller] Reset to Idle");
    }


    public void ResetProcess()
    {
        Debug.Log("[Controller] Resetting process…");

        if (_currentWaste != null)
        {
            _currentWaste.transform.position = new Vector3(-0.85f, 1.1f, 0f);
            _currentWaste.transform.rotation = Quaternion.identity;

            var rb = _currentWaste.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false; // re-enable physics
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

        }

        if (wasteTypeText != null)
            wasteTypeText.text = "Waste Type: ";

        belt.ResetPosition();
        belt.gameObject.SetActive(true);

        _currentWaste = null;
        _currentType = WasteType.Unknown;
        wasteType = "";

        Debug.Log("[Controller] Process reset complete.");
    }
}