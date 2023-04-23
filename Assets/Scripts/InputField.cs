using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField : MonoBehaviour
{
    private string input;
    public void ReadStringInput(string s)
    {
        input = s;

    }

    public string getInput()
    {
        print("b");
        return input;
    }
}
