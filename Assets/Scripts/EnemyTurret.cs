using System;
using UnityEngine;

public class EnemyTurret : MonoBehaviour, IDamageable
{
    public Action OnDie;
    public LayerMask damageableLayerMask;
    [Serializable] public class Turret
    {
        public GameObject turret;
        public GameObject bullet;
        public ObjectsRewards objReward;
        public GameObject goAim;
        public GameObject firePoint;
        public GameObject PfBullet;
        public int maxLife;
    }
    [Serializable] public class Settings
    {
        public float rateFire;
    }
    [SerializeField] private Turret turret;
    [SerializeField] private Settings settings;
    private bool reloaded;
    private bool shooting;
    private float rateFireTime;
    public int life;

    void Start()
    {
        
    }
    void Update()
    {
        Aim();
    }
    void Aim()
    {
        RaycastHit hit;
        Vector3 direction = turret.firePoint.transform.position - turret.goAim.transform.position;
        if (Physics.Raycast(turret.firePoint.transform.position, direction, out hit, 1000, damageableLayerMask))
        {
            Debug.DrawRay(hit.point, direction, Color.black);
            //StartCoroutine(Shooting(hit.point));
        }
        else
        {
            rateFireTime = settings.rateFire;
            reloaded = true;
            shooting = false;
        }
    }
    void Shoot()
    {

    }
    public void TakeDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            life = 0;
            return;
        }
    }
}