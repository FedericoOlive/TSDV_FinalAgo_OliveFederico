using UnityEngine;

public class GasPump : MonoBehaviour
{
    [SerializeField] private float fuelCharged;

    private void OnTriggerStay(Collider other)
    {
        IRechargeFuel objetive = other.gameObject.GetComponent<IRechargeFuel>();
        Debug.Log("Entró un objeto " + other.name);
        if (objetive == null) return;

        objetive.RechargeFuel(fuelCharged);
    }
}