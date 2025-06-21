using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 10;
    public int damage = 1;
    public float moveSpeed = 2f;

    [Header("Animation")]
    public Sprite[] walkSprites;
    public Sprite[] deathSprites;
    public float frameRate = 0.2f;

    [Header("Target")]
    public Transform target; // usually the player

    private int currentHealth;
    private int frameIndex = 0;
    private float frameTimer = 0f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isDead = false;
    private bool hasRecentlyDamagedPlayer = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (target == null && GameManager.Instance != null)
            target = GameManager.Instance.player.transform;
    }

    void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleAnimation();
        FaceTarget();
    }

    void HandleMovement()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
    }

    void HandleAnimation()
    {
        if (walkSprites == null || walkSprites.Length == 0) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            frameIndex = (frameIndex + 1) % walkSprites.Length;
            spriteRenderer.sprite = walkSprites[frameIndex];
        }
    }

    void FaceTarget()
    {
        if (target == null) return;
        Vector2 dir = target.position - transform.position;
        spriteRenderer.flipX = dir.x < 0;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;

        foreach (var sprite in deathSprites)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(frameRate);
        }

        MoneyManager.Instance.AddMoney(100);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !hasRecentlyDamagedPlayer)
        {
            Player player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                Vector2 knockbackDir = (player.transform.position - transform.position).normalized;
                player.TakeDamage(damage, knockbackDir);
                StartCoroutine(DamageCooldown());
            }
        }
    }

    IEnumerator DamageCooldown()
    {
        hasRecentlyDamagedPlayer = true;
        yield return new WaitForSeconds(0.5f);
        hasRecentlyDamagedPlayer = false;
    }
}
