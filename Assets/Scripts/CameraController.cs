using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera menuCamera;      // Vue sur le menu de départ
    public CinemachineVirtualCamera gridCamera;      // Vue au-dessus de la grille
    public CinemachineVirtualCamera resultsCamera;   // Vue des résultats

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

    // Appelée lorsqu'on appuie sur le bouton Start dans le menu
    public void OnStartButtonPressed()
    {
        ShowGridView();
    }

    // Appelée lorsqu'on appuie sur Rejouer dans les résultats
    public void OnReplayButtonPressed()
    {
        ShowGridView();
    }

    // on modifie la vue des résultats dans GameController une fois le jeu fini
}
