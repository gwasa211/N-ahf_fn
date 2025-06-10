using UnityEngine;
using System.Collections;
public class Dodge : MonoBehaviour
{
    public float dashSpeed = 5f;
    public float dashDuration = 0.2f;
    public float invincibleTime = 0.3f;

    private bool isDodging = false;
    private Rigidbody2D rb;
    private Vector2 lastMoveDir;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inputDir != Vector2.zero)
            lastMoveDir = inputDir.normalized;

        if (Input.GetKeyDown(KeyCode.C) && !isDodging)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDodging = true;
        Vector2 dashDir = -lastMoveDir;

        float startTime = Time.time;
        float endTime = startTime + dashDuration;

        // 公利 贸府 矫累
        gameObject.layer = LayerMask.NameToLayer("Invincible");

        while (Time.time < endTime)
        {
            rb.MovePosition(rb.position + dashDir * dashSpeed * Time.deltaTime);
            yield return null;
        }

        // 公利 秦力
        yield return new WaitForSeconds(invincibleTime - dashDuration);
        gameObject.layer = LayerMask.NameToLayer("Player");

        isDodging = false;
    }
}
