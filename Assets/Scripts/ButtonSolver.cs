using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSolver : MonoBehaviour
{
    // Start is called before the first frame update
    public Game game;
    public Text buttonText;

    // Update is called once per frame
    void Update()
    {
        if(game.getSolverRunning())
        {
            buttonText.text = "Arrêt du solveur";
        }
        else
        {
            buttonText.text = "Lancement du solveur";
        }
    }

    public void TaskOnClick()
    {
        if(game.getSolverRunning())
        {
            game.SetSolverRunning(false);
        }
        else
        {
            game.switchSolver();
        }
    } 
}
