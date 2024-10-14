using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインカメラ制御クラス(Main Cameraにアタッチ)
/// </summary>
public class CameraController : MonoBehaviour
{
    // オブジェクト・コンポーネント
    public Transform backGroundTransform; // 背景スプライトTransform
    public CameraMovingLimitter movingLimitter; // カメラ移動範囲制限クラス

    // 各種変数
    private Vector2 basePos; // 基点座標
    private Rect limitQuad; // 有効中のカメラ移動制限範囲

    // 定数定義
    public const float BG_Scroll_Speed = 0.5f; // 背景スクロール速度(1.0でカメラの移動量と同じ)

    // Start
    void Start()
    {
        // カメラ可動制限範囲を取得
        limitQuad = movingLimitter.GetSpriteRect();    // 可動範囲をRect型で取得
    }

    /// <summary>
    /// カメラの位置を動かす
    /// </summary>
    /// <param name="targetPos">座標</param>
    public void SetPosition(Vector2 targetPos)
    {
        basePos = targetPos;
    }

    // FixedUpdate
    private void FixedUpdate()
    {
        // カメラ移動
        Vector3 pos = transform.localPosition;
        // アクターの現在位置より少し右上を映すようにX・Y座標を補正
        pos.x = basePos.x + 5.0f; // X座標
        pos.y = basePos.y + 1.5f; // Y座標
                                  // Z座標は現在値(transform.localPosition)をそのまま使用

        // カメラ可動範囲を反映
        pos.x = Mathf.Clamp(pos.x, limitQuad.xMin, limitQuad.xMax); // x方向の可動範囲
        pos.y = Mathf.Clamp(pos.y, limitQuad.yMax, limitQuad.yMin); // y方向の可動範囲

        // 計算後のカメラ座標を反映
        transform.localPosition = Vector3.Lerp(transform.localPosition, pos, 0.08f);

        // 背景スプライト移動
        // (カメラが移動する半分の速度で背景が移動する)
        Vector3 bgPos = transform.localPosition * BG_Scroll_Speed;
        bgPos.z = backGroundTransform.position.z;
        backGroundTransform.position = bgPos;
    }
}