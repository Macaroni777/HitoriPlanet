using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージマネージャクラス
/// </summary>
public class StageManager : MonoBehaviour
{
    [HideInInspector] public PlayerController actorController; // アクター制御クラス

    [Header("初期エリアのAreaManager")]
    public AreaManager initArea; // ステージ内の最初のエリア(初期エリア)

    // ステージ内の全エリアの配列(Startで取得)
    private AreaManager[] inStageAreas;

    // Start
    void Start()
    {
        // 参照取得
        actorController = GetComponentInChildren<PlayerController>();

        // ステージ内の全エリアを取得・初期化
        inStageAreas = GetComponentsInChildren<AreaManager>();
        foreach (var targetAreaManager in inStageAreas)
            targetAreaManager.Init(this);

        // 初期エリアをアクティブ化(その他のエリアは全て無効化)
        initArea.ActiveArea();
    }

    /// <summary>
    /// ステージ内の全エリアを無効化する
    /// </summary>
    public void DeactivateAllAreas()
    {
        foreach (var targetAreaManager in inStageAreas)
            targetAreaManager.gameObject.SetActive(false);
    }
}