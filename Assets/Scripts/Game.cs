using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject robot;
    public GameObject goal;

    private GameObject[,] positions = new GameObject[16,16];
    
    private GameObject[] robots = new GameObject[4];
    Stack<GameObject> pileGoals = new Stack<GameObject>();
    private GameObject[] goals = new GameObject[4];

    private int nbrCoups;
    public Text coupText;
    public Text currentGoalText;
    public GameObject gameOverScreen;
    public GameObject UIScreen;

    private GameObject currentGoal;

    Dictionary<(int, int), (int, int)[]> myDictionary = new Dictionary<(int, int), (int, int)[]>();

    private bool gameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        UIScreen.SetActive(true);

        //Walls légende : (0,1) haut | (0,-1) bas | (1,0) droite | (-1,0) gauche;
        addWall(5,0,1,0,true);
        addWall(11,0,1,0,true);
        addWall(2,0,0,1,true);
        addWall(1,1,1,0,true);
        addWall(14,1,0,1,true);
        addWall(14,1,1,0,true);
        addWall(9,1,0,1,true);
        addWall(6,2,0,1,true);
        addWall(8,2,1,0,true);
        addWall(6,3,1,0,true);
        addWall(0,4,0,1,true);
        addWall(8,4,0,1,true);
        addWall(12,4,1,0,true);
        addWall(13,4,0,1,true);
        addWall(8,5,1,0,true);
        addWall(15,5,0,1,true);
        addWall(1,6,1,0,true);
        addWall(1,6,0,1,true);
        addWall(4,6,1,0,true);
        addWall(5,6,0,1,true);
        addWall(7,6,0,1,true);
        addWall(8,6,0,1,true);
        addWall(15,6,0,-1,true);
        addWall(6,7,1,0,true);
        addWall(9,7,-1,0,true);
        addWall(2,8,0,1,true);
        addWall(6,8,1,0,true);
        addWall(9,8,-1,0,true);
        addWall(2,9,1,0,true);
        addWall(7,9,0,-1,true);
        addWall(8,9,0,-1,true);
        addWall(9,9,0,1,true);
        addWall(11,9,0,1,true);
        addWall(11,9,1,0,true);
        addWall(6,10,0,1,true);
        addWall(9,10,1,0,true);
        addWall(15,10,0,1,true);
        addWall(0,11,0,1,true);
        addWall(6,11,1,0,true);
        addWall(8,11,0,1,true);
        addWall(1,12,0,1,true);
        addWall(7,12,1,0,true);
        addWall(14,12,0,1,true);
        addWall(0,13,1,0,true);
        addWall(13,13,1,0,true);
        addWall(5,14,1,0,true);
        addWall(5,14,0,1,true);
        addWall(11,14,1,0,true);
        addWall(12,14,0,1,true);
        addWall(3,15,1,0,true);
        addWall(10,15,1,0,true);
        
        
        //Robots
        System.Random rnd = new System.Random();
        robots = new GameObject[]{
            CreateRobot("robot_bleue",rnd.Next(0, 16),rnd.Next(0, 16)), CreateRobot("robot_rouge",rnd.Next(0, 16),rnd.Next(0, 16)), CreateRobot("robot_vert",rnd.Next(0, 16),rnd.Next(0, 16)), CreateRobot("robot_jaune",rnd.Next(0, 16),rnd.Next(0, 16))
        };

        //Met les robots dans leur cases
        for(int i = 0; i < robots.Length;i++){
            SetPositionRobot(robots[i]);
        }

        //Goals
    
        goals = new GameObject[]{
            InstantiateGoal("goal_rouge",14,1), InstantiateGoal("goal_bleue",5,14), InstantiateGoal("goal_vert",2,1), InstantiateGoal("goal_jaune",11,9)
        };

        pileGoals = new Stack<GameObject>(goals);

        currentGoal = pileGoals.Pop();
        currentGoalText.text = currentGoal.name;

        // Pour VINCENT : Un exemple de comment bouger le robot[0] (bleu) le "vecteur" en paramètre représente la direction selon l'axe
        // x et l'axe y, (1,0) : droite | (-1,0) : gauche | (0,1) : haut | (0,-1) : bas

        robots[0].GetComponent<RobotMan>().MoveRobot(1,0);
        robots[0].GetComponent<RobotMan>().MoveRobot(0,1);
        
    }

    public GameObject CreateRobot(string name, int x, int y)
    {
        GameObject obj = Instantiate(robot, new Vector3(0,0,-1),Quaternion.identity);
        RobotMan rm = obj.GetComponent<RobotMan>();
        rm.name = name;
        rm.SetXBoard(x);
        rm.SetYBoard(y);
        rm.SetXInit(x);
        rm.SetYInit(y);
        rm.Activate();
        return obj;
        
    }

    public GameObject InstantiateGoal(string name, int x, int y)
    {
        GameObject obj = Instantiate(goal, new Vector3(0,0,-1),Quaternion.identity);
        GoalMan gm = obj.GetComponent<GoalMan>();
        gm.name = name;
        gm.SetXBoard(x);
        gm.SetYBoard(y);
        gm.Activate();
        return obj;
        
    }

    public void SetPositionRobot(GameObject obj)
    {
        RobotMan rm = obj.GetComponent<RobotMan>();

        positions[rm.GetXBoard(),rm.GetYBoard()] = obj;
    }

    public void SetPositionGoal(GameObject obj)
    {
        GoalMan gm = obj.GetComponent<GoalMan>();

        positions[gm.GetXBoard(),gm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x,y]= null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x,y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x<0 || y<0 || x>= positions.GetLength(0) || y>=positions.GetLength(1)) return false;
        return true;
    }

    public bool isWallInDir(int x, int y,int dirX, int dirY)
    {
        try
        {
            (int,int)[] li = myDictionary[(x,y)];
            
            foreach ((int,int) item in li)
            {
                if (item==(dirX,dirY)){
                    return true;
                }
            }
            return false;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    private void addWall(int x, int y, int dirX,int dirY,bool newWall)
    {
        if (myDictionary.ContainsKey((x,y)))
        {
            (int,int)[] newListe = new (int,int)[myDictionary[(x,y)].Length + 1];
            for (int i = 0; i<newListe.Length-1; i++)
            {
                newListe[i] = myDictionary[(x,y)][i];
            }
            newListe[newListe.Length -1] = (dirX,dirY);
            myDictionary[(x,y)] = newListe;
            if (newWall) addWall(x+dirX,y+dirY,-dirX,-dirY,false);
        }
        else
        {
            myDictionary.Add((x, y), new (int, int)[] {(dirX, dirY)});
            if (newWall) addWall(x+dirX,y+dirY,-dirX,-dirY,false);
        }
    }

    public void addCoups()
    {
        nbrCoups = nbrCoups +1;
        coupText.text = nbrCoups.ToString();
    }

    private void updateGoal()
    {
        if (pileGoals.Count!=0)
        {
            Destroy(currentGoal);
            currentGoal = pileGoals.Pop();
            currentGoalText.text = currentGoal.name;
            nbrCoups = 0;
            coupText.text = nbrCoups.ToString();
        }
        else
        {
            gameOver = true;
            UIScreen.SetActive(false);
            gameOverScreen.SetActive(true);
        }
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void restartPosition()
    {
        for(int i = 0; i < robots.Length;i++){
            SetPositionDefaultRobot(robots[i]);
        }

        nbrCoups = 0;
        coupText.text = nbrCoups.ToString();
    }

    public void SetPositionDefaultRobot(GameObject obj)
    {
        RobotMan rm = obj.GetComponent<RobotMan>();

        SetPositionEmpty(rm.GetXBoard(),rm.GetYBoard());
        rm.SetXBoard(rm.GetXInit());
        rm.SetYBoard(rm.GetYInit());
        rm.SetCoords();

        SetPositionRobot(obj);        
    }

    public void hasWin(GameObject rob)
    {
        int Yobj = currentGoal.GetComponent<GoalMan>().GetYBoard();
        int Xobj = currentGoal.GetComponent<GoalMan>().GetXBoard();

        int Yrob = rob.GetComponent<RobotMan>().GetYBoard();
        int Xrob = rob.GetComponent<RobotMan>().GetXBoard();
        switch (rob.name)
        {
            case "robot_bleue": 
                if (currentGoal.name == "goal_bleue")
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        updateGoal();
                    }
                } 
                break;
            case "robot_jaune": 
                if (currentGoal.name == "goal_jaune")
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        updateGoal();
                    }
                } 
                break;
            case "robot_rouge": 
                if (currentGoal.name == "goal_rouge")
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        updateGoal();
                    }
                } 
                break;
            case "robot_vert": 
               if (currentGoal.name == "goal_vert")
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        updateGoal();
                    }
                } 
                break;
        }
    }

}
