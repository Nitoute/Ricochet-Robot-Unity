using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMan : MonoBehaviour
{
    public GameObject controller;

    //Position du tableau
    private int xBoard = -1;
    private int yBoard = -1;

    //Sprite de tout les robots
    public Sprite goal_bleue, goal_jaune, goal_rouge, goal_vert;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        switch (this.name)
        {
            case "goal_bleue": this.GetComponent<SpriteRenderer>().sprite = goal_bleue; break;
            case "goal_jaune": this.GetComponent<SpriteRenderer>().sprite = goal_jaune; break;
            case "goal_rouge": this.GetComponent<SpriteRenderer>().sprite = goal_rouge; break;
            case "goal_vert": this.GetComponent<SpriteRenderer>().sprite = goal_vert; break;
        }
    }

    public void SetCoords() {
        float x = xBoard;
        float y = yBoard;

        x *= 0.15f;
        y *= 0.15f;

        x += -0.12f;
        y += -2.12f;

        this.transform.position = new Vector3(x,y,-1.0f);
    }

    public int GetXBoard(){
        return xBoard;
    }

    public int GetYBoard(){
        return yBoard;
    }

    public void SetXBoard(int x){
        xBoard = x;
    }

    public void SetYBoard(int y){
        yBoard = y;
    }
}
