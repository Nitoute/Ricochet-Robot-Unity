using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Solution : MonoBehaviour
{
    public Game game;
    public Text text;
    private Solver solver;
    public void Start(){
        solver = GameObject.FindGameObjectWithTag("SolverObject").GetComponent<Solver>();
    }
    public void Update()
    {
        if (game.getSolverRunning())
        {
            text.text = "En cours de calcul...";
        }
        else
        {
            if (solver.getFinalLen() == -1)
            {
                text.text = "Aucun calcul en cours";
            }
            else
            {
                string solution = solver.Seq2String(solver.getFinalSeq(), solver.getFinalLen());
                text.text = solution;
            }
        }
    }
    
}
