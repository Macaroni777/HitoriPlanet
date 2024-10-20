using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    public float slowedTimeScale = 0.5f; // 全体の時間を50%にする
    public float normalTimeScale = 1f;   // 通常時の時間スケール

    void Start()
    {
        // ゲーム開始時に時間スケールを変更する
        SlowDownGame();
    }

    public void SlowDownGame()
    {
        Time.timeScale = slowedTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f; // Physics更新のスケール調整
    }

    public void ResetGameSpeed()
    {
        Time.timeScale = normalTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
