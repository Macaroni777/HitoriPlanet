using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 個別敵クラス：Snake
/// 
/// アクターが近くにいると接近する
/// 攻撃してこないが体当たりはしてくる
/// </summary>
public class Enemy_Snake : EnemyBase
{
    // Start
    void Start()
    {
    }
    /// <summary>
    /// このモンスターの居るエリアにアクターが進入した時の起動時処理(エリアアクティブ化時処理)
    /// </summary>
    public override void OnAreaActivated()
    {
        // 元々の起動時処理を実行
        base.OnAreaActivated();

    }

    // Update
    void Update()
    {
    }
}