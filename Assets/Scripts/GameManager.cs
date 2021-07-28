using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [Serializable] public class Objects
    {
        public GameObject pfBarrel;
        public GameObject pfLootBox;
        public GameObject spawnBarrel;
        public GameObject spawnLootBox;
        public Transform group;
    }
    [SerializeField] public Objects objects;
    const float barrelOffset = 2.174f;
    const float LootBoxOffset = 1.58612f;
    public int initialBoxes = 10;
    public int initialBarrel = 10;
    public int initialBullets= 10;
    public int remainingBoxes = 10;
    public int remainingBarrel = 10;
    public int remainingBullets = 10;
    private float gameTime;

    void Start()
    {
        Bounds limits = objects.spawnBarrel.GetComponent<BoxCollider>().bounds;
        for (int i = 0; i < initialBarrel; i++)
        {
            GameObject barrel = Instantiate(objects.pfBarrel, RandomPosition(limits), Random.rotation, objects.group);
            barrel.transform.rotation = SetInclination(barrel.transform);
            barrel.transform.position = SetHeight(barrel, barrelOffset);
        }

        limits = objects.spawnLootBox.GetComponent<BoxCollider>().bounds;
        for (int i = 0; i < initialBoxes; i++)
        {
            GameObject boxes = Instantiate(objects.pfLootBox, RandomPosition(limits), Random.rotation, objects.group);
            boxes.transform.rotation = SetInclination(boxes.transform);
            boxes.transform.position = SetHeight(boxes, LootBoxOffset);
        }
    }
    void Update()
    {
        gameTime += Time.deltaTime;
    }


    Vector3 SetHeight(GameObject go, float offset)
    {
        Vector3 pos = go.transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(pos) + offset;
        return pos;
    }
    Vector3 RandomPosition(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
    Quaternion SetInclination(Transform transform)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Quaternion qTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            return qTo;
        }
        return Quaternion.identity;
    }
}