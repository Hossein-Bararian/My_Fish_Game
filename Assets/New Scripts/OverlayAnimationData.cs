using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OverlayAnimationData
{
    public bool useAnimation = false;
    public RectTransform[] elements;
    public float[] durations = new float[] { 0.3f };
    public LeanTweenType[] easeTypes = new LeanTweenType[] {LeanTweenType.easeOutQuart};
    public float delay = 0.1f;
    
}
