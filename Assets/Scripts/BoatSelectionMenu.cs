using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BoatSelectionMenu : MonoBehaviour
{
    public List<BoatData> boatsData;        // Liste des données de bateaux disponibles (ScriptableObjects)
    public GameObject buttonPrefab;          // Prefab de bouton pour chaque bateau
    public Transform buttonContainer;        // Conteneur dans lequel les boutons seront instanciés
    public GridController gridController;    // Référence au script GridController pour le placement des bateaux
    private BoatData selectedBoatData;       // Données du bateau actuellement sélectionné pour le placement
    private GameObject previewBoat;          // Prévisualisation du bateau
    public BoatController boatController;    // Référence au script BoatController pour le déplacement des bateaux
    private Dictionary<BoatData, Boat> placedBoats = new Dictionary<BoatData, Boat>();
    public bool boatSelection = false;

    void Start()
    {
        // Initialiser les boutons pour chaque bateau
        GenerateBoatSelectionButtons();
        print("BoatSelectionMenu initialized");
    }

    // Génère un bouton de sélection pour chaque bateau
    private void GenerateBoatSelectionButtons()
    {
        foreach (var boatData in boatsData)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            Button buttonComponent = newButton.GetComponent<Button>();

            // Affiche l'image et le nom du bateau sur le bouton
            print("Boat name: " + boatData.boatName);
            Image buttonImage = newButton.transform.Find("Icon").GetComponent<Image>();

            buttonImage.sprite = boatData.boatSprite;
            TextMeshProUGUI boatNameText = newButton.transform.Find("boatName").GetComponent<TextMeshProUGUI>();
            boatNameText.text = boatData.boatName;


            // Associe l'action de sélection du bateau à ce bouton
            buttonComponent.onClick.AddListener(() => SelectBoat(boatData));
        }
    }

    // Méthode appelée lors de la sélection d'un bateau
    public void SelectBoat(BoatData boatData)
    {

        selectedBoatData = boatData;
        Debug.Log("Bateau sélectionné : " + selectedBoatData.boatName);

        // Place le bateau à la position A1 sur la grille
        PlaceSelectedBoat(boatData.currentPosition, true);  // Place le bateau en "A1" en mode horizontal par défaut
    }

    // Place le bateau sélectionné à une position spécifique sur la grille
    public void PlaceSelectedBoat(string startPositionCode, bool isHorizontal)
    {
        if(boatSelection == false)
        {
            return;
        }
        if (selectedBoatData == null)
        {
            Debug.LogWarning("Aucun bateau n'a été sélectionné !");
            return;
        }

       
       

        // Vérifie si un bateau de ce type a déjà été placé
        if (placedBoats.ContainsKey(selectedBoatData))
        {
            // Si le bateau existe déjà, le déplacer
            Debug.Log("Un bateau de ce type existe déjà. Mise à jour de sa position.");
            Boat existingBoat = placedBoats[selectedBoatData];
            existingBoat.boatData.isHorizontal = true;
            existingBoat.isVertical = true;
            bool moveSuccess = gridController.PlaceBoat(existingBoat, startPositionCode, true);

            if (moveSuccess)
            {
                Debug.Log("Bateau déplacé en : " + startPositionCode);
                //trouver le boat
                existingBoat.currentPosition = startPositionCode;
                boatController.SetSelectedBoat(existingBoat);
            }
            else
            {
                Debug.LogWarning("Impossible de déplacer le bateau en : " + startPositionCode);
            }

            return; // Terminer la méthode ici, car aucune nouvelle instance n'est nécessaire
        }

        // Crée une instance du bateau à placer sur la grille
        GameObject boatInstance = Instantiate(selectedBoatData.boatPrefab, gridController.transform);

        // Assurez-vous que le bateau a un composant Boat
        Boat boatComponent = boatInstance.GetComponent<Boat>();
        boatComponent.boatData = selectedBoatData;

        // Place le bateau via le GridController
        bool placementSuccess = gridController.PlaceBoat(boatComponent, startPositionCode, isHorizontal);

        if (placementSuccess)
        {
            Debug.Log("Bateau placé en : " + startPositionCode);

            // Ajoute le bateau au dictionnaire des bateaux placés
            placedBoats[selectedBoatData] = boatComponent;

            // Associe le bateau au contrôleur pour le déplacement
            boatController.SetSelectedBoat(boatComponent);

            // Réinitialise le bateau sélectionné
            selectedBoatData = null;
        }
        else
        {
            Debug.LogWarning("Impossible de placer le bateau en : " + startPositionCode);
            Destroy(boatInstance); // Détruire l'instance si le placement échoue
        }
    }

    public void Reset()
    {
       placedBoats.Clear();
    }

    public void toggleBoatSelection()
    {
        boatSelection = !boatSelection;
    }

}
