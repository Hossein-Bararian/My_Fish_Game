using System;
using UnityEngine;

public abstract class Panel : Overlay   
{
    
    public virtual void Close()
    {
        if (IsClosing)
        {
            Debug.Log("multiple close");
            return;
        }

        IsClosing = true;

        ClientCoordinator.Instance.ClosePanel(this);
    }

    public virtual void Close(Action onClosed)
    {
        if (IsClosing)
        {
            Debug.Log("multiple close");
            return;
        }

        IsClosing = true;

        ClientCoordinator.Instance.ClosePanel(this, onClosed);
    }
}
