using UnityEngine;
using UnityEngine.UI;

public class PortalMarkerUI : MonoBehaviour
{
    [Header("����")]
    public Transform portal;                  // ���� �� ��Ż ��ġ
    public RectTransform markerUI;            // ��Ŀ UI (Image)
    public Canvas canvas;                     // UI�� ĵ����

    [Header("ȭ�� ����")]
    public float screenMargin = 30f;

    [Header("���� ȿ��")]
    public float floatAmplitude = 10f;  // ���Ʒ� ��鸲 ũ�� (�ȼ�)
    public float floatSpeed = 2f;       // ��鸲 �ӵ�
    public float hoverOffsetY = 20f;    // ��Ż ���� ��¦ ���� ����

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
            // ȭ�� �� �� ������ ���ϵ��� ȸ��
            finalScreenPos = ClampToScreenEdge(screenPos);
            Vector2 dir = screenPos - screenCenter;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            markerUI.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            // ȭ�� �� �� ��Ż ������ ��鸮�� ����� ����
            float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            screenPos.y += hoverOffsetY + floatOffset;
            finalScreenPos = screenPos;

            markerUI.rotation = Quaternion.Euler(0f, 0f, -90f); // �� ����
        }

        // ��ġ ���� (��ũ�� �� ĵ���� ����)
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
