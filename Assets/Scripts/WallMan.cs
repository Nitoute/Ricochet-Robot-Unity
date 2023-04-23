using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMan : MonoBehaviour
{
    public GameObject controller;
    public Game game;

    //Position du tableau
    private int xBoard = -1;
    private int yBoard = -1;

    //Direction du mur
    private int xDir = -1;
    private int yDir = -1;

    //Sprite de tout les robots
    public Sprite wall_horizontal, wall_vertical;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //Modifier les valeurs de multiplication et d'addication
    public void SetCoords() {
        float x = xBoard;
        float y = yBoard;

        x *= 0.15f;
        y *= 0.15f;
        
        //(1,0) : droite | (-1,0) : gauche | (0,1) : haut | (0,-1) : bas
        switch ((xDir,yDir))
        { 
            case var t when t == (1,0): x += -0.05f;y += -2.12f; this.transform.localScale = new Vector3(0.07f,0.23f,1.0f);break;
            case var t when t == (-1,0):  x += -0.20f;y += -2.12f; this.transform.localScale = new Vector3(0.07f,0.23f,1.0f);break;
            case var t when t == (0,1):  x += -0.10f;y += -2.05f; this.transform.localScale = new Vector3(0.6f,0.03f,1.0f);break;
            case var t when t == (0,-1):  x += -0.1f;y += -2.2f; this.transform.localScale = new Vector3(0.6f,0.03f,1.0f);break;
        }
        

        this.transform.position = new Vector3(x,y,-1.0f);
    }

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();

        SetCoords();

        //(1,0) : droite | (-1,0) : gauche | (0,1) : haut | (0,-1) : bas
        switch ((xDir,yDir))
        { 
            case var t when t == (1,0): this.GetComponent<SpriteRenderer>().sprite = wall_vertical; break;
            case var t when t == (-1,0): this.GetComponent<SpriteRenderer>().sprite = wall_vertical; break;
            case var t when t == (0,1): this.GetComponent<SpriteRenderer>().sprite = wall_vertical; break;
            case var t when t == (0,-1): this.GetComponent<SpriteRenderer>().sprite = wall_vertical; break;
        }
    }

    public int GetXBoard(){
        return xBoard;
    }

    public int GetYBoard(){
        return yBoard;
    }

    public int GetxDir(){
        return xDir;
    }

    public int GetyDir(){
        return yDir;
    }
    
    public void SetXBoard(int x){
        xBoard = x;
    }

    public void SetYBoard(int y){
        yBoard = y;
    }

    public void SetxDir(int x){
        xDir = x;
    }

    public void SetyDir(int y){
        yDir = y;
    }
}
