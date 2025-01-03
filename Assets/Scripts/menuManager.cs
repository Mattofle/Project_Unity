using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject canvasMenu;     // Référence au Canvas du menu principal
    public GameObject canvasOptions;  // Référence au Canvas des options
    public CanvasGroup canvasMenuGroup; // Référence au CanvasGroup du menu principal
    public AudioSource backgroundMusic; // Référence à l'AudioSource de la musique de fond

    [Header("Buttons")]
    public Button crossButton;        // Bouton croix (mute/unmute)

    private Image crossImage;         // Image associée au bouton croix
    private bool isMuted = false;     // État du son (muté ou non)

    void Start()
    {
        // Initialisation au démarrage
        crossImage = crossButton.GetComponent<Image>();

        // Assure que les états initiaux sont corrects
        MakeCrossInvisible();
        canvasMenu.SetActive(true);
        canvasOptions.SetActive(false);

        // Démarrer le fondu
        StartCoroutine(FadeInMenu());
    }

    // Coroutine pour le fondu du menu
    private IEnumerator FadeInMenu()
    {
        // Initialiser l'alpha à 0 (invisible)
        canvasMenuGroup.alpha = 0f;
        yield return new WaitForSeconds(0.5f);


        // Faire apparaître le menu progressivement
        float fadeDuration = 3f; // Durée du fondu en secondes
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            canvasMenuGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // S'assurer que l'alpha est à 1 une fois le fondu terminé
        canvasMenuGroup.alpha = 1f;
    }

    // MÉTHODES POUR LES MENUS

    // Afficher le menu des options
    public void ShowOptions()
    {
        canvasMenu.SetActive(false);
        canvasOptions.SetActive(true);
    }

    // Retourner au menu principal
    public void ShowMenu()
    {
        canvasOptions.SetActive(false);
        canvasMenu.SetActive(true);
    }

    // MÉTHODES POUR LE BOUTON CROIX (MUTE/UNMUTE)

    // Basculer l'état du son et la visibilité de la croix
    public void ToggleCross()
    {
        // Inverse l'état du mute
        isMuted = !isMuted;

        // Mute ou unmute la musique de fond
        if (backgroundMusic != null)
        {
            backgroundMusic.mute = isMuted;
        }

        // Affiche ou cache la croix en fonction de l'état
        if (isMuted)
        {
            MakeCrossVisible();
        }
        else
        {
            MakeCrossInvisible();
        }

        Debug.Log("Volume toggled: " + (isMuted ? "Muted" : "Unmuted"));
    }

    // Rendre la croix visible
    private void MakeCrossVisible()
    {
        crossImage.color = new Color(1, 1, 1, 1); // Alpha = 1 (opaque)
    }

    // Rendre la croix invisible
    private void MakeCrossInvisible()
    {
        crossImage.color = new Color(1, 1, 1, 0); // Alpha = 0 (transparent)
    }

    // AUTRES MÉTHODES POUR LES BOUTONS (si besoin)

    // Exemple : Méthode pour quitter le jeu
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    // Exemple : Méthode pour démarrer le jeu
    public void StartGame()
    {
        Debug.Log("Start Game");
        // Ajoute ici le comportement pour lancer ton jeu (chargement de scène, etc.)
    }
}
