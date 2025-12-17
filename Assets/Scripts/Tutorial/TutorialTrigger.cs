using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("UI Panel für Tutorial")]
    public GameObject tutorialPanel;

    private bool hasShown = false; // merkt sich ob schon angezeigt

    private void OnTriggerEnter(Collider other)
    {
        if (!hasShown && other.CompareTag("Player"))
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f; // optional pausieren
            hasShown = true;
        }
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}