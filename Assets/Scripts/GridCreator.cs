using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class GridCreator : MonoBehaviour
{
    public GameObject waterCellPrefab; // Prefab du cube d'eau
    public TextMeshProUGUI textPrefab; // Prefab pour les lettres et numéros (TextMeshProUGUI)
    public Canvas gridCanvas; // Canvas en mode World Space pour afficher les étiquettes
    public int rows = 10; // Nombre de lignes (lettres)
    public int columns = 10; // Nombre de colonnes (numéros)
    public float spacing = 1.1f; // Espace entre les cubes
    public float labelYOffset = -0.2f; // Décalage en Y pour élever les étiquettes

    private Vector3 startPosition; // Position de départ de la grille
    private Dictionary<string, GridCell> gridCellsPositions;

    [SerializeField]
    private Material basicMaterial;

    void Start()
    {
        // Vérifier les références
        if (waterCellPrefab == null || textPrefab == null || gridCanvas == null)
        {
            Debug.LogError("Assurez-vous que tous les prefabs et le canvas sont assignés dans l'inspecteur.");
            return;
        }


        // Utiliser la position du prefab cubeEauPrefab comme position de départ
        startPosition = waterCellPrefab.transform.position;

        // Ajuster le Canvas en fonction de la taille de la grille
        AdjustCanvas(startPosition);

        // Générer la grille de cubes et placer les labels
        GenerateGrid(startPosition);
    }

    private void AdjustCanvas(Vector3 startPosition)
    {
        // Calculer la largeur et la hauteur de la grille
        float gridWidth = (columns) * spacing;
        float gridHeight = (rows) * spacing;


        // Positionner le Canvas au centre de la grille
        gridCanvas.transform.position = startPosition + new Vector3(gridWidth / 2 - spacing / 2, 0, gridHeight / 2 - spacing / 2);


        // Ajuster la taille du Canvas pour qu'il couvre toute la grille
        gridCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(gridWidth, gridHeight);
    }

    private void GenerateGrid(Vector3 startPosition)
    {

        waterCellPrefab.SetActive(true);
        textPrefab.gameObject.SetActive(true);

        if (gridCellsPositions == null)
        {
            gridCellsPositions = new Dictionary<string, GridCell>();
        }

        for (int z = 0; z < rows; z++)
        {
            char rowLetter = (char)('A' + z); // Détermine la lettre de la ligne, ex : 'A', 'B', etc.

            for (int x = 0; x < columns; x++)
            {
                Vector3 position = startPosition + new Vector3(x * spacing, 0, z * spacing);


                // Générer le code de la position (ex. "A1", "B2")
                string positionString = rowLetter + (x + 1).ToString();

                // Si un prefab de cellule est défini, instancie-le pour visualiser la grille dans Unity
                if (waterCellPrefab != null)
                {
                    Instantiate(waterCellPrefab, position, Quaternion.identity, transform);
                }

                // Ajouter des lettres à gauche (pour la première colonne seulement)
                if (x == 0)
                {
                    Vector3 positionGauche = position + new Vector3(-spacing, labelYOffset, 0);
                    TextMeshProUGUI letterText = Instantiate(textPrefab, positionGauche, Quaternion.identity, gridCanvas.transform);
                    letterText.text = rowLetter.ToString();
                    letterText.transform.eulerAngles = new Vector3(90, 0, 0);
                }

                // Ajouter des numéros en haut (pour la première ligne seulement)
                if (z == 0)
                {
                    Vector3 positionHaut = position + new Vector3(0, labelYOffset, -spacing);
                    TextMeshProUGUI numberText = Instantiate(textPrefab, positionHaut, Quaternion.identity, gridCanvas.transform);
                    numberText.text = (x + 1).ToString();
                    numberText.transform.eulerAngles = new Vector3(90, 0, 0);
                }

                // Ajouter la cellule avec son code à gridCellsPositions
                GridCell cell = new GridCell(position, positionString); 
                gridCellsPositions.Add(positionString, cell);
            }
        }
        // suppression du prefab de la grille
        waterCellPrefab.SetActive(false);
        textPrefab.gameObject.SetActive(false);
    }


    public Dictionary<string, GridCell> GetGridCellsPositions()
    {
        return gridCellsPositions;
    }

    public void ResetGrid()
    {
        // Réinitialiser les matériaux de toutes les cellules
        foreach (var cell in gridCellsPositions.Values)
        {
            Vector3 position = cell.GetPositionVector();

            // Trouver les objets proches de la position de la cellule
            Collider[] hitColliders = Physics.OverlapSphere(position, 0.1f);
            foreach (var collider in hitColliders)
            {
                // Vérifie si le nom de l'objet correspond au prefab de la cellule
                if (collider.gameObject.name == "Cube-eau(Clone)")
                {
                    // Recherche spécifiquement le composant "Plane" dans les enfants du prefab
                    Transform planeTransform = collider.transform.Find("Plane");
                    if (planeTransform != null)
                    {
                        MeshRenderer renderer = planeTransform.GetComponent<MeshRenderer>();
                        if (renderer != null)
                        {
                            renderer.material = basicMaterial; // Réinitialise au matériau de base
                        }
                    }
                }
            }
        }

        // Effacer les données des cellules
        gridCellsPositions.Clear();

        // Recréer la grille
        Start();
    }


    public Vector3 getStartPosition()
    {
        return startPosition;
    }


    public void SetCellMaterial(string positionString, Material material){
    // Vérifie si la cellule existe dans la grille
    if (gridCellsPositions.TryGetValue(positionString, out GridCell cell))
    {
        Vector3 position = cell.GetPositionVector();

        // Trouver les objets proches de la position de la cellule
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.1f);
        foreach (var collider in hitColliders)
        {
                Debug.Log("collider : " + collider.gameObject.name);
            // Vérifie si le nom de l'objet correspond au prefab de la cellule
            if (collider.gameObject.name == "Cube-eau(Clone)")
            {
                // Recherche spécifiquement le composant "Plane" dans les enfants du prefab
                Transform planeTransform = collider.transform.Find("Plane");
                if (planeTransform != null)
                {
                    MeshRenderer renderer = planeTransform.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.material = material; // Applique le matériau uniquement au "Plane"
                        Debug.Log($"Matériau appliqué à la cellule {positionString}");
                    }
                    else
                    {
                        Debug.LogWarning($"Aucun MeshRenderer trouvé sur 'Plane' pour {positionString}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Le composant 'Plane' est introuvable dans la cellule {positionString}");
                }
                break; // Une fois le prefab trouvé, on termine la boucle
            }
        }
    }
    else
    {
        Debug.LogWarning("Aucune cellule trouvée pour la position "+ positionString);
    }
}


}



