using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public GridCreator grid; // Référence à GridCreator
    public int boatsCount = 5; // Nombre de bateaux à placer
    public List<Boat> boats = new List<Boat>(); // Liste de tous les bateaux
    public Dictionary<string, Boat> cellToBoat = new Dictionary<string, Boat>(); // Mappe les positions (code de cellule) aux bateaux
    private string exceptionBoat = "Bateau de pêche";

    public AudioSource explosionSound;
    public AudioSource waterDroupSdound;

    // Place un bateau entre deux codes de position
    public bool PlaceBoat(Boat boat, string codeDepart, bool isVertical)
    {
        Debug.Log("IN GRID CONTROLLER - PLACEBOAT");

        // Récupère toutes les cellules entre le départ et la fin
        List<string> boatPositions = GetCellsBetweenCodes(boat, codeDepart, isVertical);

        if (boatPositions == null || !CheckAvailabilityOfCells(boatPositions, boat))
        {
            Debug.Log("Impossible de placer le bateau : espace insuffisant ou occupé.");
            return false;
        }

        if (!boats.Contains(boat))
        {
            boats.Add(boat);
            Debug.Log("Bateau ajouté à la liste");
        }


        // Supprime les anciennes positions de `cellToBoat`
        List<string> currentBoatPosition = GetBoatCells(boat);
        if (currentBoatPosition == null)
        {
            Debug.Log("Impossible de placer le bateau :problème dans GetCellsBetweenCodes");
            return false;
        }
        foreach (string position in currentBoatPosition)
        {
            cellToBoat.Remove(position);
            grid.GetGridCellsPositions()[position].HasNoBoat();
        }

        // Ajoute le bateau et marque ses nouvelles positions dans `cellToBoat`
        foreach (string position in boatPositions)
        {
            cellToBoat[position] = boat;
            grid.GetGridCellsPositions()[position].HasBoat();
        }

        string firstPosition = boatPositions[0]; 
        boat.currentPosition = firstPosition;   // Met à jour la position actuelle du bateau

        // Met à jour la position et la rotation du bateau dans le monde 3D
        AlignBoatToGrid(boat, firstPosition, isVertical);

        Debug.Log("Bateau placé avec succès.");
        return true;
    }

    // Aligne le bateau sur la grille et applique la rotation
    private void AlignBoatToGrid(Boat boat, string firstPosition, bool isVertical)
    {
        Debug.Log("Alignement du bateau sur la grille.");
        Vector3 worldPosition = grid.GetGridCellsPositions()[firstPosition].GetPositionVector();
        Transform frontAnchor = boat.transform.Find("headBoat");

        // Détermine la rotation en fonction de l'orientation
        Quaternion targetRotation;
        if (boat.boatData.boatName == exceptionBoat)
        {
            targetRotation = isVertical
                ? Quaternion.Euler(0, 180, 0) 
                : Quaternion.Euler(0, -90, 0); 
        }
        else
        {
            targetRotation = isVertical
                ? Quaternion.Euler(0, 0, 0)   
                : Quaternion.Euler(0, 90, 0); 
        }

        // Applique la rotation
        boat.transform.rotation = targetRotation;

        // Ajuste la position
        Vector3 anchorOffset = boat.transform.position - frontAnchor.position;
        boat.transform.position = worldPosition + anchorOffset;

        Debug.Log($"Bateau aligné en {firstPosition} avec orientation {(isVertical ? "vertical" : "horizontal")}");
    }

    private List<string> GetCellsBetweenCodes(Boat boat, string codeDepart, bool isHorizontal)
    {
        if (boat == null || codeDepart.Length < 2)
        {
            Debug.LogWarning("Bateau ou code de départ invalide.");
            return null;
        }

        int sizeBoat = boat.boatData.boatSize;
        int gridColumns = grid.columns; // Nombre de colonnes de la grille
        int gridRows = grid.rows;       // Nombre de lignes de la grille

        // Récupère la position de départ
        int x = codeDepart[0] - 'A'; // Convertit la lettre de la colonne en index (0 pour 'A', 1 pour 'B', etc.)
        int y = int.Parse(codeDepart.Substring(1)) - 1; // Ligne de départ

        List<string> positions = new List<string>();

        for (int i = 0; i < sizeBoat; i++)
        {
            int currentX = x + (isHorizontal ? i : 0);
            int currentY = y + (isHorizontal ? 0 : i) + 1;

            // Vérifie si les coordonnées sont dans les limites
            if (currentX < 0 || currentX >= gridColumns || currentY <= 0 || currentY > gridRows)
            {
                Debug.LogWarning($"Cellule hors limites détectée : {currentX}, {currentY}");
                return null;
            }

            string cellCode = $"{(char)(currentX + 'A')}{currentY}";

            // Vérifie si la cellule existe dans le dictionnaire
            if (!grid.GetGridCellsPositions().ContainsKey(cellCode))
            {
                Debug.LogWarning($"La cellule {cellCode} n'existe pas dans le dictionnaire.");
                return null;
            }

            positions.Add(cellCode);
        }

        return positions;
    }



    // Tir sur une position donnée et inflige des dégâts si un bateau y est présent
    public bool Shoot(string position)
    {
        if (cellToBoat.TryGetValue(position, out Boat boat))
        {
            boat.TakeDamage();
            grid.GetGridCellsPositions()[position].Hit();
            CameraShakeOnHit.Instance.TriggerImpact(2f, .5f);
            explosionSound.Play();
            Debug.Log("Touché sur : " + boat.name);
            Debug.Log("Position : " + grid.GetGridCellsPositions()[position].GetPositionVector());

            if (boat.IsSunk())
            {
                Debug.Log("Le bateau " + boat.name + " a coulé !");
                boat.ShowBoat();
            }

            return true;
        }
        else
        {
            Debug.Log("À l'eau !");
            waterDroupSdound.Play();
            return false;
        }
    }

    // Vérifie si toutes les cellules d’une liste sont disponibles
    public bool CheckAvailabilityOfCells(List<string> cells, Boat currentBoat)
    {
        foreach (var cell in cells)
        {
            if (cellToBoat.ContainsKey(cell) && cellToBoat[cell] != currentBoat)
            {
                Debug.Log("Position occupée par un autre bateau.");
                return false;
            }
        }
        return true;
    }

    // Vérifie si tous les bateaux sont placés
    public bool AreAllBoatsPlaced()
    {
        return boats.Count == boatsCount;
    }

    // Vérifie si tous les bateaux sont coulés
    public bool AreAllBoatsSunk()
    {
        foreach (Boat boat in boats)
        {
            if (!boat.IsSunk()) return false;
        }
        return true;
    }

    // Récupère les données des bateaux
    public List<BoatData> GetDataBoats()
    {
        List<BoatData> boatData = new List<BoatData>();
        foreach (Boat boat in boats)
        {
            boatData.Add(boat.boatData);
        }
        return boatData;
    }

    // Réinitialise la grille pour une nouvelle partie
    public void ResetGrid()
    {
        foreach (Boat boat in boats)
        {
            if (boat != null)
            {
                Destroy(boat.gameObject);
            }
        }

        cellToBoat.Clear();
        boats.Clear();
        grid.ResetGrid();
    }


    public List<string> GetBoatCells(Boat targetBoat)
    {
        List<string> boatCells = new List<string>();

        foreach (var entry in cellToBoat)
        {
            if (entry.Value == targetBoat) // Vérifie si la case appartient au bateau recherché
            {
                boatCells.Add(entry.Key);
            }
        }

        return boatCells;
    }

    public List<string> GetBoatCells()
    {
        return cellToBoat.Keys.ToList();
    }

}