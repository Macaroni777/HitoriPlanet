using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public float speed = 1.0f;
    private Text text;
    private float time;
    public AudioClip sound1;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        text = GameObject.Find("PushAnyKey").GetComponent<Text>();
        audioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        text.color = GetAlphaColor(text.color);
        if (Input.GetKeyDown("1"))
        {
            StartCoroutine("WaitSceneLoad", 1);
        }
        else if (Input.anyKeyDown && !Input.GetKeyDown("1"))
        {
            StartCoroutine("WaitSceneLoad", -1);
        }
    }

    Color GetAlphaColor(Color color)
    {
        time += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(time);
        return color;
    }

    private IEnumerator WaitSceneLoad(int stageid)
    {
        audioSource.PlayOneShot(sound1);
        //1秒待つ
        yield return new WaitForSeconds(1.0f);

        if (stageid == 1)
        {
            SceneManager.LoadScene("Stage1");
        }
        else if (stageid == -1)
        {
            SceneManager.LoadScene("StageSelect");
        }

    }

}
