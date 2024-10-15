using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySkillGauge : MonoBehaviour
{
    // スキルゲージUIの1個当たりの間隔
    private float skillGaugePaddingRatio = 1.1f;

    // スキルの装備個数
    int maxSkillNumber;

    // 描画にするためのコンポーネント
    Image[] skillGauge;

    // 現在のゲージの溜まっている値
    int[] currentSkillGauge;

    // 使用直前のゲージの溜まっている値
    int[] previousSkillGauge;

    // ゲージを使用する際に必要な値
    int[] requiredSkillGauge;

    protected PlayerController player;
    protected UILayout uILayout;

    /// <summary>
    /// ゲージパラメーターの初期化
    /// </summary>
    void InitializeSkillGauge()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        uILayout = GetComponent<UILayout>();
        //各スキルゲージの間隔
        maxSkillNumber = player.skillNumber;
        skillGauge = new Image[maxSkillNumber];
        currentSkillGauge = new int[maxSkillNumber];
        previousSkillGauge = new int[maxSkillNumber];
        requiredSkillGauge = new int[maxSkillNumber];

        for (int i = 0; i < skillGauge.Length; i++)
        {
            // i番目のスキルゲージを取得
            currentSkillGauge[i] = player.currentSkillGauge[i];

            // 直前のスキルの値を取得
            currentSkillGauge[i] = currentSkillGauge[i];

            // i番目のスキルに必要なゲージを取得
            requiredSkillGauge[i] = player.currentSkillGauge[i];
        }
    }

    /// <summary>
    /// 初期のスキルゲージ表示
    /// </summary>
    void DrawInitialSkillGauge()
    {
        for (int i = 0; i < skillGauge.Length; i++)
        {
            // スキルゲージの生成
            var uiSkillGauge = Instantiate((GameObject)Resources.Load("UI_SkillGauge"));

            // スキルゲージに識別するための＋1個分命名
            uiSkillGauge.name = "SkillGauge" + (i + 1);

            // Canvasを親にスキルゲージを設置
            uiSkillGauge.transform.SetParent(this.transform, false);

            // スキルゲージをi個分ずらす
            uiSkillGauge.transform.position = new Vector3(uiSkillGauge.transform.position.x + (i + 1) * skillGaugePaddingRatio, uiSkillGauge.transform.position.y, uiSkillGauge.transform.position.z);

            // スキルゲージの割り当てをし、初期のゲージが満たされている状態にする
            skillGauge[i] = uiSkillGauge.GetComponent<Image>();
            skillGauge[i].fillAmount = 0.0f;
        }
    }

    /// <summary>
    /// ゲージの描画更新
    /// </summary>
    void DrawCurrentSkillGauge()
    {
        for (int i = 0; i < skillGauge.Length; i++)
        {
            // 所持ポイント/必要ポイントの割合
            skillGauge[i].fillAmount = (float)currentSkillGauge[i] / (float)requiredSkillGauge[i];
        }
    }

    /// <summary>
    /// スキル使用時のゲージ消費描画
    /// </summary>
    /// <param name="skillNumber"></param>
    void UseSkillGauge(int skillNumber)
    {
        // ゲージが満たされていたら１本を消費できるようにする。
        if (currentSkillGauge[skillNumber] == requiredSkillGauge[skillNumber])
        {
            currentSkillGauge[skillNumber] = 0;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        InitializeSkillGauge();
        DrawInitialSkillGauge();
    }

    // Update is called once per frame
    void Update()
    {
        // マージの際に消す テスト用自動回復
        for (int i = 0; i < skillGauge.Length; i++)
        {
            if (currentSkillGauge[i] < requiredSkillGauge[i])
            {
                currentSkillGauge[i]++;
            }
        }
        for (int i = 0; i < skillGauge.Length; i++)
        {
            // 前のフレームから値が変わったかどうか？
            if (player.currentSkillGauge[i] <= 0 && previousSkillGauge[i] != player.currentSkillGauge[i])
            {
                try
                {
                    UseSkillGauge(i);
                }
                finally
                {
                    // 次の判定のために今の値を覚えておく
                    previousSkillGauge[i] = player.currentSkillGauge[i];
                }
            }
        }
        DrawCurrentSkillGauge();
        // マージの際に消す
        if (Input.GetKeyDown("1"))
        {

            UseSkillGauge(0);
        }
        if (Input.GetKeyDown("2"))
        {

            UseSkillGauge(1);
        }
    }
}
