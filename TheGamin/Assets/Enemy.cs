using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 50f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Можно проиграть анимацию смерти здесь
        Destroy(gameObject);
    }
}
