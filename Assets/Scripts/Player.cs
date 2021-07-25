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
    }
    [Serializable] public class Attributes
    {
        public float speedTankMovement;
        public float speedTankRotate;
        public float speedCannonRotate;
    }
    [SerializeField] private Tank tank;
    [SerializeField] private Attributes attributes;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * attributes.speedTankMovement);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * (-attributes.speedTankMovement / 2));
        }
        if (Input.GetKey(KeyCode.A))
        {

        }
        if (Input.GetKey(KeyCode.D))
        {

        }
        if (Input.GetKey(KeyCode.Space))
        {

        }
        if (Input.GetMouseButtonDown(0))
        {

        }
    }
}
