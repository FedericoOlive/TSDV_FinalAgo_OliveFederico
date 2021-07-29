using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsRewards : MonoBehaviour, IDamageable
{
    public GameManager.RewardsType type;
    public Action<int> giveReward;
    public int rewardScore;
    public int life;

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
                case GameManager.RewardsType.EnemyTurret:
                    Debug.Log("EnemyTurret Destruido.");
                    break;
                default:
                    Debug.LogWarning("Se destruyó fuera del rango (No asignado).");
                    break;
            }
            life = 0;
            giveReward?.Invoke(rewardScore);
            Destroy(gameObject);
        }
    }
}