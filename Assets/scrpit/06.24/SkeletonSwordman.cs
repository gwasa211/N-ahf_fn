using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class SkeletonSwordman : MonoBehaviour
{
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int damage = 2;
    public float attackRange = 1.5f;
    public float skillCooldown = 5f;
    public float knockbackForce = 0.3f;
    public int rewardMoney = 50;

    public Sprite[] walkRight;
    public Sprite[] walkUp;
    public Sprite[] walkDown;

    public Sprite[] attackRight;
    public Sprite[] attackUp;
    public Sprite[] attackDown;

    public Sprite[] deathSprites;
    public float animFrameRate = 0.2f;

    private enum Direction { Right, Up, Down }
    private Direction currentDir = Direction.Down;

    private int currentHealth;
    private bool isAlive = true;
    private bool isAttacking = false;

    private Transform target;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Sprite[] currentAnim;
    private int animIndex = 0;
    private float animTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
            target = GameManager.Instance.player.transform;

        StartCoroutine(SkillAttackRoutine());

        currentAnim = walkDown;
        if (currentAnim.Length > 0) sr.sprite = currentAnim[0];
    }

    private void Update()
    {
        if (isAlive && !isAttacking && currentAnim.Length > 0)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animFrameRate)
            {
                animTimer = 0f;
                animIndex = (animIndex + 1) % currentAnim.Length;
                sr.sprite = currentAnim[animIndex];
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive || isAttacking || target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;
        UpdateDirection(dir);
        UpdateWalkAnimation();

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
            currentDir = dir.y > 0 ? Direction.Up : Direction.Down;
        else
            currentDir = Direction.Right;
    }

    void UpdateWalkAnimation()
    {
        switch (currentDir)
        {
            case Direction.Up: currentAnim = walkUp; break;
            case Direction.Down: currentAnim = walkDown; break;
            case Direction.Right: currentAnim = walkRight; break;
        }
    }

    void UpdateAttackAnimation()
    {
        switch (currentDir)
        {
            case Direction.Up: currentAnim = attackUp; break;
            case Direction.Down: currentAnim = attackDown; break;
            case Direction.Right: currentAnim = attackRight; break;
        }
    }

    IEnumerator SkillAttackRoutine()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(skillCooldown);

            if (!isAlive || target == null) continue;

            isAttacking = true;

            Vector2 dir = (target.position - transform.position).normalized;
            UpdateDirection(dir);
            UpdateAttackAnimation();

            animIndex = 0;
            for (int i = 0; i < currentAnim.Length; i++)
            {
                sr.sprite = currentAnim[i];
                yield return new WaitForSeconds(animFrameRate);
            }

            TryDealDamage();
            isAttacking = false;
        }
    }

    void TryDealDamage()
    {
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= attackRange)
        {
            Player player = target.GetComponent<Player>();
            if (player != null)
            {
                Vector2 knockbackDir = (player.transform.position - transform.position).normalized;
                player.TakeDamage(damage, knockbackDir);
            }
        }
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
            rb.AddForce(knockback * knockbackForce, ForceMode2D.Impulse);
            sr.color = Color.gray;
            Invoke(nameof(ResetColor), 0.15f);
        }
    }

    void ResetColor()
    {
        sr.color = Color.white;
    }

    void Die()
    {
        isAlive = false;
        StopAllCoroutines();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(rewardMoney);
            GameManager.Instance.player?.Heal(2);
        }

        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        currentAnim = deathSprites;
        for (int i = 0; i < currentAnim.Length; i++)
        {
            sr.sprite = currentAnim[i];
            yield return new WaitForSeconds(animFrameRate);
        }

        Destroy(gameObject);
    }
}
