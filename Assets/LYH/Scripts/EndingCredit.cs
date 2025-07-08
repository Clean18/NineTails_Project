using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class EndingCredit : MonoBehaviour
{
    [SerializeField] private VideoPlayer player;
    [SerializeField] private GameObject GameEndUI;
    [SerializeField] private RawImage playUI;

    private void Start()
    {
        player.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        GameEndUI.SetActive(true);
        playUI.gameObject.SetActive(false);
    }

    public void previousScene()
    {
        Debug.Log("Go to 3-2 Battle Scene.");
    }
    public void EndGame()
    {
        Debug.Log("End Game.");
    }
}
