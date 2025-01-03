using UnityEngine;

public class GridCell
{
    private bool hasBoat;       // Indique si la cellule contient un bateau
    private Vector3 positionVector;    // Position de la cellule dans le monde
    private string positionString;     // Position de la cellule sous forme de chaîne de caractères
    private bool isHit;         // Indique si la cellule a été touchée par un tir

    // Constructeur
    public GridCell(Vector3 pos, string posString)
    {
        positionVector = pos;
        positionString = posString;
        hasBoat = false;
    }

    // Vérifie si la cellule contient un bateau
    public bool  HasBoat()
    {
        if (hasBoat)
        {
            Debug.LogWarning("La cellule " + positionString + " contient déjà un bateau.");
            return false;
        }

        hasBoat = true;
        return true;
    }


    public bool Hit()
    {
        if (isHit)
        {
            Debug.LogWarning("La cellule " + positionString + " a déjà été touchée.");
            return false;
        }

        isHit = true;
        return true;
    }

    // Réinitialise l'état de la cellule pour une nouvelle partie
    public void Reset()
    {
        hasBoat = false;
        isHit = false;
    }

    public string GetPositionString()
    {
        return positionString;
    }

    public Vector3 GetPositionVector()
    {
        return positionVector;
    }

    public bool HasNoBoat()
    {
     
        hasBoat = false;
        return true;
    }

}
