using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public enum AvailableProjectiles { PlayerLight};
    public AvailableProjectiles ProjectileType;
    [SerializeField] float projectileSpeed = 5f, projectileDuration = 4f;

    private void Start()
    {
        Destroy(gameObject,projectileDuration);
    }
    private void Update()
    {
        transform.Translate(0, 0, 1 * Time.deltaTime * projectileSpeed);
    }
}
