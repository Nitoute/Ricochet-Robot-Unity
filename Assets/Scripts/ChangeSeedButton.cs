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
        print("a");
        string[] s = inputfield.getInput().Split(',');
        int[] seed=new int[4];
        for (int i = 0; i< 4; i++)
        {
            seed[i] = int.Parse(s[i]);
        }
        if (s.Length==12){
            List<(int,int)> posPion = new List<(int,int)>();
            for (int i = 4; i< 12; i+=2)
            {
                posPion[i] = (int.Parse(s[i]),int.Parse(s[i+1]));
                try{
                    game.SetPositionDefaultRobots(posPion);
                }
                catch{
                    print("Les positions données ne sont pas bonnes");
                }
            }
        }
        game.changeSeed(seed[0], seed[1], seed[2], seed[3]);
    }
}

