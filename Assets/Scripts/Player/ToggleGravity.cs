using MoreMountains.CorgiEngine;
using UnityEngine;

public class ToggleGravity : MonoBehaviour
{
    private CorgiController _characterGravity;
    private bool _gravityEnabled = true;

    private void Start()
    {
        // CharacterGravity コンポーネントへの参照を取得
        _characterGravity = GetComponent<CorgiController>();
        if (_characterGravity == null)
        {
            Debug.LogWarning("CharacterGravityコンポーネントが見つかりません。");
        }
    }

    private void Update()
    {
        // スペースキーを押したときに重力のオン・オフを切り替え
        if (Input.GetMouseButtonDown(1) && _characterGravity != null)
        {
            ToggleCharacterGravity();
        }
    }

    private void ToggleCharacterGravity()
    {
        // 重力の有効/無効を切り替え
        _gravityEnabled = !_gravityEnabled;
        if (_gravityEnabled)
        {
            Debug.Log("無重力状態");
            _characterGravity._currentGravity = 0.0f;
            //_characterGravity.SetGravityAngle(0.0f);
        }
        else
        {
            Debug.Log("重力状態");
            _characterGravity._currentGravity = -30.0f;
            //_characterGravity.SetGravityAngle(-9.81f);
        }
    }
}
