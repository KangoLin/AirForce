using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool<T> : MonoBehaviour where T : ProjectileBase
{
    public static ProjectilePool<T> Instance { get; private set; }

    [SerializeField] private T projectilePrefab;
    [SerializeField] private int initialPoolSize = 20;

    private Queue<T> pool = new Queue<T>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // [!code ++]
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewProjectile();
        }
    }

    public T Get()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning($"Pool empty, creating new {typeof(T).Name}");
            CreateNewProjectile();
        }

        T projectile = pool.Dequeue();
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void Return(T projectile)
    {
        if (projectile == null) return;

        projectile.gameObject.SetActive(false);
        pool.Enqueue(projectile);
    }

    private void CreateNewProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab not set!");
            return;
        }

        T p = Instantiate(projectilePrefab, transform);
        p.gameObject.SetActive(false);
        pool.Enqueue(p);
    }
}