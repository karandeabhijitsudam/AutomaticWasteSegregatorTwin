using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Flapper : MonoBehaviour
{
    Animator _anim;

    [Tooltip("Impulse force applied to the waste object")]
    public float pushForce = 5f;
    [Tooltip("Local direction in which to push the waste")]
    public Vector3 pushDirection = Vector3.right;

    // The waste we’re about to eject
    private GameObject _pendingWaste;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Called by SortingController to start the eject animation
    /// and register which object to push.
    /// </summary>
    public void Eject(GameObject waste)
    {
        Debug.Log($"[Flapper] Ejecting {waste.name}");
        _pendingWaste = waste;
        _anim.SetTrigger("firstFlapperON");
    }

    /// <summary>
    /// Physics callback: when our blade trigger touches the pending waste,
    /// push it off immediately.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Only care about the one we're ejecting
        if (other.gameObject == _pendingWaste)
        {
            PushWaste();
        }
    }

    /// <summary>
    /// Applies the impulse to knock the waste off the belt.
    /// </summary>
    private void PushWaste()
    {
        if (_pendingWaste == null) return;

        var rb = _pendingWaste.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;  // let physics take over

            // Simple: push along the waste’s own -Z axis
            Vector3 worldDir = -_pendingWaste.transform.forward;

            rb.AddForce(worldDir * pushForce, ForceMode.Impulse);
            Debug.Log($"[Flapper] Pushed {_pendingWaste.name} along its +Z with force {pushForce}");
        }

        _pendingWaste = null;
    }
}
