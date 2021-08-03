using UnityEngine;
public class Bullet : MonoBehaviour
{
    public LayerMask layersImpacts;
    public int damage;
    private bool impact;
    public AudioSource soundShoot;

    private void Awake()
    {
        soundShoot = GameObject.Find("BulletGroup_Ref").GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!impact)
        {
            if (ConstantsFunctions.LayerEquals(layersImpacts, other.gameObject.layer))
            {
                if (!soundShoot)
                {
                    soundShoot = GameObject.Find("BulletGroup_Ref").GetComponent<AudioSource>();
                }
                soundShoot.Play();
                impact = true;
                Debug.Log("La bala choca contra: " + other.gameObject.name);
                Destroy(gameObject);
                
                IDamageable objetive = other.gameObject.GetComponent<IDamageable>();
                if (objetive == null)
                {
                }
                else
                {
                    objetive.TakeDamage(damage);
                }

                //ObjectsRewards rewards = other.gameObject.GetComponent<ObjectsRewards>();
                //if (rewards == null) return;
            }
        }
    }
}