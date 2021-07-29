using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsRewards : MonoBehaviour, IDamageable
{
    public GameManager.RewardsType type;
    public void TakeDamage(int damage)
    {
        Debug.Log("Impacto.");
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
        Destroy(gameObject);
    }
}