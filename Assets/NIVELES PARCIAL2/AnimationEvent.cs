using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
   public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Método que podés llamar desde otra animación
    public void PlayAnim(string animName)
    {
        anim.Play(animName);
    }
}