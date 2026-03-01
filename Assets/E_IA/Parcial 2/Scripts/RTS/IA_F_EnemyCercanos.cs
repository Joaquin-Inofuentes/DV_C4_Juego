using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_F_EnemyCercanos : MonoBehaviour
{
    public List<GameObject> Colisionados;

    public void OnCollisionEnter(Collision collision)
    {
        Colisionados.Add(collision.gameObject);
    }

    public void OnCollisionExit(Collision collision)
    {
        Colisionados.Remove(collision.gameObject);
    }
}
