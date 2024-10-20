using System;
using System.Collections;
using System.Collections.Generic;
using BotController;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class PlayerController : MonoBehaviour
{


    private SpriteRenderer spriteRenderer;
    private Bot bot;
    private bool rightFacing; // 向いている方向(true.右向き false:左向き)

    private float moveSpeed = 10.0f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isnuketa = false;

    private float jumpPower = 12.0f;

    private Rigidbody2D rb;

    private ActorGroundSensor groundSensor; // アクター接地判定クラス
    private float moveInputDeadZone = 0.25f;

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
    public const float KnockBack_X = 5.0f;     // 被ダメージ時ノックバック力(x方向)
    private float knockbackDuration = 1.4f;    // ノックバックの持続時間
    private float knockbackTimer = 0.0f;        // ノックバックの時間を計測
    private bool isKnockBack = false;
    public int skillNumber = 2;
    public int[] currentSkillGauge;
    RaycastHit2D previousHit;
    bool isGrounded = false;
    private Vector2 moveDirection;

    public GameObject GunObject;

    protected HitWall hitWall;
    protected ZeroGravity zeroGravity;

    public Rigidbody2D Rb()
    {
        return rb;
    }


    private Vector2 GetGroundNormal()
    {
        // 地面の法線を保存する変数
        Vector2 groundNormal = Vector2.up; // デフォルトは平坦な道（垂直）

        // 衝突している間の法線ベクトルを取得
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5f, groundLayer);
        if (hit.collider != null)
        {
            groundNormal = hit.normal; // 地面の法線を取得
        }
        return groundNormal;
    }

    //下るときに浮かないようにするメソッド
    void GoDownSlope(float moveInput)
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 5.0f, groundLayer);

        if (Math.Abs(-hit.normal.x + moveInput) < Math.Abs(hit.normal.x + moveInput))
        {
            rb.velocity += new Vector2(rb.velocity.x * 0.05f, -15.0f);
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

    void Awake()
    {
        zeroGravity = GetComponent<ZeroGravity>();
        currentSkillGauge = new int[2] { 100, 1000 };
        previousHit = Physics2D.Raycast(groundCheck.position, Vector2.down, 5.0f, groundLayer);
        hitWall = GetComponentInChildren<HitWall>();
    }

    void Start()
    {
        bot = GetComponentInChildren<Bot>();
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
        float moveInput = Input.GetAxis("Horizontal");
        moveDirection = new Vector2(-moveInput, 0).normalized;
        // 移動処理
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

        //接地、壁接触あり、移動入力あり
        if (hitWall.isTouchingGround && hitWall.isTouchingWall && Math.Abs(moveInput) > 0.0f)
        {

            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            rb.velocity += new Vector2(0.0f, -5.0f);

            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        //非接地、壁接触あり、移動入力あり
        if (!hitWall.isTouchingGround && hitWall.isTouchingWall && Math.Abs(moveInput) > 0.0f && !zeroGravity.IsZeroGravity())
        {
            rb.velocity += new Vector2(0.0f, -5.0f);
        }
        //接地、壁接触なし、移動入力あり
        if (hitWall.isTouchingGround && !hitWall.isTouchingWall && Math.Abs(moveInput) > 0.0f)
        {
            if (Math.Abs(moveInput) > moveInputDeadZone)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // 回転のみ固定
                                                                         // 坂道に沿った速度の設定
                AdjustVelocityAlongSlope();
                GoDownSlope(moveInput);
            }
        }
        //非接地、壁接触なし、移動入力あり
        if (!hitWall.isTouchingGround && !hitWall.isTouchingWall && Math.Abs(moveInput) > 0.0f && !zeroGravity.IsZeroGravity())
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        //接地、壁接触あり、移動入力なし
        if (hitWall.isTouchingGround && hitWall.isTouchingWall && Math.Abs(moveInput) <= 0.0f)
        {
            if (Math.Abs(moveInput) <= moveInputDeadZone)
            {
                rb.velocity += new Vector2(0.0f, -5.0f);

            }

        }

        //非接地、壁接触あり、移動入力なし、無重力なし
        if (!hitWall.isTouchingGround && hitWall.isTouchingWall && Math.Abs(moveInput) <= 0.0f && !zeroGravity.IsZeroGravity())
        {
            rb.velocity += new Vector2(0.0f, -5.0f);
        }

        //接地、壁接触なし、移動入力なし
        if (hitWall.isTouchingGround && !hitWall.isTouchingWall && Math.Abs(moveInput) <= 0.0f)
        {
            if (Math.Abs(moveInput) <= moveInputDeadZone)
            {
                // 移動入力がない場合はピタッと止まるようにする
                rb.velocity = new Vector2(0f, 0f);  // X軸方向の速度を0にする
                                                    // 坂道で滑らないようにX軸のみ固定する
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            }
        }

        //非接地、壁接触なし、移動入力なし
        if (!hitWall.isTouchingGround && !hitWall.isTouchingWall && Math.Abs(moveInput) <= 0.0f)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        }

        if (!hitWall.isTouchingGround && isnuketa)
        {
            rb.velocity += new Vector2(0.0f, -10.0f);
            isnuketa = false;
        }

    }

    private void AdjustVelocityAlongSlope()
    {
        // 現在の速度を坂道に沿って投影
        Vector2 groundNormal = GetGroundNormal(); // 現在接地している地面の法線ベクトルを取得
        Vector2 moveAlongSlope = Vector2.Perpendicular(groundNormal).normalized * moveDirection.x;

        rb.velocity = moveAlongSlope * moveSpeed;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            isGrounded = true;
            return;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            isGrounded = false;
            isnuketa = true;
        }
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
        // ノックバック適用
        // プレイヤーのRigidbody2Dに力を加える
        rb.AddForce(new Vector2(knockBackPower, 5.0f), ForceMode2D.Impulse);
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
        float bulletAngle = -1 * bot.GetCursorAngle() + 90.0f; // 右向き
                                                               // アクターが左向きなら弾も左向きに進む
        if (!rightFacing)
            bulletAngle = 180.0f;

        // 弾丸オブジェクト生成・設定
        GameObject obj = Instantiate( // オブジェクト新規生成
            weaponBulletPrefab,     // 生成するオブジェクトのプレハブ
            GunObject.transform.position,     // 生成したオブジェクトの初期座標
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
