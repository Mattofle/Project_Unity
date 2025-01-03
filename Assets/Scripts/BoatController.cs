using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public GridController gridController; // Référence au GridController pour vérifier les positions
    public Boat selectedBoat;            // Le bateau actuellement sélectionné pour le déplacement
    private string currentBoatPosition;   // Position actuelle sous forme de code (ex: "A1")
    //private bool isVertical = true;     // Orientation du bateau (true = vertical, false = horizontal)
    public BoatSelectionMenu boatSelectionMenu;
    public bool boatSelection = false;

    void Start()
    {
        if (gridController == null)
        {
            Debug.LogError("GridController n'est pas assigné !");
        }
    }

    void Update()
    {
        if (boatSelection)
        {
            if (Input.GetMouseButtonDown(0)) // Vérifie si le bouton gauche de la souris est cliqué
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject clickedObject = hit.collider.gameObject;
                    // Vérifie si l'objet cliqué est un bateau
                    Boat boat = clickedObject.GetComponent<Boat>();
                    if (boat != null) // Vérifie si le script Boat est attaché
                    {
                        SetSelectedBoat(boat); // Passe l'objet Boat au contrôleur
                    }
                }
            }
        }
    }

    // Méthode pour définir le bateau sélectionné
    public void SetSelectedBoat(Boat boat)
    {
        print("SELECTING A BOAT ");
        selectedBoat = boat;
        currentBoatPosition = boat.currentPosition;
    }

    // Déplace le bateau vers le haut
    public void MoveUp()
    {
        TryMoveBoat(1, 0);
    }

    // Déplace le bateau vers le bas
    public void MoveDown()
    {
        TryMoveBoat(-1, 0);
    }

    // Déplace le bateau vers la droite
    public void MoveRight()
    {
        TryMoveBoat(0, 1);
    }

    // Déplace le bateau vers la gauche
    public void MoveLeft()
    {
        TryMoveBoat(0, -1);

    }

    // Méthode pour tourner le bateau vers la droite
    public void RotateRight()
    {
        if (selectedBoat == null) return;

        // Simule la nouvelle orientation
        bool newIsVertical = !selectedBoat.isVertical;

        // Vérifie si le bateau peut être placé avec la nouvelle orientation
        if (PlaceSelectedBoat(currentBoatPosition, newIsVertical))
        {
            // Applique la rotation si le placement est valide
            selectedBoat.isVertical = newIsVertical;
            //selectedBoat.ToggleOrientation();
        }
        else
        {
            Debug.LogWarning("Rotation vers la droite impossible : espace insuffisant ou position occupée.");
        }
    }

    // Méthode pour tourner le bateau vers la gauche
    public void RotateLeft()
    { 
        if (selectedBoat == null) return;

        // Simule la nouvelle orientation
        bool newIsVertical = !selectedBoat.isVertical;

        // Vérifie si le bateau peut être placé avec la nouvelle orientation
        if (PlaceSelectedBoat(currentBoatPosition, newIsVertical))
        {
            // Applique la rotation si le placement est valide
            selectedBoat.isVertical = newIsVertical;
            //selectedBoat.ToggleOrientation();
        }
        else
        {
            Debug.LogWarning("Rotation vers la droite impossible : espace insuffisant ou position occupée.");
        }
    }



    // Essaye de déplacer le bateau vers une nouvelle position
    private void TryMoveBoat(int deltaX, int deltaZ)
    {
        if (selectedBoat == null) return;
        if (boatSelection == false) return;

        string newPositionCode = CalculateNewPositionCode(selectedBoat.currentPosition, deltaX, deltaZ);

      currentBoatPosition = newPositionCode;
      PlaceSelectedBoat(newPositionCode, selectedBoat.isVertical);
    }

    // Calcule le nouveau code de position en fonction du déplacement
    private string CalculateNewPositionCode(string currentCode, int deltaX, int deltaZ)
    {
        print("STRING CODE: " + currentCode);
        char column = currentCode[0];
        int row = int.Parse(currentCode.Substring(1));

        char newColumn = (char)(column + deltaX);
        int newRow = row + deltaZ;

        return newColumn.ToString() + newRow.ToString();
    }

    public bool PlaceSelectedBoat(string startPositionCode, bool isVertical)
    {
        if (selectedBoat == null)
        {
            Debug.LogWarning("Aucun bateau n'a été sélectionné !");
            return false;
        }

        Boat boatComponent = selectedBoat.GetComponent<Boat>();

        // Place le bateau via le GridController
        bool placementSuccess = gridController.PlaceBoat(boatComponent, startPositionCode, isVertical);

        return placementSuccess;
    }

    public void toggleBoatSelection()
    {
        boatSelection = !boatSelection;
    }
}