// Flapper.cs
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Flapper : MonoBehaviour
{
    Animator _anim;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Play the eject animation, then destroy the waste.
    /// </summary>
    public void Eject(GameObject waste)
    {
        Debug.Log($"[Flapper] Ejecting {waste.name}");
        _anim.SetTrigger("Eject");
        Destroy(waste, 1f);
    }
}