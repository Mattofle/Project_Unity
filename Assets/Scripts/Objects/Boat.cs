using UnityEngine;
using System.Collections;

public class Boat : MonoBehaviour
{
    private GameObject fireParticles;
    public BoatData boatData; // Donn�es du bateau, d�finies par le ScriptableObject
    private int damage;        // Nombre de coups re�us
    public string currentPosition = "A1"; // Position actuelle du bateau sur la grille"
    public bool isVertical = true; // Orientation du bateau (true = vertical, false = horizontal)

    private void Start()
    {
        fireParticles = transform.Find("flames_particles").gameObject;

        if (fireParticles == null)
        {
            Debug.LogError("fire_particles n'a pas �t� trouv� !");
        }
    }

    // Initialise le bateau avec ses donn�es

    public void InitializeBoat(BoatData data)
    {
        boatData = data;
        damage = 0;
    }

    public void TakeDamage()
    {
        damage++;
    }

    public bool IsSunk()
    {
        if (damage >= boatData.boatSize)
        {
            ActivateFireParticles();
            return true;
        }
        return false;
    }

    public void ActivateFireParticles()
    {
       
        fireParticles.SetActive(true);
        
    }

    public void ShowBoat()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
        Debug.Log($"Boat {boatData.boatName} is now visible.");
    }





    public string GetName() => boatData.boatName;
    public Sprite GetIcon() => boatData.boatSprite;

    public int GetSize() => boatData.boatSize;


}
