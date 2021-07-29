using System;
using UnityEngine;

public class ObjectsRewards : MonoBehaviour, IDamageable
{
    public LayerMask explodeLayerMask;
    public GameManager.RewardsType type;
    public Action<int, GameManager.RewardsType, bool> giveReward;
    public int rewardScore;
    public int rewardBullet;
    public int life;
    public int damageExplosion = 0;

    public void TakeDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            switch (type)
            {
                case GameManager.RewardsType.Barrel:
                    Debug.Log("Barril Destruido.");
                    break;
                case GameManager.RewardsType.LootBox:
                    Debug.Log("LootBox Destruido.");
                    break;
                default:
                    Debug.LogWarning("Se destruyó fuera del rango (No asignado).");
                    break;
            }
            life = 0;
            giveReward?.Invoke(rewardScore, type, true);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (ConstantsFunctions.LayerEquals(explodeLayerMask, other.gameObject.layer))
        {
            Debug.Log("Choca Tanke: " + other.gameObject.name);
            TakeDamage(life);
            giveReward?.Invoke(-rewardScore, type, true);
            giveReward?.Invoke(rewardBullet, type, false);
        }
    }
}