using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Mouse : MonoBehaviour
{
    public C_InputManager InputManager;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            InputManager.InvokeDisparar();
        }
    }
}
