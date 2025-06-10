using UnityEngine;
using System.Collections;

public class Crop : MonoBehaviour
{
    public GameObject[] levels; // 단계별 오브젝트 (0~2: 고정, 3 이상: 이펙트성)
    int currentLevel = 0;

    public void LevelUp()
    {
        if (currentLevel < 3)
        {
            if (currentLevel > 0)
                levels[currentLevel - 1].SetActive(false);

            levels[currentLevel].SetActive(true);
            currentLevel++;
        }
        else if (currentLevel < levels.Length)
        {
            // 3단계 이상 → 위에 이펙트 연출
            StartCoroutine(AnimateFloatingCrop(levels[currentLevel]));
            currentLevel++;
        }
        else
        {
            Debug.Log("최고 레벨 도달");
        }
    }

    private IEnumerator AnimateFloatingCrop(GameObject cropObj)
    {
        cropObj.SetActive(true);

        Transform crop = cropObj.transform;
        Vector3 start = crop.localPosition;
        Vector3 end = start + Vector3.up * 1f;

        float t = 0f;
        float duration = 0.5f;

        while (t < duration)
        {
            crop.localPosition = Vector3.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        crop.localPosition = start; // 위치 복원
        cropObj.SetActive(false);
    }
}
