﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Game : MonoBehaviour
{
    public GameObject robot;
    public GameObject wallPhys;
    public GameObject goal;
    public GameObject solver;
    private Solver solverScript;

    private GameObject[,] positions = new GameObject[16,16];
    private GameObject[] robots = new GameObject[4];
    Stack<GameObject> pileGoals = new Stack<GameObject>();
    private GameObject[] goals = new GameObject[0];

    private int nbrCoups;
    public Text coupText;
    public Text currentGoalText;
    public Image currentGoalImage;
    public GameObject gameOverScreen;
    public GameObject UIScreen;

    private GameObject currentGoal;
    private GameObject currentRobotGoal;
    /*initializing board*/
    public Board board = new Board(2,7,1,8);
    private bool gameOver = false;
    private bool solverRunning = false;
    private bool continueSolveV1 = false;
    private bool continueSolveV2 = false;
    private bool continueSolveV3 = false;
    private bool continueSolveV31 = false;
    private bool continueSolveV4 = false;
    private bool continueSolveV5 = false;

    private System.Random rnd = new System.Random();

    //Dictionary<(int, int), (int, int)[]> walls = new Dictionary<(int, int), (int, int)[]>();

    // Start is called before the first frame update
    void Start()
    {
        UIScreen.SetActive(true);
        //Recupération de l'objet Solver + script
        solver = GameObject.FindGameObjectWithTag("SolverObject");
        solverScript = solver.GetComponent<Solver>();
        addWalls();
        addGoals();
        //Robots
        robots = new GameObject[]{
            CreateRobot("robot_bleue",rnd.Next(0, 16),rnd.Next(0, 16)), CreateRobot("robot_rouge",10, 0), CreateRobot("robot_vert",9, 0), CreateRobot("robot_jaune",rnd.Next(0, 16),rnd.Next(0, 16))
        };

        //Met les robots dans leur cases
        for(int i = 0; i < robots.Length;i++){
            SetPositionRobotInitial(robots[i]);
        }

        pileGoals = new Stack<GameObject>(goals);
        currentGoal = pileGoals.Pop();
        //print("goal init = "+ currentGoal);
        currentGoalText.text = currentGoal.name;
        changeImageGoal();
        currentRobotGoal = GetCurrentRobotGoal();
    }

    private void changeImageGoal()
    {
        SpriteRenderer spriteRenderer = currentGoal.GetComponent<SpriteRenderer>();
        // Copier le sprite
        currentGoalImage.sprite = spriteRenderer.sprite;

        // Copier la couleur
        currentGoalImage.color = spriteRenderer.color;
    }


    private void addWalls(){
        foreach (var wall in board.getWallDict()){
            addWallBis(wall.Key.Item1, 15-(wall.Key.Item2), wall.Value.Item1, wall.Value.Item2, true);
            //Debug.Log("at position " + wall.Key + " walls " + wall.Value);
        }
        int i, j;
        //Ajouter des murs physiques à droite si la position x==15 et ou en bas si y==15
        for(j = 0; j<16; j++){
            for(i=0; i<16; i++){
                if(i==15 && j==15){
                    addWallBis(i, 15-j, 1, -1, true);
                }
                else if(i==15){
                    addWallBis(i, 15-j, 1, 0, true);
                }
                else if(j==15){
                    addWallBis(i, 15-j, 0, -1, true);
                }
            }
        }
    }
    private void addGoals(){
        foreach (var goal in board.getGoalDict()){
            //addWall(wall.Key.Item1, wall.Key.Item2, wall.Value.Item1, wall.Value.Item2, true);
            switch (goal.Value)
            {
                case 11: addGoal("goal_moon_blue", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 12: addGoal("goal_moon_green", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 13: addGoal("goal_moon_red", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 14: addGoal("goal_moon_yellow", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 21: addGoal("goal_pentagon_blue", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 22: addGoal("goal_pentagon_green", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 23: addGoal("goal_pentagon_red", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 24: addGoal("goal_pentagon_yellow", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 31: addGoal("goal_circle_blue", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 32: addGoal("goal_circle_green", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 33: addGoal("goal_circle_red", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 34: addGoal("goal_circle_yellow", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 41: addGoal("goal_star_blue", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 42: addGoal("goal_star_green", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 43: addGoal("goal_star_red", goal.Key.Item1, 15 - goal.Key.Item2); break;
                case 44: addGoal("goal_star_yellow", goal.Key.Item1, 15 - goal.Key.Item2); break;
            }
            //Debug.Log("at position " + goal.Key + " goal " + goal.Value);
        }
        pileGoals = new Stack<GameObject>(goals);
        currentGoal = pileGoals.Pop();
        //print("goal init = "+ currentGoal);
        currentGoalText.text = currentGoal.name;
        currentRobotGoal = GetCurrentRobotGoal();

        switch(currentGoal.GetComponent<GoalMan>().getColor())
            {
                case 0: currentGoalText.color = Color.blue; break;
                case 3: currentGoalText.color = Color.yellow; break;
                case 2: currentGoalText.color = Color.green; break;
                case 1: currentGoalText.color = Color.red; break;

            }
        changeImageGoal();
    }

    private void addGoal(string name, int x, int y){
        GameObject[] newListe = new GameObject[goals.Length+1];
        
        for (int i = 0 ; i<=goals.Length-1;i++)
        {
            newListe[i] = goals[i];

        }
        newListe[newListe.Length-1] = InstantiateGoal(name,x,y);
        goals=newListe;
    }

    public GameObject getRobot(int p){
        return robots[p];
    }

    public List<(int,int)> getPositionRobots(){
        List<(int,int)> pos=new List<(int,int)>();
        for (int i=0;i<4;i++){
            pos.Add((robots[i].GetComponent<RobotMan>().GetXBoard(),robots[i].GetComponent<RobotMan>().GetYBoard()));
        }
        return pos;
    }

    public List<(int,int)> getPositionInitRobot(){
        List<(int,int)> pos=new List<(int,int)>();
        for (int i=0;i<4;i++){
            pos.Add((robots[i].GetComponent<RobotMan>().GetXInit(),robots[i].GetComponent<RobotMan>().GetYInit()));
        }
        return pos;
    }

    public void setPositionRobot( List<(int,int)> pos){
        for (int i=0;i<4;i++){
            (int x,int y)=pos[i];
            robots[i].GetComponent<RobotMan>().Teleport(x,y);
        }
        for (int i=0;i<4;i++){
            (int x,int y)=pos[i];
            robots[i].GetComponent<RobotMan>().Teleport(x,y);
        }
    }

    public GameObject CreateRobot(string name, int x, int y)
    {
        while ((x==7 && y==7) || (x==7 && y==8) || (x==8 && y==7) || (x==8 && y==8))
        {
            x = rnd.Next(0, 16);
            y = rnd.Next(0, 16);
        }
        GameObject obj = Instantiate(robot, new Vector3(0,0,-1),Quaternion.identity);
        RobotMan rm = obj.GetComponent<RobotMan>();
        rm.name = name;
        rm.SetXBoard(x);
        rm.SetYBoard(y);
        rm.SetXInit(x);
        rm.SetYInit(y);
        rm.Activate();
        //hasWin(obj);
        return obj;

    }

    public GameObject CreateWall(int xPos,int yPos, int xDir, int yDir)
    {
        GameObject obj = Instantiate(wallPhys, new Vector3(0,0,-1),Quaternion.identity);
        WallMan wm = obj.GetComponent<WallMan>();
        wm.SetXBoard(xPos);
        wm.SetYBoard(yPos);
        wm.SetxDir(xDir);
        wm.SetyDir(yDir);
        wm.Activate();
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

    public void SetPositionRobotInitial(GameObject obj)
    {
        RobotMan rm = obj.GetComponent<RobotMan>();
        if (positions[rm.GetXBoard(),rm.GetYBoard()]==null)
        {
            positions[rm.GetXBoard(),rm.GetYBoard()] = obj;
        }else{
            int newX = rnd.Next(0, 16);
            int newY = rnd.Next(0, 16);
            rm.SetXBoard(newX);
            rm.SetYBoard(newY);
            rm.SetXInit(newX);
            rm.SetYInit(newY);
            rm.Activate();
            SetPositionRobot(obj);
        }
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

    public void SetSolverRunning(bool cond)
    {
        solverRunning = cond;
    }

    public GameObject GetCurrentGoal()
    {
        return currentGoal;
    }

    public GameObject GetActiveRobot()
    {
        return currentRobotGoal;
    }


    public GameObject GetCurrentRobotGoal()
    {
        switch (currentGoal.GetComponent<GoalMan>().getColor())
        {
            case 0:
                return robots[0];
            case 1:
                return robots[1];
            case 2:
                return robots[2];
            case 3:
                return robots[3];
        }
        return null;
    }

    public void switchContinueSolveV1(){
        continueSolveV1=!continueSolveV1;
        solverRunning=!solverRunning;
    }

    public void switchContinueSolveV2(){
        continueSolveV2=!continueSolveV2;
        solverRunning=!solverRunning;
    }

    public void switchContinueSolveV3(){
        continueSolveV3=!continueSolveV3;
        solverRunning=!solverRunning;
    }

    public void switchContinueSolveV31(){
        continueSolveV31=!continueSolveV31;
        solverRunning=!solverRunning;
    }

    public void switchContinueSolveV4(){
        continueSolveV4=!continueSolveV4;
        solverRunning=!solverRunning;
    }
    public void switchContinueSolveV5(){
        continueSolveV5=!continueSolveV5;
        solverRunning=!solverRunning;
    }

    public bool getContinueSolveV1(){
        return continueSolveV1;
    }

    public bool getContinueSolveV2(){
        return continueSolveV2;
    }

    public bool getContinueSolveV3(){
        return continueSolveV3;
    }
    public bool getContinueSolveV31(){
        return continueSolveV31;
    }
    public bool getContinueSolveV4(){
        return continueSolveV4;
    }
    public bool getContinueSolveV5(){
        return continueSolveV5;
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x<0 || y<0 || x>= positions.GetLength(0) || y>=positions.GetLength(1)) return false;
        return true;
    }

    //Privilégier isWallInDir depuis classe board.
    /*
    public bool isWallInDir(int x, int y,int dirX, int dirY)
    {
        try
        {
            (int,int)[] li = walls[(x,y)];
            //IDictionary<(int i, int j),(int right, int top)> li = board.getWallDict();
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
        if (walls.ContainsKey((x,y)))
        {
            (int,int)[] newListe = new (int,int)[walls[(x,y)].Length + 1];
            for (int i = 0; i<newListe.Length-1; i++)
            {
                newListe[i] = walls[(x,y)][i];
            }
            newListe[newListe.Length -1] = (dirX,dirY);
            walls[(x,y)] = newListe;
            if (newWall)
            {
                CreateWall(x, y, dirX,dirY);
                addWall(x+dirX,y+dirY,-dirX,-dirY,false);
            }
        }
        else
        {
            walls.Add((x, y), new (int, int)[] {(dirX, dirY)});
            if (newWall)
            {
                CreateWall(x, y, dirX,dirY);
                addWall(x+dirX,y+dirY,-dirX,-dirY,false);
            }
        }
    }*/

    private void addWallBis(int x, int y){
        if(board.isWallInPos(x,y,3)){
            CreateWall(x, y, -1, 0);
        }
        if(board.isWallInPos(x,y,0)){
            CreateWall(x, y, 0, 1);
        }
    }

    private void addWallBis(int x, int y, int dirX,int dirY,bool newWall){
        if(dirX==0 || dirY==0){
            CreateWall(x, y, dirX, dirY);
        }
        else{
            CreateWall(x, y, dirX, 0);
            CreateWall(x, y, 0, dirY);
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
            switch(currentGoal.GetComponent<GoalMan>().getColor())
            {
                case 0: currentGoalText.color = Color.blue; break;
                case 3: currentGoalText.color = Color.yellow; break;
                case 2: currentGoalText.color = Color.green; break;
                case 1: currentGoalText.color = Color.red; break;

            }
            nbrCoups = 0;
            coupText.text = nbrCoups.ToString();
            currentRobotGoal = GetCurrentRobotGoal();
            changeImageGoal();
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
            robots[i].GetComponent<RobotMan>().Teleport(robots[i].GetComponent<RobotMan>().GetXInit(),robots[i].GetComponent<RobotMan>().GetYInit());
            //SetPositionDefaultRobot(robots[i]);
            robots[i].GetComponent<RobotMan>().DestroyMovePlates();
        }
        for(int i = 0; i < robots.Length;i++){
            robots[i].GetComponent<RobotMan>().Teleport(robots[i].GetComponent<RobotMan>().GetXInit(),robots[i].GetComponent<RobotMan>().GetYInit());
        }

        nbrCoups = 0;
        coupText.text = nbrCoups.ToString();
    }

    public void SetPositionDefaultRobots(List<(int, int)> positions)
    {
        for(int i=0; i<4;i++)
        {
            (int x,int y) = positions[i];
            robots[i].GetComponent<RobotMan>().SetXInit(x);
            robots[i].GetComponent<RobotMan>().SetYInit(y);
            SetPositionDefaultRobot(robots[i]);
        }
        
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

    public bool hasWin(GameObject rob)
    {

        RobotMan rm = rob.GetComponent<RobotMan>();
        int Yobj = currentGoal.GetComponent<GoalMan>().GetYBoard();
        int Xobj = currentGoal.GetComponent<GoalMan>().GetXBoard();

        int Yrob = rob.GetComponent<RobotMan>().GetYBoard();
        int Xrob = rob.GetComponent<RobotMan>().GetXBoard();

        switch (rob.name)
        {
            case "robot_bleue":
                if (currentGoal.GetComponent<GoalMan>().getColor() == 0)
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        if (!solverRunning){
                            updateGoal();
                            foreach(GameObject robo in robots)
                            {
                            robo.GetComponent<RobotMan>().switchPositionInit();
                            }
                        }
                        return true;
                    }
                }
                break;
            case "robot_jaune":
                if (currentGoal.GetComponent<GoalMan>().getColor() == 3)
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        print("gagné 1!");
                        if (!solverRunning){
                            print("gagné 2!");
                            updateGoal();
                            foreach(GameObject robo in robots)
                            {
                            robo.GetComponent<RobotMan>().switchPositionInit();
                            }

                        }
                        return true;
                    }
                }
                break;
            case "robot_rouge":
                if (currentGoal.GetComponent<GoalMan>().getColor() == 1)
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        if (!solverRunning){
                            updateGoal();
                            foreach(GameObject robo in robots)
                            {
                            robo.GetComponent<RobotMan>().switchPositionInit();
                            }
                        }
                        return true;
                    }
                }
                break;
            case "robot_vert":
               if (currentGoal.GetComponent<GoalMan>().getColor() == 2)
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        if (!solverRunning){
                            updateGoal();
                            foreach(GameObject robo in robots)
                            {
                            robo.GetComponent<RobotMan>().switchPositionInit();
                            }
                        }
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    //SOLVER V1 (VINCENT)

    /*public bool touchGoal(GameObject rob)
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
                        return true;
                    }
                }
                break;
            case "robot_jaune":
                if (currentGoal.name == "goal_jaune")
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        return true;
                    }
                }
                break;
            case "robot_rouge":
                if (currentGoal.name == "goal_rouge")
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        return true;
                    }
                }
                break;
            case "robot_vert":
               if (currentGoal.name == "goal_vert")
                {
                    if (Yrob==Yobj && Xrob==Xobj)
                    {
                        return true;
                    }
                }
                break;
        }

        return false;
    }*/

    public void changeBoard(int i, int j, int x, int y)
    {
        DestroyAllWalls();
        DestroyAllGoals();
        goals = new GameObject[0];
        board = new Board(i,j,x,y);
        addWalls();
        addGoals();
    }

    private void DestroyAllWalls()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        for (int i =0; i<walls.Length;i++)
        {
            Destroy(walls[i]);
        } 
    }
    
    private void DestroyAllGoals()
    {
        GameObject[] goalsObject = GameObject.FindGameObjectsWithTag("Goal");

        for (int i =0; i<goalsObject.Length;i++)
        {
            Destroy(goalsObject[i]);
        } 
    }


}
