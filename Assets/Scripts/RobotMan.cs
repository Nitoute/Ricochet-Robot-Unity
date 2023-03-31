using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMan : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;

    //Position du tableau
    private int xBoard = -1;
    private int yBoard = -1;

    //Position initial (pour reset)
    private int xInit = -1;
    private int yInit = -1;

    //Sprite de tout les robots
    public Sprite robot_bleue, robot_jaune, robot_rouge, robot_vert;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        switch (this.name)
        {
            case "robot_bleue": this.GetComponent<SpriteRenderer>().sprite = robot_bleue; break;
            case "robot_jaune": this.GetComponent<SpriteRenderer>().sprite = robot_jaune; break;
            case "robot_rouge": this.GetComponent<SpriteRenderer>().sprite = robot_rouge; break;
            case "robot_vert": this.GetComponent<SpriteRenderer>().sprite = robot_vert; break;
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

    public int GetXInit(){
        return xInit;
    }

    public int GetYInit(){
        return yInit;
    }

    public void SetXBoard(int x){
        xBoard = x;
    }

    public void SetYBoard(int y){
        yBoard = y;
    }

    public void SetXInit(int x){
        xInit = x;
    }

    public void SetYInit(int y){
        yInit = y;
    }

    private void OnMouseUp()
    {
        DestroyMovePlates();

        InitiateMovePlates();
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");

        for (int i =0; i<movePlates.Length;i++)
        {
            Destroy(movePlates[i]);
        } 
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "robot_bleue": 
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);

                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "robot_jaune": 
                LineMovePlate(1, 0); //Droite
                LineMovePlate(0, 1); //Haut

                LineMovePlate(-1, 0); //Gauche
                LineMovePlate(0, -1); //Bas
                break;
            case "robot_rouge": 
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);

                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "robot_vert": 
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
               
                break;
        }
    }

    //Déplace le pions sur la ligne + colonne
    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();
        int oldx = xBoard;
        int oldy = yBoard;

        int x = oldx;
        int y = oldy;
        bool mur=false;
        //Si il y a un mur directement dans notre case + direction on ne bouge pas !
        if (!sc.isWallInDir(oldx,oldy,xIncrement,yIncrement)){
            x = xBoard + xIncrement;
            y = yBoard + yIncrement;

            while (sc.PositionOnBoard(x,y) && sc.GetPosition(x,y) == null)
            {
                if (sc.isWallInDir(x,y,xIncrement,yIncrement)){
                    mur = true;
                    break;
                }
                x += xIncrement;
                y += yIncrement;

                
            }
            if(!mur){
                x-=xIncrement;
                y-=yIncrement;
            }
        }
        if (x!=oldx || y!=oldy)MovePlateSpawn(x,y);
    }

    //Déplace le pion comme un fou au échec
    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard +2);
        PointMovePlate(xBoard - 1, yBoard +2);
        PointMovePlate(xBoard + 2, yBoard +1);
        PointMovePlate(xBoard + 2, yBoard -1);
        PointMovePlate(xBoard + 1, yBoard -2);
        PointMovePlate(xBoard - 1, yBoard -2);
        PointMovePlate(xBoard - 2, yBoard +1);
        PointMovePlate(xBoard - 2, yBoard -1);
    }    

    //Déplace le pions tout autour de lui (roi aux échec)
    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard,yBoard +1);
        PointMovePlate(xBoard,yBoard -1);
        PointMovePlate(xBoard -1,yBoard -1);
        PointMovePlate(xBoard -1,yBoard -0);
        PointMovePlate(xBoard -1,yBoard +1);
        PointMovePlate(xBoard +1,yBoard -1);
        PointMovePlate(xBoard +1,yBoard -0);
        PointMovePlate(xBoard +1,yBoard +1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x,y))
        {
            if (sc.GetPosition(x,y) == null)
            {
                MovePlateSpawn(x,y);
            }

        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.15f;
        y *= 0.15f;

        x += -0.12f;
        y += -2.12f;

        GameObject mp = Instantiate(movePlate, new Vector3(x,y,-3.0f), Quaternion.identity);

        MovePlate mpScipt = mp.GetComponent<MovePlate>();
        mpScipt.SetReference(gameObject);
        mpScipt.SetCoords(matrixX,matrixY);
    }

    //Pour VINCENT : Les fonctions suivantes permettent de bouger le robot dans une direction donné par ligne de commande !
    // Tu n'a théoriquement rien à changé pour les déplacer mais si tu voit des bugs ou quoi envoi un message sur discord

    public void MoveRobot(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();
        int oldx = xBoard;
        int oldy = yBoard;

        int x = oldx;
        int y = oldy;
        bool mur=false;
        //Si il y a un mur directement dans notre case + direction on ne bouge pas !
        if (!sc.isWallInDir(oldx,oldy,xIncrement,yIncrement)){
            x = xBoard + xIncrement;
            y = yBoard + yIncrement;

            while (sc.PositionOnBoard(x,y) && sc.GetPosition(x,y) == null)
            {
                if (sc.isWallInDir(x,y,xIncrement,yIncrement)){
                    mur = true;
                    break;
                }
                x += xIncrement;
                y += yIncrement;

                
            }
            if(!mur){
                x-=xIncrement;
                y-=yIncrement;
            }
        }
        if (x!=oldx || y!=oldy)Teleport(x,y);
    }

    public void Teleport(int x,int y)
    {
        controller.GetComponent<Game>().SetPositionEmpty(this.GetXBoard(),this.GetYBoard());
        
        this.SetXBoard(x);
        this.SetYBoard(y);
        this.SetCoords();

        controller.GetComponent<Game>().SetPositionRobot(gameObject);
        controller.GetComponent<Game>().addCoups();
        controller.GetComponent<Game>().hasWin(gameObject);
    }
}
