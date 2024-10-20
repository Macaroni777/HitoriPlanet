using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : MonoBehaviour
{
    public Renderer HitBox1;
    public Renderer HitBox2;

    // Start is called before the first frame update
    void Start()
    {

        HitBox1.enabled = false;
        HitBox2.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
