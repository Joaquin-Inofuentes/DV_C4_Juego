using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AccionAlDestruir : MonoBehaviour
{
    public UnityEvent onDestroyed;
    public void OnDestroy()
    {
        onDestroyed.Invoke();
    }
}
