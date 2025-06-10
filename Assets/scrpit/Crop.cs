using UnityEngine;
using System.Collections;

public class Crop : MonoBehaviour
{
    public GameObject[] levels; // �ܰ躰 ������Ʈ (0~2: ����, 3 �̻�: ����Ʈ��)
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
            // 3�ܰ� �̻� �� ���� ����Ʈ ����
            StartCoroutine(AnimateFloatingCrop(levels[currentLevel]));
            currentLevel++;
        }
        else
        {
            Debug.Log("�ְ� ���� ����");
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

        crop.localPosition = start; // ��ġ ����
        cropObj.SetActive(false);
    }
}
