using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Start Game clicked!");
        // Ajoute ici le code pour démarrer le jeu
    }

    public void OpenSettings()
    {
        Debug.Log("Settings clicked!");
        // Ajoute ici le code pour ouvrir les paramètres
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game clicked!");
        Application.Quit();
    }

    public void MuteGame()
    {
        Debug.Log("Mute clicked!");
    }
}