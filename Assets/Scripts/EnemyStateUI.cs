using UnityEngine;
using TMPro;

public class EnemyStateUI : MonoBehaviour
{
    //  敵の状態を表示するためのTextMeshProUGUIコンポーネントと、EnemyMoveスクリプトを設定するための変数
    public EnemyMove enemy;
    public TextMeshProUGUI stateText;

    void Update()
    {
        stateText.text = enemy.DebugCurrentState.ToString();
    }
}