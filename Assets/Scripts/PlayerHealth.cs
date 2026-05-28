using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHp = 3;
    public int currentHp;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI resultText;

    void Start()
    {
        currentHp = maxHp;
        UpdateHpUI();
    }

    public void TakeDamage(int damage)
    {
        // ダメージを受けたときにHPを減らす
        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
        }

        UpdateHpUI();

        // HPが0以下になったときにゲームオーバーのメッセージを表示し、ゲームを停止する
        if (currentHp <= 0)
        {
            resultText.text = "Game Over";
            Time.timeScale = 0f;
        }
    }

    void UpdateHpUI()
    {
        hpText.text = "HP: " + currentHp;
    }
}