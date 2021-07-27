using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
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
        public float speedTankMovement;
        public float rotateTank;
        public float gravityTankRotate;
        public float rotateCannon;
        public float bulletForce;
        public int initialBullet;
    }
    [SerializeField] private Tank tank;
    [SerializeField] private Settings settings;

    public float tankSeparateTerrain = 0.5f;
    public float rateFire;
    private float rateFireTime;
    private bool shooting;
    private bool reloaded;

    void Start()
    {
        reloaded = true;
        rateFireTime = rateFire;
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
                rateFireTime = 0;
                reloaded = false;
                shooting = true;
                Aim();
            }
        }
    }
    private void Aim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.black);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            StartCoroutine(Shooting(hit.point));
        }
    }
    IEnumerator Shooting(Vector3 point)                         //todo: clampear la rotacion en z a 0 y de x entre 9 y -20
    {
        Transform cannonT = tank.cannon.transform;
        Quaternion rotateDestiny = Quaternion.LookRotation(point - cannonT.position, cannonT.up);
        
        float posX = Mathf.Clamp(rotateDestiny.eulerAngles.x, -20, 9);
        rotateDestiny = Quaternion.Euler(posX, rotateDestiny.eulerAngles.y, 0);
        Debug.Log("Clamped: " + rotateDestiny);

        while (cannonT.rotation != rotateDestiny)
        {
            cannonT.rotation = Quaternion.Euler(cannonT.eulerAngles.x, cannonT.eulerAngles.y, 0);// testear, sino sacar del while (abajo)
            rotateDestiny = Quaternion.LookRotation(point - cannonT.position, cannonT.up);
            cannonT.rotation = Quaternion.RotateTowards(cannonT.rotation, rotateDestiny, settings.rotateCannon * Time.deltaTime);
            yield return null;
        }
        Shoot();
    }

    void Shoot()
    {
        shooting = false;
        if (!bulletGroup)
        {
            bulletGroup = GameObject.Find("BulletGroup_Ref").transform;
            Debug.LogWarning("bulletGroup no estaba asignado", gameObject);
        }
        GameObject bullet = Instantiate(tank.PfBullet, tank.firePoint.transform.position, tank.cannon.transform.rotation, bulletGroup);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * settings.bulletForce, ForceMode.Impulse);
    }
    void Movement()
    {
        float ver = Input.GetAxisRaw("Vertical");
        transform.Translate(Vector3.forward * (settings.speedTankMovement * ver));
        transform.Translate(Vector3.forward * (-settings.speedTankMovement / 2 * ver));
    }
    void Rotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -settings.rotateTank);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, settings.rotateTank);
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
}