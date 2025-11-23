using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [Header("Pool Config")]
    public Enemy enemyPrefab;
    public int cantidadInicial = 20;

    private Queue<Enemy> pool = new Queue<Enemy>();

    void Awake()
    {
        Instance = this;
        CrearPoolInicial();
    }

    void CrearPoolInicial()
    {
        for (int i = 0; i < cantidadInicial; i++)
        {
            Enemy e = Instantiate(enemyPrefab);
            e.gameObject.SetActive(false);
            pool.Enqueue(e);
        }
    }

    public Enemy GetEnemy(Vector3 pos, Quaternion rot)
    {
        Enemy enemigo;

        if (pool.Count > 0)
        {
            enemigo = pool.Dequeue();
        }
        else
        {
            enemigo = Instantiate(enemyPrefab);
        }

        enemigo.transform.position = pos;
        enemigo.transform.rotation = rot;
        enemigo.gameObject.SetActive(true);

        return enemigo;
    }

    public void ReturnEnemy(Enemy e)
    {
        e.gameObject.SetActive(false);
        pool.Enqueue(e);
    }
}