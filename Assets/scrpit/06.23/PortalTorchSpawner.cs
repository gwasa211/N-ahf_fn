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
    public float delayBetweenTorches = 0.5f;
    public int torchCount = 12;
    public float holdTimeToTeleport = 6f;
    public string dungeonSceneName = "DungeonScene";

    private bool playerInPortal = false;
    private float holdTimer = 0f;
    private Coroutine torchRoutine;
    private List<GameObject> spawnedTorches = new List<GameObject>();
    private GameObject currentPlayer;

    void Update()
    {
        if (playerInPortal)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                holdTimer += Time.deltaTime;

                if (torchRoutine == null)
                    torchRoutine = StartCoroutine(SpawnTorches());

                if (holdTimer >= holdTimeToTeleport)
                {
                    SceneManager.LoadScene(dungeonSceneName);
                }
            }
            else
            {
                // W키에서 손 떼면 취소
                CancelTorchSequence();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInPortal = true;
            currentPlayer = other.gameObject;
            holdTimer = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInPortal = false;
            CancelTorchSequence();
        }
    }

    IEnumerator SpawnTorches()
    {
        Vector3 center = portalCenter.position;
        spawnedTorches.Clear();

        for (int i = 0; i < torchCount; i++)
        {
            if (!playerInPortal || !Input.GetKey(KeyCode.W)) yield break;

            float angleDeg = 30f * i - 60f;
            float angleRad = Mathf.Deg2Rad * angleDeg;
            Vector3 pos = center + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;

            GameObject torch = Instantiate(torchPrefab, pos, Quaternion.identity);
            spawnedTorches.Add(torch);

            yield return new WaitForSeconds(delayBetweenTorches);
        }
    }

    void CancelTorchSequence()
    {
        holdTimer = 0f;

        if (torchRoutine != null)
        {
            StopCoroutine(torchRoutine);
            torchRoutine = null;
        }

        foreach (var torch in spawnedTorches)
        {
            if (torch != null) Destroy(torch);
        }

        spawnedTorches.Clear();
    }
}
