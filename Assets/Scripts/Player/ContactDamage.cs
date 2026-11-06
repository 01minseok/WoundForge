using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ApplyDamage(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ApplyDamage(collision);
    }

    private void ApplyDamage(Collision2D collision)
    {
        PlayerHealth health =
            collision.gameObject.GetComponentInParent<PlayerHealth>();

        if (health != null)
            health.TakeDamage(damage);
    }
}