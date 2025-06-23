using UnityEngine;
using System.Collections;

public class CropVisual : MonoBehaviour
{
    public Sprite[] growthStages;       // 0~3단계 작물 스프라이트
    public GameObject fruitPrefab;      // 열매 프리팹

    private int currentStage = 0;
    private SpriteRenderer sr;

    public string cropID;       // 작물 식별용 (예: 이름 또는 위치)
    public int stage;           // 성장 단계
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = growthStages[0];
    }

    public void AdvanceStage()
    {
        currentStage++;

        // 0~3단계까지만 스프라이트 변경
        if (currentStage < growthStages.Length)
        {
            sr.sprite = growthStages[currentStage];
        }
        else
        {
            sr.sprite = growthStages[growthStages.Length - 1]; // 마지막 모습 유지
        }
    }

    public void ShowFruitEffect()
    {
        if (currentStage >= growthStages.Length) // 4단계 이상일 때만
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

    public int CurrentStage => currentStage;

 
 

  

        public void SetStage(int stage)
        {
            currentStage = stage;
            sr.sprite = growthStages[Mathf.Clamp(stage, 0, growthStages.Length - 1)];
        }
    

}
