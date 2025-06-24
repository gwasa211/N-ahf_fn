using UnityEngine;
using UnityEngine.UI;

public class PortalMarkerUI : MonoBehaviour
{
    [Header("참조")]
    public Transform portal;                  // 월드 상 포탈 위치
    public RectTransform markerUI;            // 마커 UI (Image)
    public Canvas canvas;                     // UI용 캔버스

    [Header("화면 설정")]
    public float screenMargin = 30f;

    [Header("진동 효과")]
    public float floatAmplitude = 10f;  // 위아래 흔들림 크기 (픽셀)
    public float floatSpeed = 2f;       // 흔들림 속도
    public float hoverOffsetY = 20f;    // 포탈 위로 살짝 띄우는 높이

    void Update()
    {
        if (portal == null || markerUI == null || canvas == null) return;

        Camera cam = Camera.main;
        Vector2 screenPos = cam.WorldToScreenPoint(portal.position);
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        bool isOffScreen = screenPos.x < 0 || screenPos.x > Screen.width ||
                           screenPos.y < 0 || screenPos.y > Screen.height;

        Vector2 finalScreenPos;

        if (isOffScreen)
        {
            // 화면 밖 → 방향을 향하도록 회전
            finalScreenPos = ClampToScreenEdge(screenPos);
            Vector2 dir = screenPos - screenCenter;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            markerUI.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            // 화면 안 → 포탈 위에서 흔들리며 ↓방향 고정
            float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            screenPos.y += hoverOffsetY + floatOffset;
            finalScreenPos = screenPos;

            markerUI.rotation = Quaternion.Euler(0f, 0f, -90f); // ↓ 방향
        }

        // 위치 설정 (스크린 → 캔버스 로컬)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            finalScreenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 anchoredPos
        );

        markerUI.anchoredPosition = anchoredPos;
    }

    Vector2 ClampToScreenEdge(Vector2 pos)
    {
        float x = Mathf.Clamp(pos.x, screenMargin, Screen.width - screenMargin);
        float y = Mathf.Clamp(pos.y, screenMargin, Screen.height - screenMargin);
        return new Vector2(x, y);
    }
}
