using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SkeletonEnemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int damage = 1;
    public int rewardMoney = 50;

    [Header("Hit Effect")]
    public float knockbackDistance = 0.3f;
    public Color hitColor = new Color(0.6f, 0.6f, 0.6f);
    public float hitEffectTime = 0.1f;

    private int currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool isHit = false;
    private bool isAlive = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    public void TakeDamage(int amount, Vector2 knockbackDir)
    {
        if (!isAlive) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            isAlive = false;
            StartCoroutine(DieAndDestroy());
        }
        else
        {
            StartCoroutine(HitEffect(knockbackDir));
        }
    }

    IEnumerator DieAndDestroy()
    {
        // DungeonScene3 에선 회복 생략
        if (SceneManager.GetActiveScene().name != "DungeonScene3")
            GameManager.Instance?.player?.Heal(1);

        GameManager.Instance?.AddMoney(rewardMoney);

        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    IEnumerator HitEffect(Vector2 knockback)
    {
        isHit = true;
        rb.AddForce(knockback.normalized * knockbackDistance, ForceMode2D.Impulse);
        sr.color = hitColor;
        yield return new WaitForSeconds(hitEffectTime);
        sr.color = Color.white;
        isHit = false;
    }
}
