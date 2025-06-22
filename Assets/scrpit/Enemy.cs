using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int damage = 1;

    [Header("Hit Effect")]
    public float knockbackDistance = 0.3f;
    public float hitEffectTime = 0.1f;
    public Color hitColor = new Color(0.6f, 0.6f, 0.6f); // 회색

    [Header("Target")]
    public Transform target;

    [Header("Animations")]
    public Sprite[] walkSprites;
    public Sprite[] deathSprites;
    public float animFrameRate = 0.2f;

    private int currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isHit = false;
    private bool canDamage = true;
    private bool isAlive = true;

    private int rewardMoney = 50;

    // 애니메이션 관련
    private Sprite[] currentAnim;
    private float animTimer = 0f;
    private int animIndex = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        if (target == null && GameManager.Instance != null)
            target = GameManager.Instance.player.transform;

        currentAnim = walkSprites;
        if (currentAnim.Length > 0)
            spriteRenderer.sprite = currentAnim[0];
    }

    private void Update()
    {
        if (!isHit && isAlive && currentAnim != null && currentAnim.Length > 0)
        {
            Animate();
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive || isHit || target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (!isAlive || isHit || target == null) return;

        Vector2 toTarget = (target.position - transform.position);
        currentAnim = toTarget.magnitude > 0.01f ? walkSprites : new Sprite[] { spriteRenderer.sprite };
    }

    void Animate()
    {
        animTimer += Time.deltaTime;
        if (animTimer >= animFrameRate)
        {
            animTimer = 0f;
            animIndex = (animIndex + 1) % currentAnim.Length;
            spriteRenderer.sprite = currentAnim[animIndex];
        }
    }

    public void TakeDamage(int amount, Vector2 knockbackDir)
    {
        if (!isAlive) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            isAlive = false;
            StartCoroutine(PlayDeathAnimation());
        }
        else
        {
            StartCoroutine(HitEffect(knockbackDir));
        }
    }

    IEnumerator PlayDeathAnimation()
    {
        // 회복, 돈 지급
        if (GameManager.Instance?.player != null)
            GameManager.Instance.player.Heal(2);

        GameManager.Instance?.AddMoney(rewardMoney);

        isHit = true;
        currentAnim = deathSprites;
        animIndex = 0;
        animTimer = 0f;

        for (int i = 0; i < deathSprites.Length; i++)
        {
            spriteRenderer.sprite = deathSprites[i];
            yield return new WaitForSeconds(animFrameRate);
        }

        Destroy(gameObject);
    }

    IEnumerator HitEffect(Vector2 knockback)
    {
        isHit = true;

        Vector3 originalPos = transform.position;
        Vector3 knockbackPos = originalPos + (Vector3)(knockback.normalized * knockbackDistance);
        transform.position = knockbackPos;

        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitEffectTime);

        spriteRenderer.color = Color.white;
        isHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive || !canDamage) return;

        if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                Vector2 knockbackDir = (player.transform.position - transform.position).normalized;
                player.TakeDamage(damage, knockbackDir);

                rb.velocity = Vector2.zero;
                StartCoroutine(DamageCooldown());
            }
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(0.5f);
        canDamage = true;
    }
}
