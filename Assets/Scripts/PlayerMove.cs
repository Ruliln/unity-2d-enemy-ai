using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    // プレイヤーの移動速度とジャンプ力を設定するための変数
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    // ゲームオーバーやクリアのメッセージを表示するためのTextMeshProUGUIコンポーネントを設定するための変数
    public TextMeshProUGUI resultText;

    // ジャンプ中かどうかを判定するための変数
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        // Rigidbody2Dコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        transform.position += new Vector3(moveX * moveSpeed * Time.deltaTime, 0, 0);

        // スペースキーが押され、プレイヤーが地面にいる場合にジャンプする
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // ゲームオーバーやクリアの状態でRキーが押された場合にシーンをリロードする
        if (Time.timeScale == 0f && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    //  プレイヤーが地面に接触しているかどうかを判定するためのコールバック関数
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //  地面に接触した場合、isGroundedをtrueに設定
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    //  プレイヤーが地面から離れたときにisGroundedをfalseに設定するためのコールバック関数
    private void OnCollisionExit2D(Collision2D collision)
    {
        //  地面から離れた場合、isGroundedをfalseに設定
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    //  プレイヤーが敵やゴールに接触したときの処理を行うためのコールバック関数
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Enemy"))
        //{
        //    resultText.text = "Game Over";
        //    Time.timeScale = 0f;
        //}

        if (collision.CompareTag("Goal"))
        {
            resultText.text = "Clear!";
            Time.timeScale = 0f;
        }
    }
}