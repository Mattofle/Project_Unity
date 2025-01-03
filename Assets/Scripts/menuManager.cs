using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject canvasMenu;     // R�f�rence au Canvas du menu principal
    public GameObject canvasOptions;  // R�f�rence au Canvas des options
    public CanvasGroup canvasMenuGroup; // R�f�rence au CanvasGroup du menu principal
    public AudioSource backgroundMusic; // R�f�rence � l'AudioSource de la musique de fond

    [Header("Buttons")]
    public Button crossButton;        // Bouton croix (mute/unmute)

    private Image crossImage;         // Image associ�e au bouton croix
    private bool isMuted = false;     // �tat du son (mut� ou non)

    void Start()
    {
        // Initialisation au d�marrage
        crossImage = crossButton.GetComponent<Image>();

        // Assure que les �tats initiaux sont corrects
        MakeCrossInvisible();
        canvasMenu.SetActive(true);
        canvasOptions.SetActive(false);

        // D�marrer le fondu
        StartCoroutine(FadeInMenu());
    }

    // Coroutine pour le fondu du menu
    private IEnumerator FadeInMenu()
    {
        // Initialiser l'alpha � 0 (invisible)
        canvasMenuGroup.alpha = 0f;
        yield return new WaitForSeconds(0.5f);


        // Faire appara�tre le menu progressivement
        float fadeDuration = 3f; // Dur�e du fondu en secondes
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            canvasMenuGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // S'assurer que l'alpha est � 1 une fois le fondu termin�
        canvasMenuGroup.alpha = 1f;
    }

    // M�THODES POUR LES MENUS

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

    // M�THODES POUR LE BOUTON CROIX (MUTE/UNMUTE)

    // Basculer l'�tat du son et la visibilit� de la croix
    public void ToggleCross()
    {
        // Inverse l'�tat du mute
        isMuted = !isMuted;

        // Mute ou unmute la musique de fond
        if (backgroundMusic != null)
        {
            backgroundMusic.mute = isMuted;
        }

        // Affiche ou cache la croix en fonction de l'�tat
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

    // AUTRES M�THODES POUR LES BOUTONS (si besoin)

    // Exemple : M�thode pour quitter le jeu
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    // Exemple : M�thode pour d�marrer le jeu
    public void StartGame()
    {
        Debug.Log("Start Game");
        // Ajoute ici le comportement pour lancer ton jeu (chargement de sc�ne, etc.)
    }
}
