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
        int[] seed=new int[4];
        //on prend les 4 premiers chiffres pour changer le plateau
        for (int i = 0; i< 4; i++)
        {
            seed[i] = int.Parse(s[i]);
        }
        // si la seed donné fait 12 de long 
        if (s.Length==12){
            //alors on prend les 8 nombres suivant pour définir la position des pions
            List<(int,int)> posPion = new List<(int,int)>();
            for (int i = 4; i< 12; i+=2)
            {
                print(i);
                print(int.Parse(s[i]));
                print(int.Parse(s[i+1]));
                posPion.Add((int.Parse(s[i]),int.Parse(s[i+1])));
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

