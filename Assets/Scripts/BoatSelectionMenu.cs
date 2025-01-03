using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BoatSelectionMenu : MonoBehaviour
{
    public List<BoatData> boatsData;        // Liste des donn�es de bateaux disponibles (ScriptableObjects)
    public GameObject buttonPrefab;          // Prefab de bouton pour chaque bateau
    public Transform buttonContainer;        // Conteneur dans lequel les boutons seront instanci�s
    public GridController gridController;    // R�f�rence au script GridController pour le placement des bateaux
    private BoatData selectedBoatData;       // Donn�es du bateau actuellement s�lectionn� pour le placement
    private GameObject previewBoat;          // Pr�visualisation du bateau
    public BoatController boatController;    // R�f�rence au script BoatController pour le d�placement des bateaux
    private Dictionary<BoatData, Boat> placedBoats = new Dictionary<BoatData, Boat>();
    public bool boatSelection = false;

    void Start()
    {
        // Initialiser les boutons pour chaque bateau
        GenerateBoatSelectionButtons();
        print("BoatSelectionMenu initialized");
    }

    // G�n�re un bouton de s�lection pour chaque bateau
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


            // Associe l'action de s�lection du bateau � ce bouton
            buttonComponent.onClick.AddListener(() => SelectBoat(boatData));
        }
    }

    // M�thode appel�e lors de la s�lection d'un bateau
    public void SelectBoat(BoatData boatData)
    {

        selectedBoatData = boatData;
        Debug.Log("Bateau s�lectionn� : " + selectedBoatData.boatName);

        // Place le bateau � la position A1 sur la grille
        PlaceSelectedBoat(boatData.currentPosition, true);  // Place le bateau en "A1" en mode horizontal par d�faut
    }

    // Place le bateau s�lectionn� � une position sp�cifique sur la grille
    public void PlaceSelectedBoat(string startPositionCode, bool isHorizontal)
    {
        if(boatSelection == false)
        {
            return;
        }
        if (selectedBoatData == null)
        {
            Debug.LogWarning("Aucun bateau n'a �t� s�lectionn� !");
            return;
        }

       
       

        // V�rifie si un bateau de ce type a d�j� �t� plac�
        if (placedBoats.ContainsKey(selectedBoatData))
        {
            // Si le bateau existe d�j�, le d�placer
            Debug.Log("Un bateau de ce type existe d�j�. Mise � jour de sa position.");
            Boat existingBoat = placedBoats[selectedBoatData];
            existingBoat.boatData.isHorizontal = true;
            existingBoat.isVertical = true;
            bool moveSuccess = gridController.PlaceBoat(existingBoat, startPositionCode, true);

            if (moveSuccess)
            {
                Debug.Log("Bateau d�plac� en : " + startPositionCode);
                //trouver le boat
                existingBoat.currentPosition = startPositionCode;
                boatController.SetSelectedBoat(existingBoat);
            }
            else
            {
                Debug.LogWarning("Impossible de d�placer le bateau en : " + startPositionCode);
            }

            return; // Terminer la m�thode ici, car aucune nouvelle instance n'est n�cessaire
        }

        // Cr�e une instance du bateau � placer sur la grille
        GameObject boatInstance = Instantiate(selectedBoatData.boatPrefab, gridController.transform);

        // Assurez-vous que le bateau a un composant Boat
        Boat boatComponent = boatInstance.GetComponent<Boat>();
        boatComponent.boatData = selectedBoatData;

        // Place le bateau via le GridController
        bool placementSuccess = gridController.PlaceBoat(boatComponent, startPositionCode, isHorizontal);

        if (placementSuccess)
        {
            Debug.Log("Bateau plac� en : " + startPositionCode);

            // Ajoute le bateau au dictionnaire des bateaux plac�s
            placedBoats[selectedBoatData] = boatComponent;

            // Associe le bateau au contr�leur pour le d�placement
            boatController.SetSelectedBoat(boatComponent);

            // R�initialise le bateau s�lectionn�
            selectedBoatData = null;
        }
        else
        {
            Debug.LogWarning("Impossible de placer le bateau en : " + startPositionCode);
            Destroy(boatInstance); // D�truire l'instance si le placement �choue
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
