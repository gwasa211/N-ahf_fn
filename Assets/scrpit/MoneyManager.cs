using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int currentMoney = 0;
    public TMP_Text moneyText;  // ¿¡µðÅÍ¿¡¼­ ¿¬°á

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    public void SpendMoney(int amount)
    {
        currentMoney -= amount;
        UpdateMoneyUI();
    }

    public void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"È¹µæÇÑ µ·: {currentMoney:N0}";
    }

    public bool HasEnoughMoney(int amount)
    {
        return currentMoney >= amount;
    }
}
