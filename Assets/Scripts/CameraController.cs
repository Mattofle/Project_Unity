using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera menuCamera;      // Vue sur le menu de d�part
    public CinemachineVirtualCamera gridCamera;      // Vue au-dessus de la grille
    public CinemachineVirtualCamera resultsCamera;   // Vue des r�sultats

    private void Start()
    {
        ShowMenuView(); // Commencer par la vue du menu
    }

    public void ShowMenuView()
    {
        menuCamera.Priority = 10;
        gridCamera.Priority = 0;
        resultsCamera.Priority = 0;
    }

    public void ShowGridView()
    {
        menuCamera.Priority = 0;
        gridCamera.Priority = 10;
        resultsCamera.Priority = 0;
    }

    public void ShowResultsView()
    {
        menuCamera.Priority = 0;
        gridCamera.Priority = 0;
        resultsCamera.Priority = 10;
    }

    // Appel�e lorsqu'on appuie sur le bouton Start dans le menu
    public void OnStartButtonPressed()
    {
        ShowGridView();
    }

    // Appel�e lorsqu'on appuie sur Rejouer dans les r�sultats
    public void OnReplayButtonPressed()
    {
        ShowGridView();
    }

    // on modifie la vue des r�sultats dans GameController une fois le jeu fini
}
