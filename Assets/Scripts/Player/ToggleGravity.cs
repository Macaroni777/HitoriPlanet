using MoreMountains.CorgiEngine;
using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.Events;

public class ToggleGravity : CharacterAbility
{
    private new CorgiController _characterGravity;

    private CharacterBounce characterBounce;
    private bool _gravityEnabled = false;


    private void Awake()
    {
        characterBounce = GetComponent<CharacterBounce>();
        characterBounce.enabled = false;
    }

    protected override void Start()
    {
        base.Start();
        characterBounce = GetComponent<CharacterBounce>();
        characterBounce.enabled = false;
        // CharacterGravity コンポーネントへの参照を取得
        _characterGravity = GetComponent<CorgiController>();
        if (_characterGravity == null)
        {
            Debug.LogWarning("CharacterGravityコンポーネントが見つかりません。");
        }
        _characterGravity._currentGravity = -30.0f;
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
            characterBounce.enabled = true;
            _movement.ChangeState(CharacterStates.MovementStates.ZeroGavity);

            //_condition.ChangeState(CharacterStates.CharacterConditions.ZeroGavity);
        }
        else
        {
            Debug.Log("重力状態");
            _characterGravity._currentGravity = -30.0f;
            characterBounce.enabled = false;
            _movement.RestorePreviousState();
            //_condition.RestorePreviousState();

        }
    }
}
