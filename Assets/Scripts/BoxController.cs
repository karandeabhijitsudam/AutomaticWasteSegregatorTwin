// BoxController.cs
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
public class BoxController : MonoBehaviour
{
    private Animator _anim;

    /// <summary>
    /// Fired when the Raise animation finishes (via Animation Event).
    /// </summary>
    public event Action RaisedComplete;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Lowers the box to enclose the waste.
    /// </summary>
    public void Lower()
    {
        Debug.Log("[BoxController] Lower()");
        _anim.SetTrigger("lowerBox");
    }

    /// <summary>
    /// Raises the box back up.
    /// </summary>
    public void Raise()
    {
        Debug.Log("[BoxController] Raise()");
        _anim.SetTrigger("riseBox");
    }

    // <-- Add this method and hook it up as an Animation Event
    /// <summary>
    /// Call this via an Animation Event on the final frame of your "Raise" clip.
    /// </summary>
    public void OnRaiseComplete()
    {
        Debug.Log("[BoxController] Raise animation complete");
        RaisedComplete?.Invoke();
    }
}
