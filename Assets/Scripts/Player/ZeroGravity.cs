using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZeroGravity : MonoBehaviour
{
    protected PlayerController playerController;
    protected Animator animator;
    protected TilemapCollider2D tilemapCollider2D;
    private bool isZeroGravity = false;
    private float zeroGravityPower = 1000.0f;
    public PhysicsMaterial2D zeroGravityMaterial;
    public PhysicsMaterial2D normalMaterial;

    public AudioClip sound1;
    AudioSource audioSource1;
    AudioSource audioSource2;
    int currentBGM = 1;
    private bool IsFade;
    public double FadeInSeconds = 1.0;
    bool IsFadeIn = true;
    double FadeDeltaTime = 0;
    public float slowedTimeScale = 0.5f; // 全体の時間を50%にする
    public float normalTimeScale = 1f;   // 通常時の時間スケール
    public bool IsZeroGravity()
    {
        return isZeroGravity;
    }

    public float ZeroGravityPower()
    {
        return zeroGravityPower;
    }

    //無重力に関するメソッド
    void UseZeroGravity()
    {
        isZeroGravity = true;
    }

    void ZeroGravityMode()
    {
        if (isZeroGravity)
        {
            playerController.Rb().gravityScale = 0.0f;
            if (tilemapCollider2D.sharedMaterial != zeroGravityMaterial)
            {

                tilemapCollider2D.sharedMaterial = zeroGravityMaterial;
            }
            zeroGravityPower -= Time.deltaTime * 500;
            if (zeroGravityPower <= 0.0f)
            {
                playerController.Rb().gravityScale = 1.0f;
                tilemapCollider2D.sharedMaterial = normalMaterial;
                isZeroGravity = false;
            }
        }

    }

    void ZeroGravityModeBGM()
    {
        if (isZeroGravity && currentBGM != 2)
        {

            animator.SetInteger("CurrentBGM", 2);
            audioSource1.time = 0.0f;
            currentBGM = 2;
        }
        else if (!isZeroGravity)
        {
            animator.SetInteger("CurrentBGM", 1);
            currentBGM = 1;
        }
    }
    void HealZeroGravity()
    {
        zeroGravityPower += Time.deltaTime * 500;
    }

    void StopZeroGravity()
    {
        playerController.Rb().gravityScale = 1.0f;
        tilemapCollider2D.sharedMaterial = normalMaterial;
        isZeroGravity = false;
    }
    void Awake()
    {
        playerController = GetComponent<PlayerController>();

        audioSource1 = GameObject.Find("ZeroGravityBGM").GetComponent<AudioSource>();

        animator = GameObject.Find("BGMControl").GetComponent<Animator>();
        tilemapCollider2D = GameObject.Find("Tilemap").GetComponent<TilemapCollider2D>();


    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isZeroGravity)
        {
            UseZeroGravity();
        }
        else if (Input.GetMouseButtonDown(1) && isZeroGravity)
        {
            StopZeroGravity();
        }

        ZeroGravityMode();
        ZeroGravityModeBGM();

        if (!isZeroGravity && zeroGravityPower < 1000.0f)
        {
            HealZeroGravity();
        }
    }
}
