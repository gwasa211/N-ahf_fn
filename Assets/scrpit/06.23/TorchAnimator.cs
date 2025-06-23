using UnityEngine;

public class TorchAnimator : MonoBehaviour
{
    public Sprite[] flameFrames;
    public float frameRate = 0.1f;

    private SpriteRenderer sr;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % flameFrames.Length;
            sr.sprite = flameFrames[currentIndex];
        }
    }
}
