using UnityEngine;

[CreateAssetMenu(fileName = "NewBoatData", menuName = "Boat Data")]
public class BoatData : ScriptableObject
{
    public GameObject boatPrefab;   // Le prefab du bateau � instancier
    public Sprite boatSprite;       // L'ic�ne du bateau pour l'UI
    public string boatName;         // Nom du bateau
    public int boatSize;            // Taille du bateau (nombre de cellules)
    public int maxPlacement;
    public string currentPosition;
    public bool isHorizontal;
}
