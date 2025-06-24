using UnityEngine;
using System.Collections;

public class CropVisual : MonoBehaviour
{
    public string cropID; // 각 작물 고유 ID
    public Sprite[] growthStages;
    public GameObject fruitPrefab;

    private int currentStage = 0;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        UpdateVisual();
    }

    public void AdvanceStage()
    {
        currentStage++;
        SetStage(currentStage);

        // 🟢 GameManager에 저장
        if (GameManager.Instance != null)
            GameManager.Instance.SetCropStage(cropID, currentStage);

    }



    public void SetStage(int stage)
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (growthStages == null || growthStages.Length == 0) return;

        currentStage = Mathf.Clamp(stage, 0, growthStages.Length - 1);
        sr.sprite = growthStages[currentStage];

        GameManager.Instance?.SetCropStage(cropID, currentStage); // 여기에 저장 연동
    }

    void UpdateVisual()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (growthStages != null && growthStages.Length > 0)
        {
            int index = Mathf.Clamp(currentStage, 0, growthStages.Length - 1);
            sr.sprite = growthStages[index];
        }

        if (currentStage >= growthStages.Length)
        {
            ShowFruitEffect();
        }
    }

    public int CurrentStage => currentStage;

    public void ShowFruitEffect()
    {
        if (currentStage >= 3) // ✅ 4단계 이상일 때만 과일 생성
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 0.5f, 0);
            GameObject fruit = Instantiate(fruitPrefab, spawnPos, Quaternion.identity);
            StartCoroutine(FruitEffect(fruit));
        }
    }

    IEnumerator FruitEffect(GameObject fruit)
    {
        Vector3 start = fruit.transform.position;
        Vector3 end = start + Vector3.up * 1f;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 2;
            fruit.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        Destroy(fruit);
    }
}
