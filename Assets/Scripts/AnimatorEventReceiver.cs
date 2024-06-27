using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventReceiver : MonoBehaviour
{
    public event Action OnAnimationStartEvent;
    public event Action OnAnimationTriggerEvent;
    public event Action OnAnimationEndEvent;
    
    void OnAnimationStart() 
    {
        OnAnimationStartEvent?.Invoke();
    }   
    
    void OnAnimationTrigger() 
    {
        OnAnimationTriggerEvent?.Invoke();
    }   

    void OnAnimationEnd() 
    {
        OnAnimationEndEvent?.Invoke();
    }   
    
    
}
