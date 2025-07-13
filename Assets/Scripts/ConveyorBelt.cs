using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ConveyorBelt : MonoBehaviour
{
    [Header("Belt Settings")]
    [Tooltip("Speed (units/sec) to move the belt when running.")]
    public float beltSpeed = 2f;

    [Tooltip("Local direction the belt moves objects.")]
    public Vector3 beltDirection = Vector3.forward;

    [Header("End-of-Line")]
    [Tooltip("World units from start at which waste should fall off")]
    public float conveyorLength = 1.25f;

    // Fired when the belt has moved ≥ conveyorLength while running
    public event Action OnEndReached;

    Rigidbody _rb;
    Vector3   _worldDir;
    bool      _isMoving = false;
    private float     _movedDistance;
    private Vector3   _startPos;

    void Awake()
    {
        _rb             = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        GetComponent<Collider>().isTrigger = false;

        // precompute the world-space direction
        _worldDir = transform.TransformDirection(beltDirection.normalized);
        _startPos    = transform.position;
    }

    /// <summary>Begin continuous belt motion.</summary>
    public void StartMoving()
    {
        if (!_isMoving)
        {
            _isMoving = true;
            _movedDistance = 0f;   // reset counter whenever we start
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

        // Accumulate how far we've gone
        _movedDistance += step;
        if (_movedDistance >= conveyorLength)
        {
            // We've reached the end!
            _isMoving = false;
            Debug.Log("[ConveyorBelt] End reached");
            OnEndReached?.Invoke();
        }
    }

    /// <summary>
    /// Teleports the belt back to its original start position
    /// and clears any distance‐tracking.
    /// </summary>
    public void ResetPosition()
    {
        transform.position  = _startPos;
        _movedDistance      = 0f;
        Debug.Log("[ConveyorBelt] ResetPosition()");
    }
}
