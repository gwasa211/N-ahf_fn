using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int currentMoney = 0;
    public TMPro.TextMeshProUGUI moneyText; // �� UI �ؽ�Ʈ ���� �ʿ�

    void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateUI(); // �� �� ���� ȣ��!
    }

    public void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = currentMoney.ToString();
    }
}
