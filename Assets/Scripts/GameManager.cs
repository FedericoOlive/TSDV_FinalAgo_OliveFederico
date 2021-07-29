using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public Action updateScore;
    public Action onGameOver;
    public Action onGetBullet;
    public enum RewardsType
    {
        LootBox,
        Barrel,
    };

    [Serializable]
    public class Objects
    {
        public Player player;
        public GameObject pfBarrel;
        public GameObject pfLootBox;
        public GameObject spawnBarrel;
        public GameObject spawnLootBox;
        public Transform group;
        public EnemyTurret enemyTurret;
    }

    [SerializeField] public Objects objects;
    const float barrelOffset = 2.174f;
    const float LootBoxOffset = 1.58612f;
    public int initialBoxes = 10;
    public int initialBarrel = 10;
    public int initialBullets = 10;
    public int boxesDestroyed;
    public int barrelsDestroyed;
    public float maxGameTime = 120;
    public float gameTime;
    public int score;
    private bool gameOver;

    void Start()
    {
        score = 0;
        Bounds limits = objects.spawnBarrel.GetComponent<BoxCollider>().bounds;
        for (int i = 0; i < initialBarrel; i++)
        {
            GameObject barrel = Instantiate(objects.pfBarrel, RandomPosition(limits), Random.rotation, objects.group);
            barrel.transform.rotation = SetInclination(barrel.transform);
            barrel.transform.position = SetHeight(barrel, barrelOffset);
            barrel.GetComponent<ObjectsRewards>().giveReward += AddReward;
        }

        limits = objects.spawnLootBox.GetComponent<BoxCollider>().bounds;
        for (int i = 0; i < initialBoxes; i++)
        {
            GameObject boxes = Instantiate(objects.pfLootBox, RandomPosition(limits), Random.rotation, objects.group);
            boxes.transform.rotation = SetInclination(boxes.transform);
            boxes.transform.position = SetHeight(boxes, LootBoxOffset);
            boxes.GetComponent<ObjectsRewards>().giveReward += AddReward;
            objects.player.onDie += GameOver;
        }
    }
    void Update()
    {
        gameTime += Time.deltaTime;
        
        //Invoke(nameof(GameOver), maxGameTime);        Diferencias?
        if (gameTime > maxGameTime)
        {
            GameOver();
        }
    }
    void GameOver()
    {
        if (!gameOver)
        {
            Time.timeScale = 0;
            gameOver = true;
            onGameOver?.Invoke();
        }
    }
    void AddReward(int reward, RewardsType type, bool bullet)
    {
        if (bullet)
        {
            score += reward;
            switch (type)
            {
                case RewardsType.Barrel:
                    barrelsDestroyed++;
                    break;
                case RewardsType.LootBox:
                    boxesDestroyed++;
                    break;
                default:
                    Debug.LogWarning("RewardType excede el límite.");
                    break;
            }

            updateScore?.Invoke();
        }
        else
        {
            switch (type)
            {
                case RewardsType.Barrel:
                    barrelsDestroyed++;
                    break;
                case RewardsType.LootBox:
                    boxesDestroyed++;
                    onGetBullet?.Invoke();
                    break;
                default:
                    Debug.LogWarning("RewardType excede el límite.");
                    break;
            }
        }
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
public class ConstantsFunctions
{
    public static bool LayerEquals(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}