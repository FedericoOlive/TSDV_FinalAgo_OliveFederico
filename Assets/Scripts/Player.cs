using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Assertions.Must;

public class Player : MonoBehaviour
{
    [Serializable]
    public class Tank
    {
        public GameObject tank;
        public GameObject cannon;
        public GameObject firePoint;
        public GameObject PfBullet;
    }

    [Serializable]
    public class Attributes
    {
        public float tankSeparateTerrain;
        public float speedTankMovement;
        public float speedTankRotate;
        public float gravityTankRotate;
        public float speedCannonRotate;
    }

    [SerializeField] private Tank tank;
    [SerializeField] private Attributes attributes;

    void Start()
    {

    }

    private void Update()
    {
        AutoRotation();
        if (Input.GetMouseButtonDown(0))
        {
            /*
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StartCoroutine(RotateCannon(mousePos));*/
        }
    }

    void FixedUpdate()
    {
        Movement();
        Rotation();
        FixHeightOnTerrain();
    }

    IEnumerator RotateCannon(Vector3 objetive)
    {
        while (false)
        {
            yield return null;
        }
    }

    void Movement()
    {
        float ver = Input.GetAxisRaw("Vertical");
        transform.Translate(Vector3.forward * (attributes.speedTankMovement * ver));
        transform.Translate(Vector3.forward * (-attributes.speedTankMovement / 2 * ver));
    }

    void Rotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -attributes.speedTankRotate);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, attributes.speedTankRotate);
        }
    }

    Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        planeNormal.Normalize();
        var distance = -Vector3.Dot(planeNormal, (point - planePoint));
        return point + planeNormal * distance;
    }

    void AutoRotation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Quaternion qTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, qTo, attributes.gravityTankRotate * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, qTo, attributes.speedTankRotate * Time.deltaTime);
        }
    }

    void FixHeightOnTerrain()
    {
        Vector3 newpos = transform.position;
        newpos.y = Terrain.activeTerrain.SampleHeight(transform.position) + attributes.tankSeparateTerrain;
        transform.position = newpos;
    }
}