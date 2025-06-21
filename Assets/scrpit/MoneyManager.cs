using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int currentMoney = 0;
    public TMPro.TextMeshProUGUI moneyText; // ← UI 텍스트 연결 필요

    void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateUI(); // ← 꼭 갱신 호출!
    }

    public void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = currentMoney.ToString();
    }
}
