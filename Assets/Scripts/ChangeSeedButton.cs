using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSeedButton : MonoBehaviour
{
    public InputField inputfield;
    public Game game;

    public void Start()
    {
        inputfield = GameObject.FindGameObjectWithTag("InputField").GetComponent<InputField>();
    }
    public void TaskOnClick()
    {
        string[] s = inputfield.getInput().Split(',');
        int[] seed = new int[4];
        for (int i = 0; i< 4; i++)
        {
            seed[i] = int.Parse(s[i]);
        }
        game.changeSeed(seed[0], seed[1], seed[2], seed[3]);
    }
}
