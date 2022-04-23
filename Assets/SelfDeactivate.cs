using System.Collections;
using UnityEngine;

public class SelfDeactivate : MonoBehaviour
{
    [Tooltip("The time until the object self-deactivates.")]
    [SerializeField] float lifeTime = 1f;

    void OnEnable()
    {
        StartCoroutine(LifetimeCountdown());
    }

    // Destroy the object after a few seconds.
    IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
}
