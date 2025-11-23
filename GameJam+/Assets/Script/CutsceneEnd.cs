using UnityEngine;
using UnityEngine.Video;

public class CutsceneEnd : MonoBehaviour
{
    public VideoPlayer player;

    private void Start()
    {
        player.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        TransitionManager.Instance.FadeOutAndLoadScene("Main Menu");
    }
}
