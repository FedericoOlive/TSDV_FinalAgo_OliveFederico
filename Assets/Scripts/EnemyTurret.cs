using System;
using UnityEngine;

public class EnemyTurret : MonoBehaviour, IDamageable
{
    public Action onDie;
    public LayerMask damageableLayerMask;
    public Transform bulletGroup;
    [Serializable] public class Turret
    {
        public GameObject turret;
        public GameObject PfBullet;
        public GameObject firePoint;
        public GameObject goAim;
       
    }
    [Serializable] public class Settings
    {
        public int maxLife;
        public float bulletForce;
        public int damageBullet;
        public float rateFire;
    }
    [SerializeField] private Turret turret;
    [SerializeField] private Settings settings;
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
        rateFireTime += Time.deltaTime;
        Vector3 objetive = turret.goAim.transform.position;
        Transform cannonT = turret.turret.transform;
        Quaternion rotateDestiny = Quaternion.LookRotation(objetive - cannonT.position, cannonT.up);

        //Debug.Log("Rotacion de Destino: " + rotateDestiny.eulerAngles);
        rotateDestiny = FixRotateDestiny(rotateDestiny, cannonT);

        rotateDestiny = Quaternion.LookRotation(objetive - cannonT.position, cannonT.up);
        rotateDestiny = FixRotateDestiny(rotateDestiny, cannonT);
        cannonT.rotation = Quaternion.RotateTowards(cannonT.rotation, rotateDestiny, 500 * Time.deltaTime);
        if (rateFireTime > settings.rateFire)
        {
            rateFireTime = 0;
            Shoot(objetive);
        }
    }
    Quaternion FixRotateDestiny(Quaternion rotateDestiny, Transform cannonT)
    {
        cannonT.localRotation = Quaternion.Euler(cannonT.localEulerAngles.x, cannonT.localEulerAngles.y, 0);

        float posX = rotateDestiny.eulerAngles.x;
        // Fix posX

        rotateDestiny = Quaternion.Euler(posX, rotateDestiny.eulerAngles.y, cannonT.rotation.eulerAngles.z);
        //Debug.Log("Rotacion Fixed: " + rotateDestiny.eulerAngles);
        return rotateDestiny;
    }
    void Shoot(Vector3 point)
    {
        if (!bulletGroup)
        {
            bulletGroup = GameObject.Find("BulletGroup_Ref").transform;
            Debug.LogWarning("bulletGroup no estaba asignado", gameObject);
        }
        GameObject bullet = Instantiate(turret.PfBullet, turret.firePoint.transform.position, turret.turret.transform.rotation, bulletGroup);
        Vector3 direction = point - bullet.transform.position;
        Debug.DrawRay(point, direction, Color.black, 2.0f);
        bullet.GetComponent<Rigidbody>().AddForce(direction.normalized * settings.bulletForce, ForceMode.Impulse);
        bullet.GetComponent<Bullet>().damage = settings.damageBullet;
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