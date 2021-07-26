using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player : MonoBehaviour
{
    [Serializable] public class Tank
    {
        public GameObject tank;
        public GameObject cannon;
        public GameObject firePoint;
        public GameObject PfBullet;
    }
    [Serializable] public class Attributes
    {
        public float speedTankMovement;
        public float speedTankRotate;
        public float speedCannonRotate;
    }
    [SerializeField] private Tank tank;
    [SerializeField] private Attributes attributes;
    
    void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }
    void FixedUpdate()
    {
        float ver = Input.GetAxisRaw("Vertical");
        transform.Translate(Vector3.forward * (attributes.speedTankMovement * ver));
        transform.Translate(Vector3.forward * (-attributes.speedTankMovement / 2 * ver));

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(transform.up, -attributes.speedTankRotate);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(transform.up, attributes.speedTankRotate);
        }
        if (Input.GetKey(KeyCode.Space))
        {

        }
    }
}