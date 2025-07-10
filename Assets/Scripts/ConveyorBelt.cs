using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ConveyorBelt : MonoBehaviour
{
    [Header("Belt Settings")]
    [Tooltip("Speed (units/sec) to move the belt when running.")]
    public float beltSpeed = 2f;

    [Tooltip("Local direction the belt moves objects.")]
    public Vector3 beltDirection = Vector3.forward;

    Rigidbody _rb;
    Vector3   _worldDir;
    bool      _isMoving = false;

    void Awake()
    {
        _rb             = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        GetComponent<Collider>().isTrigger = false;

        // precompute the world-space direction
        _worldDir = transform.TransformDirection(beltDirection.normalized);
    }

    /// <summary>Begin continuous belt motion.</summary>
    public void StartMoving()
    {
        if (!_isMoving)
        {
            _isMoving = true;
            Debug.Log("[ConveyorBelt] StartMoving()");
        }
    }

    /// <summary>Halt the belt immediately.</summary>
    public void StopMoving()
    {
        if (_isMoving)
        {
            _isMoving = false;
            Debug.Log("[ConveyorBelt] StopMoving()");
        }
    }

    void FixedUpdate()
    {
        if (!_isMoving) return;

        // move at beltSpeed each FixedUpdate
        float step = beltSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + _worldDir * step);
    }
}
