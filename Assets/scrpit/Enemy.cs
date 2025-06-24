using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int damage = 1;
    public float knockbackDistance = 0.3f;
    public Color hitColor = new Color(0.6f, 0.6f, 0.6f);
    public float hitEffectTime = 0.1f;

    public Transform target;
    public Sprite[] walkSprites;
    public Sprite[] deathSprites;
    public float animFrameRate = 0.2f;

    private int currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isHit = false;
    private bool isAlive = true;
    private int rewardMoney = 50;

    private Sprite[] currentAnim;
    private int animIndex = 0;
    private float animTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        if (target == null && GameManager.Instance?.player != null)
            target = GameManager.Instance.player.transform;

        currentAnim = walkSprites;
        sr.sprite = currentAnim[0];
    }

    private void Update()
    {
        if (!isHit && isAlive && currentAnim.Length > 0)
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
        if (!isAlive || isHit || target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        sr.flipX = dir.x < 0;
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
        GameManager.Instance?.player?.Heal(2);
        GameManager.Instance?.AddMoney(rewardMoney);

        isHit = true;
        currentAnim = deathSprites;
        animIndex = 0;

        for (int i = 0; i < deathSprites.Length; i++)
        {
            sr.sprite = deathSprites[i];
            yield return new WaitForSeconds(animFrameRate);
        }

        Destroy(gameObject);
    }

    IEnumerator HitEffect(Vector2 knockback)
    {
        isHit = true;
        transform.position += (Vector3)(knockback.normalized * knockbackDistance);
        sr.color = hitColor;
        yield return new WaitForSeconds(hitEffectTime);
        sr.color = Color.white;
        isHit = false;
    }
}
