using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    //  プレイヤーの位置を取得するための変数
    public Transform player;

    //  デバッグ用に現在の敵の状態を表示するためのプロパティ
    public string DebugCurrentState => currentState.ToString();

    public float moveSpeed = 2f;
    public float chaseDistance = 4f;

    public float leftLimit = 2f;
    public float rightLimit = 6f;

    public int attackDamage = 1;
    public float attackDistance = 2.2f;

    //  敵がプレイヤーを視認できる範囲を設定するための変数
    public float viewAngle = 90f;

    //  攻撃のクールダウン時間を設定するための変数
    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    //  プレイヤーを視認できなくなったときに警戒状態になるまでの時間を設定するための変数
    public float alertTime = 2f;
    private float alertTimer = 0f;

    //  プレイヤーを視認できなくなったときの警戒状態で、敵が最後にプレイヤーを見た位置を記憶するための変数
    private Vector3 lastKnownPlayerPosition;
    public float alertMoveStopDistance = 0.2f;

    //  敵が現在右に移動しているかどうかを判定するための変数
    private bool movingRight = true;

    //  敵が現在右を向いているかどうかを判定するための変数
    private bool facingRight = true;

    //  敵の状態を定義するための列挙型
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Alert
    }

    //  敵がプレイヤーを視認できるかどうかを判定する関数
    bool CanSeePlayer()
    {
        //  敵からプレイヤーへの方向ベクトルを計算し、正規化する
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 forwardDirection = facingRight ? Vector2.right : Vector2.left;

        //  敵の正面とプレイヤーへの方向のなす角を計算する
        float angle = Vector2.Angle(forwardDirection, directionToPlayer);

        //  なす角が視認範囲の半分より大きい場合、プレイヤーは視認できないと判断する
        if (angle > viewAngle / 2f)
            return false;

        //  敵からプレイヤーへの方向にレイを飛ばして、障害物がないか確認する
        Vector2 rayStart = (Vector2)transform.position + directionToPlayer * 1.0f;

        //  デバッグ用にレイを表示する
        Debug.DrawRay(rayStart, directionToPlayer * chaseDistance, Color.green);

        //  レイキャストを使用して、敵からプレイヤーへの方向に障害物がないか確認する
        RaycastHit2D hit = Physics2D.Raycast(
            rayStart,
            directionToPlayer,
            chaseDistance
        );

        //  レイが何かに当たった場合、そのオブジェクトがプレイヤーかどうかを確認する
        if (hit.collider != null)
        {
            //Debug.Log(hit.collider.name);

            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    //  敵の現在の状態を保持するための変数
    EnemyState currentState = EnemyState.Patrol;

    void Update()
    {
        //  攻撃のクールダウンタイマーを更新
        attackTimer += Time.deltaTime;

        //  プレイヤーとの距離を計算
        float distance = Vector2.Distance(transform.position, player.position);

        //  プレイヤーとの距離に応じて敵の状態を切り替える
        if (distance <= attackDistance)
        {
            currentState = EnemyState.Attack;
            alertTimer = alertTime;
            lastKnownPlayerPosition = player.position;
        }
        else if (distance <= chaseDistance && CanSeePlayer())
        {
            currentState = EnemyState.Chase;
            alertTimer = alertTime;
            lastKnownPlayerPosition = player.position;
        }
        else
        {
            if (alertTimer > 0f)
            {
                currentState = EnemyState.Alert;
                alertTimer -= Time.deltaTime;
            }
            else
            {
                currentState = EnemyState.Patrol;
            }
        }

        //  現在の状態に応じて行動を切り替える
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Alert:
                Alert();
                break;
        }

        //  デバッグ用にプレイヤーとの距離を表示
        //Debug.Log("Distance: " + distance);
    }

    void Patrol()
    {
        //  敵が左右に移動するパトロール行動を実装
        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;

            if (transform.position.x >= rightLimit)
            {
                movingRight = false;
                facingRight = false;
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (transform.position.x <= leftLimit)
            {
                movingRight = true;
                facingRight = true;
            }
        }
    }

    //  プレイヤーを追跡する行動を実装
    void Chase()
    {
        //  プレイヤーの方向を計算して敵を移動させる
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // プレイヤーの方向に応じて敵の向きを変更する
        if (direction.x > 0)
        {
            facingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            facingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    //  プレイヤーを攻撃する行動を実装
    void Attack()
    {
        //  敵がプレイヤーを攻撃する際の向きを調整する
        Vector3 scale = transform.localScale;

        //  プレイヤーの位置に応じて敵の向きを変更する
        if (player.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        //  敵の向きを更新する
        transform.localScale = scale;

        //  攻撃のクールダウンが経過している場合、プレイヤーにダメージを与える
        if (attackTimer >= attackCooldown)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            attackTimer = 0f;
        }
    }

    //  プレイヤーを視認できなくなったときの警戒状態を実装
    void Alert()
    {
        //  敵が最後にプレイヤーを見た位置に向かって移動する
        float distanceToLastKnownPos = Vector2.Distance(transform.position, lastKnownPlayerPosition);

        //  敵が最後にプレイヤーを見た位置に十分近づいていない場合、そこに向かって移動する
        if (distanceToLastKnownPos > alertMoveStopDistance)
        {
            //  プレイヤーの最後の位置への方向を計算して敵を移動させる
            Vector3 direction = (lastKnownPlayerPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            //  プレイヤーの最後の位置への方向に応じて敵の向きを変更する
            if (direction.x > 0)
            {
                facingRight = true;
            }
            else if (direction.x < 0)
            {
                facingRight = false;
            }
        }
    }

    //  敵の視認範囲と攻撃範囲をシーンビューに表示するための関数
    void OnDrawGizmosSelected()
    {
        //  敵の位置を中心に、視認範囲と攻撃範囲をワイヤーフレームの円で表示する
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        //  敵の位置を中心に、攻撃範囲をワイヤーフレームの円で表示する
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
