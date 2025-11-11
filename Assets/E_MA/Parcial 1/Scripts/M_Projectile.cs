using UnityEngine;

[System.Serializable]
public class M_Projectile
{
    public int Damage = 10;
    public float Speed = 10f;
    public Vector3 Direction = Vector3.forward;
    public GameObject Owner = null;
}
