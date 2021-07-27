using System;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [Serializable] public class Objects
    {
        public GameObject pfBarrel;
        public GameObject pfLootBox;
    }
    [SerializeField] public Objects objects;

    public int initialBoxes = 10;
    public int initialBarrel = 10;
    public int initialBullets= 10;
    public int remainingBoxes = 10;
    public int remainingBarrel = 10;
    public int remainingBullets = 10;
    private float gameTime;

    void Start()
    {
        
    }
    void Update()
    {
        gameTime += Time.deltaTime;
    }
}