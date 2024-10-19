using UnityEngine;

public class HitWall : MonoBehaviour
{
    // 地面と壁に接触しているかどうかの状態を保持する変数
    public bool isTouchingGround = false;
    public bool isTouchingWall = false;

    // OnCollisionStay2Dで接触しているタグを検出する
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isTouchingGround = true;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }
    }

    // OnCollisionExit2Dで接触が解除された際の処理
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isTouchingGround = false;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }
    }

    private void Update()
    {
        // デバッグ用に現在の状態を確認
        Debug.Log($"Ground: {isTouchingGround}, Wall: {isTouchingWall}");

        // 両方に触れているかどうかを判定
        if (isTouchingGround && isTouchingWall)
        {
            // 両方に触れている場合の処理
            Debug.Log("地面と壁の両方に触れています！");
        }
    }
}
