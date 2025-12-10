using System;
using UnityEngine;

public abstract class Popup : Overlay
{
    
    public OverlayAnimationData closeAnimationData = new OverlayAnimationData();
    
    public virtual void Close()
    {
        if (IsClosing)
        {
            Debug.Log("multiple close");
            return;
        }

        IsClosing = true;

        ClientCoordinator.Instance.ClosePopup(this);
    }

    public virtual void Close(Action onClosed)
    {
        if (IsClosing)
        {
            Debug.Log("multiple close");
            return;
        }

        IsClosing = true;

        ClientCoordinator.Instance.ClosePopup(this, onClosed);
    }
}
