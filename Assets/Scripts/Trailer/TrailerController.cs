using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TrailerController : MonoBehaviour
{
    public string nextSceneName = "GameScene"; // Name deiner GameScene eintragen

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnTrailerEnd; // Event bei Video-Ende
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // ENTER zum Skip
        {
            LoadNextScene();
        }
    }

    void OnTrailerEnd(VideoPlayer vp)
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}

