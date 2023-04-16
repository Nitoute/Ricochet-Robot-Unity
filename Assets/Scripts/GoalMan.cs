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
    public Sprite goal_moon, goal_pentagon, goal_circle, goal_star;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        this.GetComponent<SpriteRenderer>().sprite = goal_star;
        SetCoords();

        switch (this.name)
        {
            case "goal_moon_blue": this.GetComponent<SpriteRenderer>().sprite = goal_moon; this.GetComponent<SpriteRenderer>().color = Color.blue; break;
            case "goal_moon_green": this.GetComponent<SpriteRenderer>().sprite = goal_moon; this.GetComponent<SpriteRenderer>().color = Color.green; break;
            case "goal_moon_red": this.GetComponent<SpriteRenderer>().sprite = goal_moon; this.GetComponent<SpriteRenderer>().color = Color.red; break;
            case "goal_moon_yellow": this.GetComponent<SpriteRenderer>().sprite = goal_moon; this.GetComponent<SpriteRenderer>().color = Color.yellow; break;
            case "goal_pentagon_blue": this.GetComponent<SpriteRenderer>().sprite = goal_pentagon; this.GetComponent<SpriteRenderer>().color = Color.blue; break;
            case "goal_pentagon_green": this.GetComponent<SpriteRenderer>().sprite = goal_pentagon; this.GetComponent<SpriteRenderer>().color = Color.green; break;
            case "goal_pentagon_red": this.GetComponent<SpriteRenderer>().sprite = goal_pentagon; this.GetComponent<SpriteRenderer>().color = Color.red; break;
            case "goal_pentagon_yellow": this.GetComponent<SpriteRenderer>().sprite = goal_pentagon; this.GetComponent<SpriteRenderer>().color = Color.yellow; break;
            case "goal_circle_blue": this.GetComponent<SpriteRenderer>().sprite = goal_circle; this.GetComponent<SpriteRenderer>().color = Color.blue; break;
            case "goal_circle_green": this.GetComponent<SpriteRenderer>().sprite = goal_circle; this.GetComponent<SpriteRenderer>().color = Color.green; break;
            case "goal_circle_red": this.GetComponent<SpriteRenderer>().sprite = goal_circle; this.GetComponent<SpriteRenderer>().color = Color.red; break;
            case "goal_circle_yellow": this.GetComponent<SpriteRenderer>().sprite = goal_circle; this.GetComponent<SpriteRenderer>().color = Color.yellow; break;
            case "goal_star_blue": this.GetComponent<SpriteRenderer>().sprite = goal_star; this.GetComponent<SpriteRenderer>().color = Color.blue; break;
            case "goal_star_green": this.GetComponent<SpriteRenderer>().sprite = goal_star; this.GetComponent<SpriteRenderer>().color = Color.green; break;
            case "goal_star_red": this.GetComponent<SpriteRenderer>().sprite = goal_star; this.GetComponent<SpriteRenderer>().color = Color.red; break;
            case "goal_star_yellow": this.GetComponent<SpriteRenderer>().sprite = goal_star; this.GetComponent<SpriteRenderer>().color = Color.yellow; break;
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
    public int getColor()
    {
        switch (this.name)
        {
            /*case "goal_bleue": return 0; break;
            case "goal_jaune": return 3; break;
            case "goal_rouge": return 1; break;
            case "goal_vert": return 2; break;*/
            case "goal_moon_blue": return 0; break;
            case "goal_moon_green": return 2; break;
            case "goal_moon_red": return 1; break;
            case "goal_moon_yellow": return 3; break;
            case "goal_pentagon_blue": return 0; break;
            case "goal_pentagon_green": return 2; break;
            case "goal_pentagon_red": return 1; break;
            case "goal_pentagon_yellow": return 3; break;
            case "goal_circle_blue": return 0; break;
            case "goal_circle_green": return 2; break;
            case "goal_circle_red": return 1; break;
            case "goal_circle_yellow": return 3; break;
            case "goal_star_blue": return 0; break;
            case "goal_star_green": return 2; break;
            case "goal_star_red": return 1; break;
            case "goal_star_yellow": return 3; break;
        }
        return -1;
    }
}
