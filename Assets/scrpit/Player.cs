using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2f;
    public SpriteRenderer spriteRenderer;

    public Sprite[] walkDown, walkUp, walkRight;
    public Sprite[] idleDown, idleUp, idleRight;
    public Sprite[] swordDown, swordUp, swordRight;
    public Sprite[] bowDown, bowUp, bowRight;

    public GameObject arrowPrefab;
    public LayerMask enemyLayer;

    public float attackCooldown = 0.5f;
    public float shootCooldown = 0.5f;
    public float dashDistance = 2f;
    public float dashDuration = 0.15f;
    public float dodgeCooldown = 1f;
    public float shootMoveSpeed = 3f;
    public float swordMoveSpeed = 3f;

    public int baseMaxHealth = 10;
    public int baseSwordDamage = 3;
    public float baseSwordRange = 1f;
    public int baseBowDamage = 2;
    public int baseArrowCount = 1;
    public float baseDodgeInvincibleTime = 0.3f;

    public int maxHealth;
    public int swordDamage;
    public float attackRange;
    public int bowDamage;
    public int arrowCount;
    public float invincibleTime;

    public int swordDamageIndex, swordRangeIndex, bowDamageIndex;
    public int arrowCountIndex, maxHealthIndex, dodgeTimeIndex;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 velocity;

    private float frameTimer = 0f;
    private int frameIndex = 0;
    private float frameRate = 0.2f;

    private Sprite[] currentAnim;
    private enum Direction { Down, Up, Right }
    private Direction currentDir = Direction.Down;

    private bool isAttacking = false;
    private bool isShooting = false;
    private bool isDodging = false;

    public int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        currentAnim = walkDown;

        RecalculateStats();
        currentHealth = maxHealth;
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void RecalculateStats()
    {
        var mgr = UpgradeManager.Instance;

        maxHealth = baseMaxHealth + (int)mgr.GetUpgradeValue(maxHealthIndex);
        swordDamage = baseSwordDamage + (int)mgr.GetUpgradeValue(swordDamageIndex);
        attackRange = baseSwordRange + mgr.GetUpgradeValue(swordRangeIndex);
        bowDamage = baseBowDamage + (int)mgr.GetUpgradeValue(bowDamageIndex);
        arrowCount = baseArrowCount + (int)mgr.GetUpgradeValue(arrowCountIndex);
        invincibleTime = baseDodgeInvincibleTime + mgr.GetUpgradeValue(dodgeTimeIndex);

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void Update()
    {
        bool canAct = !isAttacking && !isShooting && !isDodging;

        if (!isDodging)
        {
            HandleInput();

            if (!isShooting && !isAttacking)
                HandleMovementAnim();
        }

        if (Input.GetKeyDown(KeyCode.S) && canAct)
            StartCoroutine(SwordAttack());

        if (Input.GetKeyDown(KeyCode.D) && canAct)
            StartCoroutine(ShootArrow());

        if (Input.GetKeyDown(KeyCode.A) && canAct)
            StartCoroutine(Dodge());

        if (Input.GetKeyDown(KeyCode.P))
            GameManager.Instance.AddMoney(1000);

        if (Input.GetKeyDown(KeyCode.O))
            UseHealthSkill();
    }

    void UseHealthSkill()
    {
        int damage = 100;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        Debug.Log($"스킬 사용 - 체력 {damage} 감소. 현재 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("플레이어 사망");
            Die();
        }
    }

    void FixedUpdate()
    {
        if (!isDodging)
        {
            float currentSpeed = isAttacking ? swordMoveSpeed : isShooting ? shootMoveSpeed : moveSpeed;
            rb.MovePosition(rb.position + input.normalized * currentSpeed * Time.fixedDeltaTime);
        }
    }

    void HandleInput()
    {
        input = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow)) input.x = -1;
        if (Input.GetKey(KeyCode.RightArrow)) input.x = 1;
        if (Input.GetKey(KeyCode.UpArrow)) input.y = 1;
        if (Input.GetKey(KeyCode.DownArrow)) input.y = -1;

        velocity = input.normalized * moveSpeed;
    }

    void HandleMovementAnim()
    {
        if (isAttacking || isShooting || isDodging) return;

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

            Animate();
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
            spriteRenderer.flipX = currentDir == Direction.Right ? spriteRenderer.flipX : false;

            Animate();
        }
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
        velocity = Vector2.zero;

        Sprite[] attackAnim = swordDown;
        Vector2 attackDir = Vector2.down;

        switch (currentDir)
        {
            case Direction.Down: attackAnim = swordDown; attackDir = Vector2.down; spriteRenderer.flipX = false; break;
            case Direction.Up: attackAnim = swordUp; attackDir = Vector2.up; spriteRenderer.flipX = false; break;
            case Direction.Right:
                attackAnim = swordRight;
                attackDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
                break;
        }

        if (currentDir == Direction.Right && input.x < 0)
            spriteRenderer.flipX = true;

        for (int i = 0; i < attackAnim.Length; i++)
        {
            spriteRenderer.sprite = attackAnim[i];
            yield return new WaitForSeconds(frameRate);
        }

        // 🎯 부채꼴 판정 시작
        float angleRange = 90f; // 부채꼴 각도
        float radius = attackRange;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
        foreach (var hit in hits)
        {
            Vector2 toEnemy = (Vector2)hit.transform.position - (Vector2)transform.position;
            float angle = Vector2.Angle(attackDir, toEnemy);

            if (angle <= angleRange * 0.5f)
            {
                if (hit.TryGetComponent(out Enemy enemy))
                    enemy.TakeDamage(swordDamage);
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator ShootArrow()
    {
        isShooting = true;

        Sprite[] bowAnim = bowDown;
        Vector2 shootDir = Vector2.down;

        switch (currentDir)
        {
            case Direction.Down: bowAnim = bowDown; shootDir = Vector2.down; spriteRenderer.flipX = false; break;
            case Direction.Up: bowAnim = bowUp; shootDir = Vector2.up; spriteRenderer.flipX = false; break;
            case Direction.Right:
                bowAnim = bowRight;
                shootDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
                break;
        }

        for (int i = 0; i < bowAnim.Length; i++)
        {
            spriteRenderer.sprite = bowAnim[i];
            if (i == 2)
            {
                GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                arrow.GetComponent<Arrow>().SetDirection(shootDir);
            }
            yield return new WaitForSeconds(frameRate);
        }

        yield return new WaitForSeconds(shootCooldown);
        isShooting = false;
    }

    IEnumerator Dodge()
    {
        isDodging = true;
        velocity = Vector2.zero;

        Vector2 dodgeDir = currentDir switch
        {
            Direction.Down => Vector2.up,
            Direction.Up => Vector2.down,
            Direction.Right => spriteRenderer.flipX ? Vector2.right : Vector2.left,
            _ => Vector2.zero
        };

        gameObject.layer = LayerMask.NameToLayer("Invincible");
        spriteRenderer.color = new Color32(0xCF, 0xCF, 0xCF, 0xFF);

        float elapsed = 0f;
        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + dodgeDir.normalized * dashDistance;

        while (elapsed < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, elapsed / dashDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos);
        isDodging = false;

        yield return new WaitForSeconds(invincibleTime - dashDuration);
        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(dodgeCooldown);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    IEnumerator DelayedInit()
    {
        while (UpgradeManager.Instance == null)
            yield return null;

        RecalculateStats();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("플레이어 사망");
            Die(); // ✅ 사망 처리 추가됨
        }
    }

    void Die()
    {
        GameManager.Instance.PlayerDied();
    }
}
