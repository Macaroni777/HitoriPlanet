using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawHP : MonoBehaviour
{
    // ハートの間隔
    private float heartPaddingRatio = 1.0f;
    // 値が変わる前のHP
    private int previousHp;
    protected PlayerController player;

    void InitializeDrawHP(int _currentHp)
    {
        for (int i = 0; i < _currentHp; i++)
        {
            // ハートの生成
            var heart = Instantiate((GameObject)Resources.Load("UI_Hp"));

            // ハートに識別するための＋1個分命名
            heart.name = "heart" + (i + 1);

            // Canvasを親にハートを設置
            heart.transform.SetParent(this.transform, false);

            // ハートをi個分ずらす
            heart.transform.position = new Vector3(heart.transform.position.x + (i + 1) * heartPaddingRatio, heart.transform.position.y, heart.transform.position.z);
        }
    }


    // HPを増やす表示。
    void IncreaseHp(int _previousHp, int _currentHp)
    {
        for (int i = _previousHp; i < _currentHp; i++)
        {
            // 0以下は描画しない
            if (i < 0)
            {
                continue;
            }

            // ハートの生成
            var heart = Instantiate((GameObject)Resources.Load("UI_Hp"));

            // ハートに識別するための＋1個分命名
            heart.name = "heart" + (i + 1);

            // Canvasを親にハートを設置
            heart.transform.SetParent(this.transform, false);

            // ハートをi個分ずらす
            heart.transform.position = new Vector3(heart.transform.position.x + (i + 1) * heartPaddingRatio, heart.transform.position.y, heart.transform.position.z);
        }
    }

    // HPの消滅
    void DecreaseHp(int _previousHp, int _currentHp)
    {
        for (int i = _previousHp; i > _currentHp; i--)
        {
            // ハートを減らす
            Destroy(GameObject.Find("heart" + i));
            if (i < 0)
            {
                break;
            }
        }
    }

    // HPの表示を変動させる。
    void ChangeHp(int _previousHp, int _currentHp)
    {
        if (_previousHp < _currentHp)
        {
            IncreaseHp(_previousHp, _currentHp);
        }

        if (_previousHp > _currentHp)
        {
            DecreaseHp(_previousHp, _currentHp);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        previousHp = player.nowHP;
        InitializeDrawHP(previousHp);
    }

    // Update is called once per frame
    void Update()
    {
        // player.GetSetHpが増減するたびに呼び出すようにする。

        // 前のフレームから値が変わったかどうか？
        if (previousHp != player.nowHP)
        {
            try
            {
                ChangeHp(previousHp, player.nowHP); // 変数が変わったときの処理の呼び出し
            }
            finally
            {
                // 次の判定のために今の値を覚えておく
                previousHp = player.nowHP;
            }
        }

        // テスト用です。削除予定
        if (Input.GetKeyDown("u"))
        {
            // nポイント回復させる。
            int healHP = 2;
            player.nowHP += healHP;
        }

        // テスト用です。削除予定
        if (Input.GetKeyDown("j"))
        {
            // nポイントダメージを受ける。
            int damageHP = 2;
            player.nowHP -= damageHP;
        }
    }
}
