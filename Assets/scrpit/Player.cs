using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    // ===================== 이동 및 애니메이션 관련 변수 =====================
    public float moveSpeed = 2f; // 기본 이동 속도
    public SpriteRenderer spriteRenderer; // 스프라이트 렌더러 참조

    // 방향별 애니메이션 스프라이트 배열
    public Sprite[] walkDown, walkUp, walkRight; // 걷기 애니메이션
    public Sprite[] idleDown, idleUp, idleRight; // 대기 애니메이션
    public Sprite[] swordDown, swordUp, swordRight; // 검 공격 애니메이션
    public Sprite[] bowDown, bowUp, bowRight; // 활 애니메이션

    // ========================== 전투 관련 변수 ==============================
    public GameObject arrowPrefab; // 화살 프리팹
    public LayerMask enemyLayer; // 적을 식별하기 위한 레이어 마스크

    public float attackCooldown = 0.25f; //  공격 쿨타임 줄임
    public float shootCooldown = 0.25f;  //  활 쿨타임 줄임
    public float dashDistance = 2f; // 회피 이동 거리
    public float dashDuration = 0.15f; // 회피 지속 시간
    public float dodgeCooldown = 1f; // 회피 쿨타임
    public float shootMoveSpeed = 3f; // 활 사용 시 이동 속도
    public float swordMoveSpeed = 3f; // 검 사용 시 이동 속도

    // ========================= 기본 능력치 ============================
    public int baseMaxHealth = 10;
    public int baseSwordDamage = 3;
    public float baseSwordRange = 1f;
    public int baseBowDamage = 2;
    public float baseDodgeInvincibleTime = 0.3f;
    public int basePierceCount = 1;

    // ======================== 실제 능력치 ============================
    public int maxHealth;
    public int swordDamage;
    public float attackRange;
    public int bowDamage;
    public float invincibleTime;
    public int pierceCount;

    // ==================== 보너스 업그레이드 수치 ====================
    public float bonusSwordDamage;
    public float bonusSwordRange;
    public float bonusBowDamage;
    public float bonusPierceCount;
    public float bonusMaxHealth;
    public float bonusInvincibleTime;

    // ==================== 상태 및 애니메이션 관리 ===================
    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 velocity;

    private float frameTimer = 0f;
    private int frameIndex = 0;
    private float frameRate = 0.2f;
    private Sprite[] currentAnim;

    private enum Direction { Down, Up, Left, Right }
    private Direction currentDir = Direction.Down;

    private bool isAttacking = false;
    private bool isShooting = false;
    private bool isDodging = false;
    public int currentHealth;
    private bool isInvincible = false;
    public float knockbackForce = 5f;

    public GameObject attackVisualizer;
    public GameObject fanMeshObject;

    public LayerMask wallLayer; // Inspector에서 설정

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentDir = Direction.Down;  // 기본 방향
        SetIdleAnimation();           // 초기 애니메이션 설정

        RecalculateStats();
        currentHealth = maxHealth;
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void RecalculateStats()
    {
        maxHealth = baseMaxHealth + (int)bonusMaxHealth;
        swordDamage = baseSwordDamage + (int)bonusSwordDamage;
        attackRange = baseSwordRange + bonusSwordRange;
        bowDamage = baseBowDamage + (int)bonusBowDamage;
        pierceCount = basePierceCount + (int)bonusPierceCount;
        invincibleTime = baseDodgeInvincibleTime + bonusInvincibleTime;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void Update()
    {
        bool isBusy = isAttacking || isShooting || isDodging;

        if (!isDodging)
        {
            HandleInput();
            HandleMovementAnim(); // 입력과 방향은 항상 최신화 (걷든 말든)
        }

        if (Input.GetKeyDown(KeyCode.S) && !isBusy) StartCoroutine(SwordAttack());
        if (Input.GetKeyDown(KeyCode.D) && !isBusy) StartCoroutine(ShootArrow());
        if (Input.GetKeyDown(KeyCode.A) && !isBusy) StartCoroutine(Dodge());
        if (Input.GetKeyDown(KeyCode.P)) GameManager.Instance.AddMoney(1000);
        if (Input.GetKeyDown(KeyCode.O)) UseHealthSkill();

        if (!isBusy)
        {
            if (input != Vector2.zero)
            {
                switch (currentDir)
                {
                    case Direction.Down: currentAnim = walkDown; break;
                    case Direction.Up: currentAnim = walkUp; break;
                    case Direction.Left:
                    case Direction.Right: currentAnim = walkRight; break;
                }
            }
            else
            {
                SetIdleAnimation();
            }
            Animate();
        }

        transform.rotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        if (isDodging) return;

        float currentSpeed = isAttacking ? swordMoveSpeed :
                             isShooting ? shootMoveSpeed : moveSpeed;

        Vector2 moveDir = input.normalized;
        Vector2 moveAmount = moveDir * currentSpeed * Time.fixedDeltaTime;
        Vector2 finalMove = Vector2.zero;

        float radius = 0.1f;

        if (input != Vector2.zero)
        {
            if (Mathf.Abs(moveAmount.x) > Mathf.Epsilon)
            {
                bool blockedX = Physics2D.CircleCast(rb.position, radius, new Vector2(moveDir.x, 0f), Mathf.Abs(moveAmount.x), wallLayer);
                if (!blockedX)
                    finalMove.x = moveAmount.x;
            }
            if (Mathf.Abs(moveAmount.y) > Mathf.Epsilon)
            {
                bool blockedY = Physics2D.CircleCast(rb.position, radius, new Vector2(0f, moveDir.y), Mathf.Abs(moveAmount.y), wallLayer);
                if (!blockedY)
                    finalMove.y = moveAmount.y;
            }
        }

        if (finalMove.sqrMagnitude > 0.0001f)
        {
            rb.MovePosition(rb.position + finalMove);
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
        if (isAttacking || isShooting || isDodging || input == Vector2.zero)
            return;

        Direction previousDir = currentDir;

        if (Mathf.Abs(input.y) > Mathf.Abs(input.x) + 0.05f)
        {
            currentDir = input.y > 0 ? Direction.Up : Direction.Down;
            spriteRenderer.flipX = false;
        }
        else if (Mathf.Abs(input.x) > Mathf.Abs(input.y) + 0.05f)
        {
            if (input.x > 0)
            {
                currentDir = Direction.Right;
                spriteRenderer.flipX = false;
            }
            else if (input.x < 0)
            {
                currentDir = Direction.Left;
                spriteRenderer.flipX = true;
            }
        }

        if (currentDir != previousDir)
        {
            switch (currentDir)
            {
                case Direction.Down: currentAnim = walkDown; break;
                case Direction.Up: currentAnim = walkUp; break;
                case Direction.Left:
                case Direction.Right: currentAnim = walkRight; break;
            }
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

    void SetIdleAnimation()
    {
        switch (currentDir)
        {
            case Direction.Down: currentAnim = idleDown; spriteRenderer.flipX = false; break;
            case Direction.Up: currentAnim = idleUp; spriteRenderer.flipX = false; break;
            case Direction.Left: currentAnim = idleRight; spriteRenderer.flipX = true; break;
            case Direction.Right: currentAnim = idleRight; spriteRenderer.flipX = false; break;
        }
    }

    public void ApplyUpgrade(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.MeleeDamage: bonusSwordDamage += value; break;
            case StatType.MeleeRange: bonusSwordRange += value; break;
            case StatType.RangedDamage: bonusBowDamage += value; break;
            case StatType.RangedPierce: bonusPierceCount += value; break;
            case StatType.MaxHealth: bonusMaxHealth += value; break;
            case StatType.InvincibleTime: bonusInvincibleTime += value; break;
        }
    }

    void UseHealthSkill()
    {
        int damage = 100;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        GameManager.Instance.PlayerDied();
        Destroy(gameObject);
    }

    public void TakeDamage(int amount, Vector2 knockbackDir)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);

        StartCoroutine(BecomeTemporarilyInvincible());

        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir.normalized * knockbackForce, ForceMode2D.Impulse);

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        spriteRenderer.color = new Color32(200, 200, 200, 255);

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = Color.white;
    }

    IEnumerator SwordAttack() { /* 생략됨 - 위와 동일 */ yield break; }
    IEnumerator ShootArrow() { /* 생략됨 - 위와 동일 */ yield break; }
    IEnumerator Dodge() { /* 생략됨 - 위와 동일 */ yield break; }

    private void OnDrawGizmosSelected() { /* 생략됨 - 위와 동일 */ }
}
