using UnityEngine;
using System.Collections;
using TMPro.Examples;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    public GridController playerGrid;    // Grille du joueur
    public GridController aiGrid;        // Grille de l'IA
    public AIController aiController;   // Contr�leur de l'IA
    public CameraController CameraController; // R�f�rence au script CameraController
    // add visuels de victoire et de d�faite
    public GameObject victoryScreen;
    public GameObject defeatScreen;

    private List<BoatData> boatDataList;

    private bool placementConfirmed = false;

    // Temps d'attente entre les tours
    public float turnDelay = 1f;

    // Liste des positions d�j� tir�es par le joueur
    private HashSet<string> playerShots = new HashSet<string>();

    public GridCreator gridCreatorAI;
    public GridCreator gridCreatorPlayer;
    public BoatController boatController;
    public BoatSelectionMenu boatSelectionMenu;
    public GameObject startMessage; // R�f�rence au texte "C'est parti!"


    [SerializeField]
    private Material hitMaterial;  // Mat�riau pour une case touch�e

    [SerializeField]
    private Material missMaterial; // Mat�riau pour une case manqu�e

   

    // M�thode appel�e au d�marrage
    void Start()
    {
        startMessage.SetActive(false);

        Debug.Log("Start Called");
        StartCoroutine(GameLoop());
        aiController.setMaterail(hitMaterial, missMaterial);
    }

    // La boucle du jeu qui alterne entre le joueur et l'IA
    IEnumerator GameLoop()
    {
        // Le joueur place ses bateaux
        boatController.toggleBoatSelection();
        boatSelectionMenu.toggleBoatSelection();
        yield return StartCoroutine(PlayerPlaceShips());
        foreach(string cell in playerGrid.GetBoatCells()){
            Debug.Log("cell: " + cell);

        }
        Debug.Log("END PlayerPlaceShips");
        boatController.toggleBoatSelection();
        boatSelectionMenu.toggleBoatSelection();

        // Une fois les bateaux du joueur plac�s, l'IA place ses propres bateaux
        aiController.InitializeAIShips(aiGrid, boatDataList);
        foreach (string cell in aiGrid.GetBoatCells())
        {
            Debug.Log("cell: " + cell);

        }
        Debug.Log("END InitializeAIShips");
        startMessage.SetActive(true);

        // Attendre 2 secondes avant de cacher le message
        yield return new WaitForSeconds(1.5f);

        startMessage.SetActive(false);

        // Tour par tour, joueur et IA tirent jusqu'� ce qu'il n'y ait plus de bateaux
        while (!playerGrid.AreAllBoatsSunk() && !aiGrid.AreAllBoatsSunk())
        {
            // Le joueur tire
            yield return StartCoroutine(PlayerTurn());

            // L'IA tire
            
        }

        // Fin du jeu
        EndGame();
    }

    // M�thode pour que le joueur place ses bateaux
    IEnumerator PlayerPlaceShips()
    {
        // Afficher un message ou un �cran de placement des bateaux du joueur ici, si n�cessaire
        Debug.Log("Placez vos bateaux !");

        // Simuler une attente pendant que le joueur place ses bateaux
        while (!placementConfirmed)
        {
            // Attendre un peu avant de v�rifier � nouveau
            yield return null;
        }

        Debug.Log("Tous les bateaux du joueur ont �t� plac�s.");
        boatDataList = playerGrid.GetDataBoats();
        Debug.Log("boatDataList: " + boatDataList.Count);

    }

    public void ConfirmPlacement()
    {
        // V�rifier si tous les bateaux sont bien plac�s
        if (playerGrid.AreAllBoatsPlaced())
        {
            Debug.Log("Placement confirm�, le jeu va commencer !");
          
            placementConfirmed = true;
        }
        else
        {
            Debug.LogWarning("Tous les bateaux ne sont pas plac�s. Compl�tez le placement avant de confirmer.");
        }
    }

    // M�thode pour que le joueur tire
    IEnumerator PlayerTurn()
    {
        Debug.Log("Tour du joueur : Cliquez sur une cellule pour tirer.");
        bool hasShot = false;
        string targetPosition = null;

        foreach (var boat in aiGrid.boats) 
        {

            var collider = boat.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = false;

            }
            else
            {
                Debug.LogWarning($"Pas de collider trouv� pour {boat.name}");
            }
        }

        while (!hasShot)
        {
            if (Input.GetMouseButtonDown(0)) // D�tecter un clic gauche
            {
                print("clicked");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 clickPosition = hit.point;

                    // Trouver la cellule la plus proche
                    //GridCreator gridCreator = aiGrid.GetComponent<GridCreator>();

                    if (gridCreatorAI != null)
                    {
                        foreach (var cell in gridCreatorAI.GetGridCellsPositions())
                        {
                            float distance = Vector3.Distance(clickPosition, cell.Value.GetPositionVector());
                            if (distance < 5f)
                            {
                                print("found cell: " + cell.Value.GetPositionString());
                                targetPosition = cell.Value.GetPositionString();

                                // V�rifier si cette cellule a d�j� �t� cibl�e
                                if (playerShots.Contains(targetPosition))
                                {
                                    Debug.LogWarning("Vous avez d�j� tir� sur cette cellule : " + targetPosition);
                                }
                                else
                                {
                                    playerShots.Add(targetPosition); // Ajouter la cellule � la liste des tirs
                                    if (aiGrid.Shoot(targetPosition))
                                    {
                                        Debug.Log("Le joueur a touch� un bateau !");
                                        gridCreatorAI.SetCellMaterial(targetPosition, hitMaterial);
                                    }
                                    else
                                    {
                                        Debug.Log("Le joueur a manqu�.");
                                        gridCreatorAI.SetCellMaterial(targetPosition, missMaterial);
                                    }
                                    Debug.Log("Le joueur a tir� sur : " + targetPosition);
                                    if (aiGrid.cellToBoat.ContainsKey(targetPosition)) {
                                        print("PLAYER A TOUCHE");
                                    } 
                                    hasShot = true; // Le tir a �t� valid�
                                    break; // Arr�ter la recherche d'autres cellules
                                }

                            }
                            if(targetPosition==null)
                            {
                                Debug.LogWarning("Vous avez cliqu� en dehors de la grille");
                            }
                        }
                    }
                }
            }
            
            yield return null; // Attendre le prochain frame
        }
        yield return StartCoroutine(AITurn());
    }

        // M�thode pour que l'IA tire
        IEnumerator AITurn()
    {
        yield return new WaitForSeconds(turnDelay);
        // L'IA effectue un tir sur la grille du joueur
        aiController.TakeTurn(playerGrid, () => Debug.Log("L'IA a termin� son tour."));

        // Attendre avant de passer au tour suivant
        yield return new WaitForSeconds(turnDelay);
    }

    // M�thode pour mettre fin � la partie
    void EndGame()
    {
        if (playerGrid.AreAllBoatsSunk())
        {
            Debug.Log("L'IA a gagn� !");
            defeatScreen.SetActive(true);
            victoryScreen.SetActive(false);
        }
        else if (aiGrid.AreAllBoatsSunk())
        {
            Debug.Log("Le joueur a gagn� !");
            victoryScreen.SetActive(true);
            defeatScreen.SetActive(false);
        }

        // Activer la vue des r�sultats � la fin du jeu
        CameraController.ShowResultsView();
    }

    // rejouer
    public void Replay()
    {
        // R�initialiser les grilles et les contr�leurs
        Reset();
        // Recommencer la boucle de jeu
        StartCoroutine(GameLoop());
    }

    private void Reset()
    {
        foreach (var boat in aiGrid.boats)
        {
            var collider = boat.GetComponent<BoxCollider>();
                collider.enabled = true;
        }

        playerGrid.ResetGrid();
        aiGrid.ResetGrid();
        aiController.ResetAI();

        victoryScreen.SetActive(false);
        defeatScreen.SetActive(false);
        boatSelectionMenu.Reset();

        playerShots.Clear(); // R�initialiser la liste des tirs
        placementConfirmed = false; // R�initialiser le placement confirm�
    }

    // retour au menu
    public void BackToMenu()
    {
        Reset();
        // Recharger la sc�ne du menu principal
        CameraController.ShowMenuView();
        StartCoroutine(GameLoop());

    }



}