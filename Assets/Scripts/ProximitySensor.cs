// ProximitySensor.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProximitySensor : MonoBehaviour
{
    [Tooltip("Which stop is this sensor?")]
    public Position position;

    [Tooltip("Drag your SortingController here")]
    public SortingController controller;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Waste"))
        {
            controller.OnWasteDetected(position, other.gameObject);
        }
    }
}