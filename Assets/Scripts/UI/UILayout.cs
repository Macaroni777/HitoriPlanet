using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class UILayout : MonoBehaviour
{
    float heartPaddingRatio;
    float skillGaugePaddingRatio;

    //プロパティ
    public float HeartPaddingRatio() { return heartPaddingRatio; }
    public float SkillGaugePaddingRatio() { return skillGaugePaddingRatio; }

    // Start is called before the first frame update
    void Awake()
    {
        heartPaddingRatio = 1.0f;
        skillGaugePaddingRatio = 1.3f;
    }

}
