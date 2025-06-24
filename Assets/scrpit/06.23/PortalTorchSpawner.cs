using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PortalTorchSpawner : MonoBehaviour
{
    [Header("참조")]
    public GameObject torchPrefab;
    public Transform portalCenter;

    [Header("설정")]
    public float radius = 2f;
    public float delayBetweenTorches = 0.2f;
    public int torchCount = 12;
    public string dungeonSceneName = "DungeonScene";

    private bool playerInPortal = false;
    private Coroutine spawnRoutine;
    private Coroutine removeRoutine;
    private List<GameObject> spawnedTorches = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInPortal = true;

            if (removeRoutine != null)
            {
                StopCoroutine(removeRoutine);
                removeRoutine = null;
            }

            if (spawnRoutine == null && spawnedTorches.Count < torchCount)
                spawnRoutine = StartCoroutine(SpawnTorches());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInPortal = false;

            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
            }

            if (gameObject.activeInHierarchy)
                removeRoutine = StartCoroutine(RemoveTorches());
        }
    }

    IEnumerator SpawnTorches()
    {
        int startIndex = spawnedTorches.Count;
        for (int i = startIndex; i < torchCount; i++)
        {
            if (!playerInPortal) yield break;

            float angleDeg = 90f - (i * (360f / torchCount)); // 1시 방향부터 시계방향
            float angleRad = Mathf.Deg2Rad * angleDeg;
            Vector3 pos = portalCenter.position + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;

            GameObject torch = Instantiate(torchPrefab, pos, Quaternion.identity);
            spawnedTorches.Add(torch);

            yield return new WaitForSeconds(delayBetweenTorches);
        }

        spawnRoutine = null;

        // ✅ 다 생성되면 즉시 씬 전환
        yield return new WaitForSeconds(0.5f); // 약간의 연출 여유
        if (playerInPortal)
            SceneManager.LoadScene(dungeonSceneName);
    }

    IEnumerator RemoveTorches()
    {
        while (spawnedTorches.Count > 0)
        {
            GameObject torch = spawnedTorches[spawnedTorches.Count - 1];
            spawnedTorches.RemoveAt(spawnedTorches.Count - 1);

            if (torch != null)
                Destroy(torch);

            yield return new WaitForSeconds(delayBetweenTorches);
        }

        removeRoutine = null;
    }
}
