using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] //si no ponemos esto el postprocesado solo se verá en Play mode 
public class ApplyPostproc : MonoBehaviour
{
    public Material material;

    // OnRenderImage se invoca cada vez que un frame se renderiza en la camara que lleva el script
    // Con esto "interceptamos" el frame para editarlo antes de que se renderice
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}
