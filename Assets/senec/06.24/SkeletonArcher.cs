using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class SkeletonArcher : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int damage = 1;
    public int rewardMoney = 50;

    [Header("Ranges & Cooldowns")]
    public float meleeRange = 1f;
    public float rangedRange = 6f;
    public float rangedCooldown = 3f;

    [Header("Sprites")]
    public Sprite[] walkDown, walkUp, walkRight;
    public Sprite[] bowDown, bowUp, bowRight;

    [Header("Death Animation")]
    public Sprite[] deathAnim;

    [Header("Arrow Prefab")]
    public GameObject arrowPrefab;
    public float arrowLifeTime = 5f;

    [Header("Hit Effect")]
    public Color hitColor = new Color(0.6f, 0.6f, 0.6f);
    public float hitEffectTime = 0.1f;
    public float knockbackForce = 0.3f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform player;

    int currentHealth;
    float rangedTimer;
    bool isAlive = true;
    bool isAttacking = false;

    // Walk anim
    Sprite[] currentAnim;
    int animIndex = 0;
    float animTimer = 0f;
    float frameRate = 0.2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        player = GameManager.Instance?.player?.transform;
        currentHealth = maxHealth;

        currentAnim = walkDown;
        if (currentAnim?.Length > 0)
            sr.sprite = currentAnim[0];
    }

    void Update()
    {
        if (!isAlive) return;
        rangedTimer += Time.deltaTime;

        if (!isAttacking) AnimateWalk();

        if (rangedTimer >= rangedCooldown && !isAttacking && player != null)
        {
            float d = Vector2.Distance(transform.position, player.position);
            if (d <= rangedRange && d > meleeRange)
            {
                rangedTimer = 0f;
                StartCoroutine(ShootRoutine((player.position - transform.position).normalized));
            }
        }

        transform.rotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        if (!isAlive || isAttacking || player == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        UpdateDirection(dir);
    }

    IEnumerator ShootRoutine(Vector2 dir)
    {
        isAttacking = true;

        // bow anim
        Sprite[] bowAnim = Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
            ? bowRight
            : (dir.y > 0 ? bowUp : bowDown);
        sr.flipX = (bowAnim == bowRight && dir.x < 0);
        foreach (var s in bowAnim)
        {
            sr.sprite = s;
            yield return new WaitForSeconds(frameRate);
        }

        // spawn arrow
        var go = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        var arr = go.GetComponent<SkeletonArrow>();
        if (arr != null)
            arr.Init(dir);
        Destroy(go, arrowLifeTime);

        yield return new WaitForSeconds(frameRate);
        isAttacking = false;
    }

    void AnimateWalk()
    {
        if (currentAnim == null || currentAnim.Length == 0) return;
        animTimer += Time.deltaTime;
        if (animTimer >= frameRate)
        {
            animTimer = 0f;
            sr.sprite = currentAnim[animIndex = (animIndex + 1) % currentAnim.Length];
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

    public void TakeDamage(int amt, Vector2 kb)
    {
        if (!isAlive) return;
        currentHealth -= amt;
        if (currentHealth <= 0) Die();
        else StartCoroutine(HitEffect(kb));
    }

    IEnumerator HitEffect(Vector2 kb)
    {
        sr.color = hitColor;
        rb.AddForce(kb.normalized * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(hitEffectTime);
        sr.color = Color.white;
    }

    void Die()
    {
        Debug.Log(">> Die() 호출됨");
        isAlive = false;
        isAttacking = true;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        Debug.Log(">> DeathRoutine 시작");

        // 보상
        GameManager.Instance?.AddMoney(rewardMoney);
        GameManager.Instance?.player?.Heal(1);

        if (deathAnim == null)
        {
            Debug.LogError("!! deathAnim 배열 자체가 null입니다.");
        }
        else if (deathAnim.Length == 0)
        {
            Debug.LogWarning("!! deathAnim 배열이 비어 있습니다.");
        }
        else
        {
            for (int i = 0; i < deathAnim.Length; i++)
            {
                Debug.Log($">> deathAnim[{i}] 프레임 재생");
                sr.sprite = deathAnim[i];
                yield return new WaitForSeconds(frameRate);
            }
        }

        Debug.Log(">> DeathRoutine 완료, Destroy 호출");
        Destroy(gameObject);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangedRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
