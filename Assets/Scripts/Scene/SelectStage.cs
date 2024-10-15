using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージセレクト画面管理クラス
/// </summary>
public class SelectStage : MonoBehaviour
{
    // ステージ選択ボタンリスト
    [SerializeField] private List<Image> stageSelectButtonImages = null;

    // Start
    void Start()
    {
    }

    /// <summary>
    /// ステージ選択ボタン押下時処理
    /// </summary>
    /// <param name="bossID">該当ボスID</param>
    public void OnStageSelectButtonPressed(int StageID)
    {
        // シーン切り替え
        SceneManager.LoadScene(StageID + 1);
    }
}