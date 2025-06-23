using UnityEngine;
using System.Collections;
using Unity.Burst.CompilerServices;
using TMPro;



public class Player : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public GameObject arrowPrefab;
    public FanMeshGenerator fanVisualizer;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float dashDistance = 2f;
    public float dashDuration = 0.15f;
    public float dodgeCooldown = 1f;
    public float shootMoveSpeed = 3f;
    public float swordMoveSpeed = 3f;

    [Header("Base Stats")]
    public int baseMaxHealth = 10;
    public int baseSwordDamage = 3;
    public float baseSwordRange = 1f;
    public int baseBowDamage = 2;
    public int basePierceCount = 1;
    public float baseInvincibleTime = 0.3f;
    public float baseDashDistance = 2f;

    [Header("Bonus Stats")]
    public int bonusSwordDamage;
    public float bonusSwordRange;
    public int bonusBowDamage;
    public int bonusPierceCount;
    public int bonusMaxHealth;
    public float bonusInvincibleTime;
    public float bonusDashDistance;

    [Header("Current Stats")]
    public int maxHealth;
    public int swordDamage;
    public float attackRange;
    public int bowDamage;
    public int pierceCount;
    public float invincibleTime;

    [Header("Combat Settings")]
    public float attackCooldown = 0.25f;
    public float shootCooldown = 0.25f;

    public LayerMask enemyLayer;

    [Header("Animations")]
    public Sprite[] walkDown, walkUp, walkRight;
    public Sprite[] idleDown, idleUp, idleRight;
    public Sprite[] swordDown, swordUp, swordRight;
    public Sprite[] bowDown, bowUp, bowRight;

    [Header("UI - Stat Display")]
    public TextMeshProUGUI swordDamageText;
    public TextMeshProUGUI swordRangeText;
    public TextMeshProUGUI bowDamageText;
    public TextMeshProUGUI pierceCountText;
    public TextMeshProUGUI maxHealthText;
    public TextMeshProUGUI invincibleTimeText;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 velocity;
    private Vector2 facingDir = Vector2.down;

    private bool isAttacking = false;
    private bool isShooting = false;
    private bool isDodging = false;

    private Sprite[] currentAnim;
    private enum Direction { Down, Up, Right }
    private Direction currentDir = Direction.Down;

    private float frameTimer = 0f;
    private int frameIndex = 0;
    private float frameRate = 0.2f;

    public int currentHealth;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        RecalculateStats();
        currentHealth = maxHealth;
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }


    public void ApplyUpgrade(StatType stat, float amount)
    {
        switch (stat)
        {
            case StatType.MeleeDamage:
                bonusSwordDamage += Mathf.RoundToInt(amount);
                break;
            case StatType.MeleeRange:
                bonusSwordRange += amount;
                break;
            case StatType.RangedDamage:
                bonusBowDamage += Mathf.RoundToInt(amount);
                break;
            case StatType.RangedPierce:
                bonusPierceCount += Mathf.RoundToInt(amount);
                break;
            case StatType.MaxHealth:
                bonusMaxHealth += Mathf.RoundToInt(amount);
                break;
            case StatType.InvincibleTime:
                bonusInvincibleTime += amount;
                bonusDashDistance += amount * 2f; // ✅ 비율 조절 가능
                break;

        }
    }


    public void RecalculateStats()
    {
        swordDamage = baseSwordDamage + bonusSwordDamage;
        attackRange = baseSwordRange + bonusSwordRange;
        bowDamage = baseBowDamage + bonusBowDamage;
        pierceCount = basePierceCount + bonusPierceCount;
        maxHealth = baseMaxHealth + bonusMaxHealth;
        invincibleTime = baseInvincibleTime + bonusInvincibleTime;

        //  이동 거리 제한
        float maxDash = 4.5f;
        dashDistance = Mathf.Min(baseDashDistance + bonusDashDistance, maxDash);

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateStatUI();
    }


    void UpdateStatUI()
    {
        if (swordDamageText != null) swordDamageText.text = $"검 데미지: {swordDamage}";
        if (swordRangeText != null) swordRangeText.text = $"검 범위: {attackRange:F1}";
        if (bowDamageText != null) bowDamageText.text = $"활 데미지: {bowDamage}";
        if (pierceCountText != null) pierceCountText.text = $"관통력: {pierceCount}";
        if (maxHealthText != null) maxHealthText.text = $"최대 체력: {maxHealth}";
        if (invincibleTimeText != null) invincibleTimeText.text = $"회피 시간: {invincibleTime:F2}초";
    }

    void Update()
    {
        bool canAct = !isAttacking && !isShooting && !isDodging;

        if (!isDodging)
        {
            HandleInput();
            if (canAct) HandleMovementAnim();
        }

        if (Input.GetKeyDown(KeyCode.S) && canAct)
            StartCoroutine(SwordAttack());

        if (Input.GetKeyDown(KeyCode.D) && canAct)
            StartCoroutine(ShootArrow());

        if (Input.GetKeyDown(KeyCode.A) && canAct)
            StartCoroutine(Dodge());

        transform.rotation = Quaternion.identity;

     

    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }

    void HandleInput()
    {
        input = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow)) input.x = -1;
        if (Input.GetKey(KeyCode.RightArrow)) input.x = 1;
        if (Input.GetKey(KeyCode.UpArrow)) input.y = 1;
        if (Input.GetKey(KeyCode.DownArrow)) input.y = -1;

        velocity = input.normalized * moveSpeed;

        if (input != Vector2.zero)
            facingDir = input.normalized;

    }


    void HandleMovementAnim()
    {
        if (input.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                currentDir = input.y > 0 ? Direction.Up : Direction.Down;
                spriteRenderer.flipX = false;
            }
            else
            {
                currentDir = Direction.Right;
                spriteRenderer.flipX = input.x < 0;
            }

            currentAnim = currentDir switch
            {
                Direction.Down => walkDown,
                Direction.Up => walkUp,
                Direction.Right => walkRight,
                _ => currentAnim
            };
        }
        else
        {
            currentAnim = currentDir switch
            {
                Direction.Down => idleDown,
                Direction.Up => idleUp,
                Direction.Right => idleRight,
                _ => currentAnim
            };
        }

        Animate();
    }

    void Animate()
    {
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            frameIndex = (frameIndex + 1) % currentAnim.Length;
            spriteRenderer.sprite = currentAnim[frameIndex];
        }
    }

    IEnumerator SwordAttack()
    {
        isAttacking = true;

        // 🟩 현재 방향을 복사해서 고정
        Vector2 attackDir = facingDir;

        // 부채꼴 시각화
        fanVisualizer.gameObject.SetActive(true);
        fanVisualizer.transform.position = transform.position;
        fanVisualizer.transform.right = attackDir;
        fanVisualizer.UpdateMesh(90f, attackRange);
        Invoke(nameof(HideFanVisualizer), 0.1f);

        // 애니메이션 고정
        Sprite[] attackAnim = swordDown;
        if (Mathf.Abs(attackDir.y) > Mathf.Abs(attackDir.x))
        {
            attackAnim = attackDir.y > 0 ? swordUp : swordDown;
            spriteRenderer.flipX = false;
        }
        else
        {
            attackAnim = swordRight;
            spriteRenderer.flipX = attackDir.x < 0;
        }

        for (int i = 0; i < attackAnim.Length; i++)
        {
            spriteRenderer.sprite = attackAnim[i];

            if (i == 1)
            {
                float angleRange = 90f;
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
                foreach (var hit in hits)
                {
                    Vector2 toTarget = (hit.transform.position - transform.position).normalized;
                    if (Vector2.Angle(attackDir, toTarget) <= angleRange / 2)
                    {
                        if (hit.TryGetComponent(out Enemy enemy))
                        {
                            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                            enemy.TakeDamage(swordDamage, knockbackDir);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(frameRate);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }


    void HideFanVisualizer()
    {
        fanVisualizer.gameObject.SetActive(false);
    }





    IEnumerator ShootArrow()
    {
        isShooting = true;

        //현재 방향을 복사해서 고정
        Vector2 shootDir = facingDir;

        Sprite[] bowAnim = bowDown;
        if (Mathf.Abs(shootDir.y) > Mathf.Abs(shootDir.x))
        {
            bowAnim = shootDir.y > 0 ? bowUp : bowDown;
            spriteRenderer.flipX = false;
        }
        else
        {
            bowAnim = bowRight;
            spriteRenderer.flipX = shootDir.x < 0;
        }

        for (int i = 0; i < bowAnim.Length; i++)
        {
            spriteRenderer.sprite = bowAnim[i];
            if (i == 2)
            {
                GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                var arrowScript = arrow.GetComponent<Arrow>();
                arrowScript.SetDirection(shootDir); // ← 고정된 방향 사용
                arrowScript.pierce = pierceCount;
            }
            yield return new WaitForSeconds(frameRate);
        }

        yield return new WaitForSeconds(shootCooldown);
        isShooting = false;
    }


    IEnumerator Dodge()
    {
        isDodging = true;

        gameObject.layer = LayerMask.NameToLayer("Invincible");
        spriteRenderer.color = new Color32(200, 200, 200, 255);

        float elapsed = 0f;
        Vector2 start = rb.position;
        Vector2 target = start + facingDir.normalized * dashDistance;

        while (elapsed < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(start, target, elapsed / dashDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(target);
        isDodging = false;

        yield return new WaitForSeconds(invincibleTime - dashDuration);
        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(dodgeCooldown);
    }

    public void TakeDamage(int amount, Vector2 knockback)
    {
        currentHealth -= amount;
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        rb.velocity = Vector2.zero; // ✅ 즉시 속도 초기화
        StartCoroutine(HitEffect(knockback));
    }


    IEnumerator HitEffect(Vector2 knockback)
    {
        rb.velocity = Vector2.zero; // ✅ 혹시 모를 이동 정지
        rb.AddForce(knockback * 5f, ForceMode2D.Impulse); // ✅ 넉백 적용

        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.15f);

        rb.velocity = Vector2.zero; // ✅ 넉백 후 정지
        spriteRenderer.color = Color.white;
    }




    void Die()
    {
        GameManager.Instance.PlayerDied();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void FixedUpdate()
    {
        if (!isDodging)
        {
            float currentSpeed = isAttacking ? swordMoveSpeed : isShooting ? shootMoveSpeed : moveSpeed;
            rb.MovePosition(rb.position + input.normalized * currentSpeed * Time.fixedDeltaTime);
        }
    }
}
