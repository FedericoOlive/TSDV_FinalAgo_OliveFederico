using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask layersImpacts;
    public int damage;
    private bool impact;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (!impact)
        {
            if (ConstantsFunctions.LayerEquals(layersImpacts, other.gameObject.layer))
            {
                impact = true;
                Debug.Log("La bala choca contra: " + other.gameObject.name);
                Destroy(gameObject);
                IDamageable objetive = other.gameObject.GetComponent<IDamageable>();

                if (objetive == null) return;
                objetive.TakeDamage(damage);

                //ObjectsRewards rewards = other.gameObject.GetComponent<ObjectsRewards>();
                //if (rewards == null) return;

            }
        }
    }
}