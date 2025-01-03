using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public GridCreator grid; // R�f�rence � GridCreator
    public int boatsCount = 5; // Nombre de bateaux � placer
    public List<Boat> boats = new List<Boat>(); // Liste de tous les bateaux
    public Dictionary<string, Boat> cellToBoat = new Dictionary<string, Boat>(); // Mappe les positions (code de cellule) aux bateaux
    private string exceptionBoat = "Bateau de p�che";

    public AudioSource explosionSound;
    public AudioSource waterDroupSdound;

    // Place un bateau entre deux codes de position
    public bool PlaceBoat(Boat boat, string codeDepart, bool isVertical)
    {
        Debug.Log("IN GRID CONTROLLER - PLACEBOAT");

        // R�cup�re toutes les cellules entre le d�part et la fin
        List<string> boatPositions = GetCellsBetweenCodes(boat, codeDepart, isVertical);

        if (boatPositions == null || !CheckAvailabilityOfCells(boatPositions, boat))
        {
            Debug.Log("Impossible de placer le bateau : espace insuffisant ou occup�.");
            return false;
        }

        if (!boats.Contains(boat))
        {
            boats.Add(boat);
            Debug.Log("Bateau ajout� � la liste");
        }


        // Supprime les anciennes positions de `cellToBoat`
        List<string> currentBoatPosition = GetBoatCells(boat);
        if (currentBoatPosition == null)
        {
            Debug.Log("Impossible de placer le bateau :probl�me dans GetCellsBetweenCodes");
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
        boat.currentPosition = firstPosition;   // Met � jour la position actuelle du bateau

        // Met � jour la position et la rotation du bateau dans le monde 3D
        AlignBoatToGrid(boat, firstPosition, isVertical);

        Debug.Log("Bateau plac� avec succ�s.");
        return true;
    }

    // Aligne le bateau sur la grille et applique la rotation
    private void AlignBoatToGrid(Boat boat, string firstPosition, bool isVertical)
    {
        Debug.Log("Alignement du bateau sur la grille.");
        Vector3 worldPosition = grid.GetGridCellsPositions()[firstPosition].GetPositionVector();
        Transform frontAnchor = boat.transform.Find("headBoat");

        // D�termine la rotation en fonction de l'orientation
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

        Debug.Log($"Bateau align� en {firstPosition} avec orientation {(isVertical ? "vertical" : "horizontal")}");
    }

    private List<string> GetCellsBetweenCodes(Boat boat, string codeDepart, bool isHorizontal)
    {
        if (boat == null || codeDepart.Length < 2)
        {
            Debug.LogWarning("Bateau ou code de d�part invalide.");
            return null;
        }

        int sizeBoat = boat.boatData.boatSize;
        int gridColumns = grid.columns; // Nombre de colonnes de la grille
        int gridRows = grid.rows;       // Nombre de lignes de la grille

        // R�cup�re la position de d�part
        int x = codeDepart[0] - 'A'; // Convertit la lettre de la colonne en index (0 pour 'A', 1 pour 'B', etc.)
        int y = int.Parse(codeDepart.Substring(1)) - 1; // Ligne de d�part

        List<string> positions = new List<string>();

        for (int i = 0; i < sizeBoat; i++)
        {
            int currentX = x + (isHorizontal ? i : 0);
            int currentY = y + (isHorizontal ? 0 : i) + 1;

            // V�rifie si les coordonn�es sont dans les limites
            if (currentX < 0 || currentX >= gridColumns || currentY <= 0 || currentY > gridRows)
            {
                Debug.LogWarning($"Cellule hors limites d�tect�e : {currentX}, {currentY}");
                return null;
            }

            string cellCode = $"{(char)(currentX + 'A')}{currentY}";

            // V�rifie si la cellule existe dans le dictionnaire
            if (!grid.GetGridCellsPositions().ContainsKey(cellCode))
            {
                Debug.LogWarning($"La cellule {cellCode} n'existe pas dans le dictionnaire.");
                return null;
            }

            positions.Add(cellCode);
        }

        return positions;
    }



    // Tir sur une position donn�e et inflige des d�g�ts si un bateau y est pr�sent
    public bool Shoot(string position)
    {
        if (cellToBoat.TryGetValue(position, out Boat boat))
        {
            boat.TakeDamage();
            grid.GetGridCellsPositions()[position].Hit();
            CameraShakeOnHit.Instance.TriggerImpact(2f, .5f);
            explosionSound.Play();
            Debug.Log("Touch� sur : " + boat.name);
            Debug.Log("Position : " + grid.GetGridCellsPositions()[position].GetPositionVector());

            if (boat.IsSunk())
            {
                Debug.Log("Le bateau " + boat.name + " a coul� !");
                boat.ShowBoat();
            }

            return true;
        }
        else
        {
            Debug.Log("� l'eau !");
            waterDroupSdound.Play();
            return false;
        }
    }

    // V�rifie si toutes les cellules d�une liste sont disponibles
    public bool CheckAvailabilityOfCells(List<string> cells, Boat currentBoat)
    {
        foreach (var cell in cells)
        {
            if (cellToBoat.ContainsKey(cell) && cellToBoat[cell] != currentBoat)
            {
                Debug.Log("Position occup�e par un autre bateau.");
                return false;
            }
        }
        return true;
    }

    // V�rifie si tous les bateaux sont plac�s
    public bool AreAllBoatsPlaced()
    {
        return boats.Count == boatsCount;
    }

    // V�rifie si tous les bateaux sont coul�s
    public bool AreAllBoatsSunk()
    {
        foreach (Boat boat in boats)
        {
            if (!boat.IsSunk()) return false;
        }
        return true;
    }

    // R�cup�re les donn�es des bateaux
    public List<BoatData> GetDataBoats()
    {
        List<BoatData> boatData = new List<BoatData>();
        foreach (Boat boat in boats)
        {
            boatData.Add(boat.boatData);
        }
        return boatData;
    }

    // R�initialise la grille pour une nouvelle partie
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
            if (entry.Value == targetBoat) // V�rifie si la case appartient au bateau recherch�
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