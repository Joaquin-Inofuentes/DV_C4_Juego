using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetonadorOnEnable : MonoBehaviour
{
    public UnityEvent OnEnabled;
    public UnityEvent OnDisabled;
    public void OnEnable()
    {
        OnEnabled.Invoke();
    }
    public void OnDisable()
    {
        OnDisabled.Invoke();
    }
}
