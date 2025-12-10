using System;
using UnityEngine;

public abstract class Overlay : MonoBehaviour
{
    
    public OverlayAnimationData openAnimationData = new ();

    
    [SerializeField]
    public GameObject content;

    [SerializeField]
    public bool closeWithBackButton = true;
    
    protected bool IsClosing = false;
    
}
