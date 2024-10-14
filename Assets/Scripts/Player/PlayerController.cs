using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : MonoBehaviour
{


    private SpriteRenderer spriteRenderer;
    public bool rightFacing; // 向いている方向(true.右向き false:左向き)

    public float moveSpeed = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public float jumpPower = 20.0f;

    private Rigidbody2D rb;

    private ActorGroundSensor groundSensor; // アクター接地判定クラス

    float speedMultiplier;
    public float moveInputDeadZone = 0.2f;

    private float remainJumpTime;   // 空中でのジャンプ入力残り受付時間

    public CameraController cameraController; // カメラ制御クラス

    public GameObject weaponBulletPrefab; // 弾プレハブ

    // 体力変数
    [HideInInspector] public int nowHP; // 現在HP
    [HideInInspector] public int maxHP; // 最大HP
    // その他変数
    private float remainStuckTime; // 残り硬直時間(0以上だと行動できない)
    private float invincibleTime;   // 残り無敵時間(秒)
    [HideInInspector] public bool isDefeat; // true:撃破された(ゲームオーバー)

    // 定数定義
    private const int InitialHP = 5;           // 初期HP(最大HP)
    private const float InvicibleTime = 2.0f;   // 被ダメージ直後の無敵時間(秒)
    private const float StuckTime = 0.5f;       // 被ダメージ直後の硬直時間(秒)
    public const float KnockBack_X = 20.0f;     // 被ダメージ時ノックバック力(x方向)
    public float knockbackForce = 5f;         // ノックバックの強さ
    public float knockbackDuration = 1.0f;    // ノックバックの持続時間
    private float knockbackTimer = 0.0f;        // ノックバックの時間を計測
    public bool isKnockBack = false;

    void AdjustSpeedOnSlope(float moveInput)
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 5f, groundLayer);

        if (groundSensor.isGround)
        {

            // 坂道の法線と同じ向きならば、足した時の絶対値が多くなる。絶対値が多い方で下っていることを検出する
            if (Math.Abs(-hit.normal.x + moveInput) < Math.Abs(hit.normal.x + moveInput))
            {
                Debug.Log("下り坂を検出しました！");
                rb.velocity += new Vector2(0.0f, -0.5f);
            }
        }
    }

    private void JumpUpdate()
    {
        // 空中でのジャンプ入力受付時間減少
        if (remainJumpTime > 0.0f)
            remainJumpTime -= Time.deltaTime;

        // ジャンプ操作
        if (Input.GetKeyDown(KeyCode.Space))
        {// ジャンプ開始
         // 接地していないなら終了
            if (!groundSensor.isGround)
                return;
            // ジャンプ力を適用
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);

            // 空中でのジャンプ入力受け付け時間設定
            remainJumpTime = 0.25f;
        }
        else if (Input.GetKey(KeyCode.Space))
        {// ジャンプ中（ジャンプ入力を長押しすると継続して上昇する処理）
         // 空中でのジャンプ入力受け付け時間が残ってないなら終了
            if (remainJumpTime <= 0.0f)
                return;
            // 接地しているなら終了
            if (groundSensor.isGround)
                return;

            // ジャンプ力加算量を計算
            float jumpAddPower = 50.0f * Time.deltaTime; // Update()は呼び出し間隔が異なるのでTime.deltaTimeが必要
                                                         // ジャンプ力加算を適用
            rb.velocity += new Vector2(0.0f, jumpAddPower);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {// ジャンプ入力終了
            remainJumpTime = -1.0f;
        }
    }

    void Start()
    {
        PhysicsMaterial2D lowFrictionMaterial = new PhysicsMaterial2D();

        groundSensor = GetComponentInChildren<ActorGroundSensor>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        // 変数初期化
        rightFacing = true; // 最初は右向き
        lowFrictionMaterial.friction = 10f; // 低摩擦

        nowHP = maxHP = InitialHP; // 初期HP

        // カメラ初期位置
        cameraController.SetPosition(transform.position);
    }

    void Update()
    {
        if (!isKnockBack)
        {
            Move();
        }
        else
        {
            Debug.Log("isKnockBack:" + isKnockBack);
            // ノックバック時間を計測して、経過したら通常の操作に戻す
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockBack = false;
            }
        }
        // ジャンプ処理
        JumpUpdate();
        // 攻撃入力処理
        StartShotAction();

        // カメラに自身の座標を渡す
        cameraController.SetPosition(transform.position);

        // 撃破された後なら終了
        if (isDefeat)
            return;

        // 無敵時間が残っているなら減少
        if (invincibleTime > 0.0f)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0.0f)
            {// 無敵時間終了時処理
                // 点滅終了
            }
        }
        // 硬直時間処理
        if (remainStuckTime > 0.0f)
        {// 硬直時間減少
            remainStuckTime -= Time.deltaTime;
            if (remainStuckTime <= 0.0f)
            {// スタン時間終了時処理
            }
            else
                return;
        }
    }


    public void Move()
    {
        // 移動処理
        float moveInput = Input.GetAxis("Horizontal");
        if (moveInput > 0.0f)
        {

            // 右向きフラグon
            rightFacing = true;

            // スプライトを通常の向きで表示
            spriteRenderer.flipX = false;
        }
        if (moveInput < 0.0f)
        {
            // 右向きフラグoff
            rightFacing = false;

            // スプライトを左右反転した向きで表示
            spriteRenderer.flipX = true;
        }

        if (groundSensor.isGround)
        {
            if (Math.Abs(moveInput) > moveInputDeadZone)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // 回転のみ固定
                rb.velocity = new Vector2(moveSpeed * moveInput, rb.velocity.y);
                AdjustSpeedOnSlope(moveInput);  // 坂道の速度調整
            }
            else
            {
                // 移動入力がない場合はピタッと止まるようにする
                rb.velocity = new Vector2(0f, rb.velocity.y);  // X軸方向の速度を0にする
                // 坂道で滑らないようにX軸のみ固定する
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
        }
        else
        {
            // 空中にいるときは制約を解除して自然な挙動にする
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }






        /*
                // 坂道を登る処理
                if ((!isJumping & groundSensor.isGround & moveInput != 0) || (isJumping & !groundSensor.isGround & moveInput == 0))
                {

                    // 位置固定解除
                    rb.constraints = RigidbodyConstraints2D.None;
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    rb.velocity = new Vector2(moveSpeed * moveInput, rb.velocity.y);
                    // 地面と接地している場合、速度を調整
                    AdjustSpeedOnSlope(moveInput);

                }
                if (!isJumping & groundSensor.isGround & moveInput == 0)
                {
                    // 回転、位置ともに固定
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                }
        */


    }

    #region 戦闘関連
    /// <summary>
	/// ダメージを受ける際に呼び出される
	/// </summary>
	/// <param name="damage">ダメージ量</param>
	public void Damaged(int damage)
    {
        // プレイヤーの移動を無効にしてノックバックを適用
        isKnockBack = true;
        knockbackTimer = knockbackDuration;

        // 撃破された後なら終了
        if (isDefeat)
            return;

        // もし無敵時間中ならダメージ無効
        if (invincibleTime > 0.0f)
            return;

        // ダメージ処理
        nowHP -= damage;

        // HP0ならゲームオーバー処理
        if (nowHP <= 0)
        {
            isDefeat = true;
            // 被撃破演出開始
            Destroy(this.gameObject);
            // 物理演算を停止
            rb.velocity = Vector2.zero;
            moveSpeed = 0.0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            return;
        }

        // スタン硬直
        remainStuckTime = StuckTime;

        // ノックバック処理
        // ノックバック力・方向決定
        float knockBackPower = KnockBack_X;
        if (rightFacing)
            knockBackPower *= -1.0f;
        Debug.Log("Test" + knockBackPower);
        // ノックバック適用
        // プレイヤーのRigidbody2Dに力を加える
        rb.AddForce(new Vector2(knockBackPower, 30.0f), ForceMode2D.Impulse);
        // 無敵時間発生
        invincibleTime = InvicibleTime;
        if (invincibleTime > 0.0f) { }
    }
    /// <summary>
    /// 攻撃ボタン入力時処理
    /// </summary>
    public void StartShotAction()
    {
        // 攻撃ボタンが入力されていないなら終了
        if (!Input.GetKeyDown(KeyCode.Z))
            return;

        // このメソッド内で選択武器別のメソッドの呼び分けやエネルギー消費処理を行う。
        // 現在は初期武器のみなのでShotAction_Normalを呼び出すだけ
        ShotAction_Normal();
    }

    /// <summary>
    /// 射撃アクション：通常攻撃
    /// </summary>
    private void ShotAction_Normal()
    {
        // 弾の方向を取得
        float bulletAngle = 0.0f; // 右向き
                                  // アクターが左向きなら弾も左向きに進む
        if (!rightFacing)
            bulletAngle = 180.0f;

        // 弾丸オブジェクト生成・設定
        GameObject obj = Instantiate( // オブジェクト新規生成
            weaponBulletPrefab,     // 生成するオブジェクトのプレハブ
            transform.position,     // 生成したオブジェクトの初期座標
            Quaternion.identity);   // 初期Rotation(傾き)
                                    // 弾丸設定
        obj.GetComponent<ActorNormalShot>().Init(
            12.0f,      // 速度
            bulletAngle,// 角度
            1,          // ダメージ量
            5.0f);      // 存在時間
    }
    #endregion

}
