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
            Time.timeScale = 0f;

            // ✅ Maus anzeigen
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            hasShown = true;
        }
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;

        // ✅ Maus wieder verstecken
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}