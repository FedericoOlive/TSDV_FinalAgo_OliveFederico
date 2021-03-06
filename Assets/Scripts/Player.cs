using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IDamageable, IRechargeFuel
{
    public Action onShooted;
    public Action onReloaded;
    public Action onDie;
    public Action onReceiveDamage;
    public Transform bulletGroup;
    [Serializable] public class Tank
    {
        public GameObject tank;
        public GameObject cannon;
        public GameObject firePoint;
        public GameObject PfBullet;
        public AudioSource soundShoot;
        public AudioSource soundReloaded;
        public AudioSource soundReloading;
    }
    [Serializable] public class Settings
    {
        public int maxLife = 100;
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
    public LayerMask limits;
    public LayerMask objetivesLayerMask;
    public float tankSeparateTerrain = 0.5f;
    public float rateFire;

    private float rateFireTime;
    private bool shooting;
    private bool reloaded;
    private float distanceFrontOffset = 5f;
    public int bullets;
    public float fuel;
    public float distance;
    public int life = 100;
    public int bulletsShooted;

    public float GetFuel() { return fuel; }
    public float GetMaxFuel() { return settings.maxFuel; }
    public int GetBullets() { return bullets; }
    public float GetRateFireTime() { return rateFireTime; }
    public float GetMaxLife() { return settings.maxLife; }

    void Start()
    {
        settings.fuelConsumptionMovement /= 100;
        settings.fuelConsumptionRotation /= 100;
        
        fuel = settings.maxFuel;
        reloaded = true;
        rateFireTime = rateFire;
        bullets = settings.initialBullets;
        life = settings.maxLife;
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
                if (!tank.soundReloaded.isPlaying)
                    tank.soundReloaded.Play();
            }
            reloaded = true;
            if (Input.GetMouseButtonDown(0))
            {
                if (!shooting && reloaded)
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (bullets > 0)
                        {
                            bullets--;
                            rateFireTime = 0;
                            reloaded = false;
                            shooting = true;
                            onShooted?.Invoke();
                            Aim();
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (!tank.soundReloading.isPlaying)
                    tank.soundReloading.Play();
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

        bulletsShooted++;
        GameObject bullet = Instantiate(tank.PfBullet, tank.firePoint.transform.position, tank.cannon.transform.rotation, bulletGroup);
        Vector3 direction = point - bullet.transform.position;
        bullet.GetComponent<Rigidbody>().AddForce(direction.normalized * settings.bulletForce, ForceMode.Impulse);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.damage = settings.damageBullet;
        bulletComponent.layersImpacts = objetivesLayerMask;
        if (!tank.soundShoot.isPlaying)
            tank.soundShoot.Play();
    }
    void Movement()
    {
        if (fuel > 0)
        {
            RaycastHit hit;
            
            float ver = Input.GetAxisRaw("Vertical");
            float multiply = (ver > 0) ? 1 : 2;
            distance += Mathf.Abs(ver) / multiply;
            Vector3 direction = ver > 0 ? transform.forward : -transform.forward;

            if (!Physics.Raycast(transform.position, direction, out hit, distanceFrontOffset, limits))
            {
                transform.Translate(Vector3.forward * ((settings.speedTankMovement / multiply) * ver));
                fuel -= Mathf.Abs(ver * (settings.fuelConsumptionMovement / multiply));
            }
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
        life -= damage;
        if (life <= 0)
        {
            life = 0;
            onDie?.Invoke();
        }
        else
        {
            onReceiveDamage?.Invoke();
        }
    }

    public void RechargeFuel(float value)
    {
        fuel += value;
    }
}