using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class Skeleton : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int damage = 1;
    public float attackRange = 1f;
    public float knockbackForce = 1.5f;
    public int rewardMoney = 50;

    [Header("Skill")]
    public float attackCooldown = 5f;

    [Header("Sprites")]
    public Sprite[] walkDown, walkUp, walkRight;
    public Sprite[] attackDown, attackUp, attackRight;
    public Sprite[] deathAnim;


    [Header("Others")]
    public LayerMask playerLayer;
    public Color hitColor = new Color(0.6f, 0.6f, 0.6f);
    public float hitEffectTime = 0.1f;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private int currentHealth;
    private Sprite[] currentAnim;
    private int animIndex = 0;
    private float animTimer = 0f;
    private float frameRate = 0.2f;

    private float attackTimer = 0f;
    private bool isAlive = true;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        player = GameManager.Instance?.player?.transform;

        currentHealth = maxHealth;
        currentAnim = walkDown;
        sr.sprite = currentAnim[0];
    }

    void Update()
    {
        if (!isAlive) return;

        attackTimer += Time.deltaTime;

        if (!isAttacking)
            Animate();

        if (attackTimer >= attackCooldown && !isAttacking)
        {
            attackTimer = 0f;
            StartCoroutine(Attack());
        }

        // ✅ 시점 고정
        transform.rotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        if (!isAlive || isAttacking || player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        UpdateDirection(dir);
    }

    void Animate()
    {
        if (currentAnim == null || currentAnim.Length == 0) return;

        animTimer += Time.deltaTime;
        if (animTimer >= frameRate)
        {
            animTimer = 0f;
            animIndex = (animIndex + 1) % currentAnim.Length;
            sr.sprite = currentAnim[animIndex];
        }
    }

    void UpdateDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            currentAnim = walkRight;
            sr.flipX = dir.x < 0;
        }
        else
        {
            currentAnim = dir.y > 0 ? walkUp : walkDown;
            sr.flipX = false;
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        Vector2 dir = (player.position - transform.position).normalized;

        // 방향에 따라 공격 애니메이션 선택
        Sprite[] attackAnim;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            attackAnim = attackRight;
            sr.flipX = dir.x < 0;
        }
        else
        {
            attackAnim = dir.y > 0 ? attackUp : attackDown;
            sr.flipX = false;
        }

        for (int i = 0; i < attackAnim.Length; i++)
        {
            sr.sprite = attackAnim[i];

            if (i == attackAnim.Length / 2)
            {
                Collider2D hit = Physics2D.OverlapCircle(transform.position + (Vector3)(dir * 0.8f), attackRange, playerLayer);
                if (hit != null && hit.CompareTag("Player"))
                {
                    if (hit.TryGetComponent<Player>(out var playerComp))
                    {
                        playerComp.TakeDamage(damage, dir);
                    }
                }
            }

            yield return new WaitForSeconds(frameRate);
        }

        isAttacking = false;
    }

    public void TakeDamage(int amount, Vector2 knockback)
    {
        if (!isAlive) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitEffect(knockback));
        }
    }

    IEnumerator HitEffect(Vector2 knockback)
    {
        sr.color = hitColor;
        rb.AddForce(knockback * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(hitEffectTime);
        rb.velocity = Vector2.zero;
        sr.color = Color.white;
    }

    void Die()
    {
        isAlive = false;
        GameManager.Instance?.AddMoney(rewardMoney);
        GameManager.Instance?.player?.Heal(1); // 피흡 1

        StartCoroutine(PlayDeathAnimation());
    }

    IEnumerator PlayDeathAnimation()
    {
        currentAnim = deathAnim;
        animIndex = 0;

        for (int i = 0; i < deathAnim.Length; i++)
        {
            sr.sprite = deathAnim[i];
            yield return new WaitForSeconds(frameRate);
        }

        Destroy(gameObject);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (player != null)
            Gizmos.DrawWireSphere(transform.position + (player.position - transform.position).normalized * 0.8f, attackRange);
    }
}
