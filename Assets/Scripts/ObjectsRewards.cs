using System;
using UnityEngine;

public class ObjectsRewards : MonoBehaviour, IDamageable
{
    public LayerMask explodeLayerMask;
    public GameManager.RewardsObject rewardObject;
    public GameManager.RewardType rewardType;
    public Action<int, GameManager.RewardsObject, GameManager.RewardType> giveReward;
    public int rewardScore;
    public int rewardBullet;
    public int life;
    public int damageExplosion = 0;
    private bool dead;
    public void TakeDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            dead = true;
            life = 0;
            giveReward?.Invoke(rewardScore, rewardObject, rewardType);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (dead) return;

        if (ConstantsFunctions.LayerEquals(explodeLayerMask, other.gameObject.layer))
        {
            dead = true;
            Destroy(gameObject);
            switch (rewardObject)
            {
                case GameManager.RewardsObject.Barrel:
                    giveReward?.Invoke(damageExplosion, rewardObject, GameManager.RewardType.Damage);
                    break;
                case GameManager.RewardsObject.LootBox:
                    giveReward?.Invoke(rewardBullet, rewardObject, GameManager.RewardType.Bullet);
                    break;
                default:
                    Debug.LogWarning("rewardObject excede el límite.");
                    break;
            }
        }
    }
}