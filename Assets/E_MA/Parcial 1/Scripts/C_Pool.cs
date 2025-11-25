using UnityEngine;

public class C_Pool : MonoBehaviour
{
    public M_Pool Model = new M_Pool();
    public F_Factory Factory;
    public string NombreDelProyectil = "proyectil";

    // Pide un objeto con posición
    public GameObject Request(Vector3 spawnPosition)
    {
        GameObject obj;

        if (Model.HasAvailable())
        {
            obj = Model.GetInactiveAt(0);
            Model.RemoveInactiveAt(0);
        }
        else
        {
            obj = Factory.Create(spawnPosition);
            obj.GetComponent<C_Projectile>().NombreDelProyectil = NombreDelProyectil;
            //Debug.Log("[Pool] Nuevo objeto creado por Factory");
        }

        obj.transform.position = spawnPosition;
        obj.transform.SetParent(transform);
        //Debug.Log("Se asocio como padre a " + transform.name, gameObject);
        obj.SetActive(true);

        Model.AddActive(obj);
        return obj;
    }

    // Devuelve objeto al pool
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        Model.RemoveActive(obj);
        Model.AddInactive(obj);
    }

    // Consultas
    public bool HasAvailable() => Model.HasAvailable();
    public int AvailableCount() => Model.AvailableCount();
}
