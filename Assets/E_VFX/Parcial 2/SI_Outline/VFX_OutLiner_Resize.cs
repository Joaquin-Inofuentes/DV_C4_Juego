using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_OutLiner_Resize : MonoBehaviour
{
    [Button(nameof(Start))]
    public Vector2 Resolucion = Vector2.zero;
    public RenderTexture ResolucionTexture;

    public RenderTexture ResolucionBajada;
    // Update is called once per frame
    void Start()
    {
        if(ResolucionBajada != null)
        {
            ResolucionBajada.height = ResolucionTexture.height;
            ResolucionBajada.width = ResolucionTexture.width;
            return;
        }

        if (Resolucion != new Vector2(Screen.width, Screen.height))
        {
            if (Screen.width > 0 && Screen.height > 0)
            {
                Resolucion = new Vector2(Screen.width, Screen.height);

                ResolucionTexture.width = Screen.width;
                ResolucionTexture.height = Screen.height;
            }
        }
    }
}
