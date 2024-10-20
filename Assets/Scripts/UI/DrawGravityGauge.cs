using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawGravityGauge : MonoBehaviour
{
    public GameObject GaugeInsideUI;//ゲージ内部UIオブジェクト

    float GaugeMax = 1000.0f;//ゲージ最大値
    protected ZeroGravity zeroGravity;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;//FPSを60に固定
        zeroGravity = GameObject.Find("Player").GetComponent<ZeroGravity>();

    }

    // Update is called once per frame
    void Update()
    {

        float remaining = zeroGravity.ZeroGravityPower() / GaugeMax;
        GaugeInsideUI.GetComponent<Image>().fillAmount = remaining;
    }
}