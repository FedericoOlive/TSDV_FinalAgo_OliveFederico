using UnityEngine;
using System;
using System.Collections;
using Unity.Mathematics;

public class Player : MonoBehaviour, IDamageable
{
    public Action Shooted;
    public Action Reloaded;
    public Action Die;
    public Transform bulletGroup;
    [Serializable] public class Tank
    {
        public GameObject tank;
        public GameObject cannon;
        public GameObject firePoint;
        public GameObject PfBullet;
    }
    [Serializable] public class Settings
    {
        public int life = 100;
        public float speedTankMovement;
        public float rotateTank;
        public float gravityTankRotate;
        public float rotateCannon;
        public float bulletForce;
        public int initialBullets;
        public int damageBullet;
        public float fuelConsumptionMovement;
        public float fuelConsumptionRotation;
        public float maxFuel;
    }
    [SerializeField] private Tank tank;
    [SerializeField] private Settings settings;
    public LayerMask objetivesLayerMask;
    public float tankSeparateTerrain = 0.5f;
    public float rateFire;

    private float rateFireTime;
    private bool shooting;
    private bool reloaded;
    private int bullets;
    public float fuel;

    public float GetFuel() { return fuel; }
    public float GetMaxFuel() { return settings.maxFuel; }
    public int GetBullets() { return bullets; }
    public float GetRateFireTime() { return rateFireTime; }

    void Start()
    {
        settings.fuelConsumptionMovement /= 100;
        settings.fuelConsumptionRotation /= 100;
        
        fuel = settings.maxFuel;
        reloaded = true;
        rateFireTime = rateFire;
        bullets = settings.initialBullets;
    }
    private void Update()
    {
        TryShoot();
    }
    void FixedUpdate()
    {
        Movement();
        Rotation();
        AutoRotation();
        FixHeightOnTerrain();
    }
    void TryShoot()
    {
        if (!shooting)
            rateFireTime += Time.deltaTime;
        if (rateFireTime > rateFire)
        {
            if (!reloaded)
            {
                // Evento recargado
            }
            reloaded = true;
            if (Input.GetMouseButtonDown(0) && !shooting && reloaded)
            {
                if (bullets > 0)
                {
                    bullets--;
                    rateFireTime = 0;
                    reloaded = false;
                    shooting = true;
                    Shooted?.Invoke();
                    Aim();
                }
                else
                {
                    // No hay Bullets
                }
            }
        }
    }
    private void Aim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.black);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5000, objetivesLayerMask))
        {
            StartCoroutine(Shooting(hit.point));
        }
        else
        {
            rateFireTime = rateFire;
            reloaded = true;
            shooting = false;
        }
    }
    IEnumerator Shooting(Vector3 point)                         //todo: clampear la rotacion en z a 0 y de x entre 9 y -20
    {
        Transform cannonT = tank.cannon.transform;
        Quaternion rotateDestiny = Quaternion.LookRotation(point - cannonT.position, cannonT.up);
        
        //Debug.Log("Rotacion de Destino: " + rotateDestiny.eulerAngles);
        rotateDestiny = FixRotateDestiny(rotateDestiny, cannonT);

        while (cannonT.rotation != rotateDestiny)
        {
            rotateDestiny = Quaternion.LookRotation(point - cannonT.position, cannonT.up);
            rotateDestiny = FixRotateDestiny(rotateDestiny, cannonT);

            cannonT.rotation = Quaternion.RotateTowards(cannonT.rotation, rotateDestiny, settings.rotateCannon * Time.deltaTime);
            yield return null;
        }
        //Debug.Log("Final: " + cannonT.rotation);
        Shoot(point);
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
        shooting = false;
        if (!bulletGroup)
        {
            bulletGroup = GameObject.Find("BulletGroup_Ref").transform;
            Debug.LogWarning("bulletGroup no estaba asignado", gameObject);
        }

        GameObject bullet = Instantiate(tank.PfBullet, tank.firePoint.transform.position, tank.cannon.transform.rotation, bulletGroup);
        Vector3 direction = point - bullet.transform.position;
        bullet.GetComponent<Rigidbody>().AddForce(direction.normalized * settings.bulletForce, ForceMode.Impulse);
        bullet.GetComponent<Bullet>().damage = settings.damageBullet;
    }
    void Movement()
    {
        if (fuel > 0)
        {
            float ver = Input.GetAxisRaw("Vertical");
            float multiply = (ver > 0) ? 1 : 2;

            transform.Translate(Vector3.forward * ((settings.speedTankMovement / multiply) * ver));
            fuel -= Mathf.Abs(ver * (settings.fuelConsumptionMovement / multiply));
        }
    }
    void Rotation()
    {
        if (fuel > 0)
        {
            float hor = Input.GetAxisRaw("Horizontal");

            transform.Rotate(Vector3.up, settings.rotateTank * hor);
            fuel -= Mathf.Abs(hor * settings.fuelConsumptionRotation);
        }
    }
    void AutoRotation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Quaternion qTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, qTo, settings.gravityTankRotate * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, qTo, settings.rotateTank * Time.deltaTime);
        }
    }
    void FixHeightOnTerrain()
    {
        Vector3 newpos = transform.position;
        newpos.y = Terrain.activeTerrain.SampleHeight(transform.position) + tankSeparateTerrain;
        transform.position = newpos;
    }

    public void TakeDamage(int damage)
    {
        settings.life -= damage;
        if (settings.life <= 0)
        {
            Die?.Invoke();
            // todo Evento Morir.
        }
    }
}