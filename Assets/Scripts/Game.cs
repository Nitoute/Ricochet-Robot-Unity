using System.Collections;
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
    
    
    
    private GameObject[,] positions = new GameObject[16,16];//matrice 16x16 de gameObject contenant des robots/goals si il y en a null sinon
    private GameObject[] robots = new GameObject[4]; //liste de tout les robots présents dans le jeu
    Stack<GameObject> pileGoals = new Stack<GameObject>(); //piles de goal utilisé pour récupérer un goal et vidé la liste goals
    private GameObject[] goals = new GameObject[0]; //liste de goal récupérer depuis board

    //Partie UI
    private int nbrCoups;
    public Text coupText;
    public Text currentGoalText;
    public Text currentSeedText;
    public Image currentGoalImage;
    public GameObject gameOverScreen;
    public GameObject UIScreen;
    public Dropdown dropdown;

    private GameObject currentGoal;
    private GameObject currentRobotGoal;
    public Board board = new Board(0,0,0,0);
    private bool solverRunning = false;
    private System.Random rnd = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        UIScreen.SetActive(true);
        //Recupération de l'objet Solver + script
        addWalls();
        addGoals();
        //Robots
        robots = new GameObject[]{
            CreateRobot("robot_bleue",rnd.Next(0, 16),rnd.Next(0, 16)), CreateRobot("robot_rouge",10, 0), CreateRobot("robot_vert",9, 0), CreateRobot("robot_jaune",rnd.Next(0, 16),rnd.Next(0, 16))
        };

        //Met les robots dans leur cases
        for(int i = 0; i < robots.Length;i++){
            SetPositionRobotInitial(robots[i]);
            hasWin(robots[i]);
        }

        pileGoals = new Stack<GameObject>(goals); //initialisation de la pile depuis la liste précédement récupérer
        currentGoal = pileGoals.Pop(); //Récupère le premier élément de la pile et le retire de la liste
        string[] goalname = currentGoal.name.Split('_');
        currentGoalText.text = UppercaseFirst(goalname[2]) + ' ' + UppercaseFirst(goalname[1]); //nom du goal + couleur
        int[] seed = board.getSeed();
        currentSeedText.text = seed[0] + "," + seed[1] + "," + seed[2] + "," + seed[3];
        changeImageGoal();
        currentRobotGoal = GetCurrentRobotGoal();
    }

    private void changeImageGoal()
    {
        // Récupére les paramètres du sprite de currentGoal
        SpriteRenderer spriteRenderer = currentGoal.GetComponent<SpriteRenderer>();
        // Copier le sprite
        currentGoalImage.sprite = spriteRenderer.sprite;

        // Copier la couleur
        currentGoalImage.color = spriteRenderer.color;
    }

    public GameObject CreateRobot(string name, int x, int y)
    {
        while ((x==7 && y==7) || (x==7 && y==8) || (x==8 && y==7) || (x==8 && y==8)) // Vérifie si ils ne sont pas sur les cases du milieu
        {
            x = rnd.Next(0, 16);
            y = rnd.Next(0, 16);
        }
        //Instantiation des différents paramètre de l'objet robot
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
    
    //Instantiation des différents paramètre de l'objet mur
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

    //Instantiation des différents paramètre de l'objet goal
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

    //Vérifie si x et y sont toujours dans le plateau
    public bool PositionOnBoard(int x, int y)
    {
        if (x<0 || y<0 || x>= positions.GetLength(0) || y>=positions.GetLength(1)) return false;
        return true;
    }

    

    //Met a jour le goal courrant et vérifie si il en reste pour la terminaison
    private void updateGoal()
    {
        if (pileGoals.Count!=0)
        {
            Destroy(currentGoal);
            currentGoal = pileGoals.Pop();
            string[] goalname = currentGoal.name.Split('_');
            currentGoalText.text = UppercaseFirst(goalname[2]) + ' ' + UppercaseFirst(goalname[1]);
            switch (currentGoal.GetComponent<GoalMan>().getColor())
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
            UIScreen.SetActive(false);
            gameOverScreen.SetActive(true);
        }
    }

    private string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void restartPosition()
    {
        for(int i = 0; i < robots.Length;i++){
            robots[i].GetComponent<RobotMan>().Teleport(robots[i].GetComponent<RobotMan>().GetXInit(),robots[i].GetComponent<RobotMan>().GetYInit());
            robots[i].GetComponent<RobotMan>().DestroyMovePlates();
        }
        for(int i = 0; i < robots.Length;i++){
            robots[i].GetComponent<RobotMan>().Teleport(robots[i].GetComponent<RobotMan>().GetXInit(),robots[i].GetComponent<RobotMan>().GetYInit()); // Pour éviter que deux pions soit mal déplacer
        }

        nbrCoups = 0; 
        coupText.text = nbrCoups.ToString();
    }

    
    
    //Vérifie les conditions de victoire à chaque déplacement
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

    //Vérifie si un pion est sur la case contact à x,y dans la direction dir
    public bool isPionInDir(int x,int y, int dir){
        switch(dir){
            case 0:
                if (y==15 || GetPosition(x,y+1)==null){
                    return false;
                }
            break;
            case 1:
                if (x==15 || GetPosition(x+1,y)==null){
                    return false;
                }
            break;
            case 2:
                if (y==0 || GetPosition(x,y-1)==null){
                    return false;
                }
            break;
            case 3:
                if (x==0 || GetPosition(x-1,y)==null){
                    return false;
                }
            break;
        }
        return true;
    }

    //change le plateau en fonction de la seed (appeler changeSeed)
    public void changeBoard(int i, int j, int x, int y)
    {
        DestroyAllWalls();
        DestroyAllGoals();
        goals = new GameObject[0];
        board = new Board(i,j,x,y);
        int[] seed = board.getSeed();
        currentSeedText.text = seed[0] + "," + seed[1] + "," + seed[2] + "," + seed[3];
        addWalls();
        addGoals();
    }

    //change le plateau en fonction de la seed et remet les robots à leur place
    public void changeSeed(int a, int b, int c, int d)
    {
       
     
        changeBoard(a, b, c, d);

        //Reseting robots positions
        restartPosition();
    }

    //détruit tout les murs
    private void DestroyAllWalls()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        for (int i =0; i<walls.Length;i++)
        {
            Destroy(walls[i]);
        } 
    }
    //détruit tout les goals
    private void DestroyAllGoals()
    {
        GameObject[] goalsObject = GameObject.FindGameObjectsWithTag("Goal");

        for (int i =0; i<goalsObject.Length;i++)
        {
            Destroy(goalsObject[i]);
        } 
    }


    public void addCoups()
    {
        nbrCoups = nbrCoups +1;
        coupText.text = nbrCoups.ToString();
    }
    
    private void addWalls(){
        foreach (var wall in board.getWallDict()){
            addWallBis(wall.Key.Item1, 15-(wall.Key.Item2), wall.Value.Item1, wall.Value.Item2, true);
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

    //créer les murs en haut et à gauche
    private void addWallBis(int x, int y){
        if(board.isWallInPos(x,y,3)){
            CreateWall(x, y, -1, 0);
        }
        if(board.isWallInPos(x,y,0)){
            CreateWall(x, y, 0, 1);
        }
    }

    //créer les murs à droite et en bas
    private void addWallBis(int x, int y, int dirX,int dirY,bool newWall){
        if(dirX==0 || dirY==0){
            CreateWall(x, y, dirX, dirY);
        }
        else{
            CreateWall(x, y, dirX, 0);
            CreateWall(x, y, 0, dirY);
        }
    }

    //Récupération des différents goals dans board
    //Switch pour récupérer tout les goals possible
    //Met a jour la pile et le currentgoal
    private void addGoals(){
        foreach (var goal in board.getGoalDict()){
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
        }
        pileGoals = new Stack<GameObject>(goals);
        currentGoal = pileGoals.Pop();
        string[] goalname = currentGoal.name.Split('_');
        currentGoalText.text = UppercaseFirst(goalname[2]) + ' ' + UppercaseFirst(goalname[1]);
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

    //Rajoute le goal dans la liste
    private void addGoal(string name, int x, int y){
        GameObject[] newListe = new GameObject[goals.Length+1]; //Crée une nouvelle liste 1 taille plus grande
        
        for (int i = 0 ; i<=goals.Length-1;i++)
        {
            newListe[i] = goals[i];

        }
        newListe[newListe.Length-1] = InstantiateGoal(name,x,y);
        goals=newListe;
    }
   
    

    //Différents getter et setter :

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

    public bool getSolverRunning(){
        return solverRunning;
    }

    
    public GameObject GetPosition(int x, int y)
    {
        return positions[x,y];
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

    //Met les position initiaux des robots à la liste de position positions
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

    //Remet le robot obj à sa position initial
    public void SetPositionDefaultRobot(GameObject obj)
    {
        RobotMan rm = obj.GetComponent<RobotMan>();

        SetPositionEmpty(rm.GetXBoard(),rm.GetYBoard());
        rm.SetXBoard(rm.GetXInit());
        rm.SetYBoard(rm.GetYInit());
        rm.SetCoords();

        SetPositionRobot(obj);
    }
    //Remet le robot obj à sa position dans le board initial
    public void SetPositionRobotInitial(GameObject obj)
    {
        RobotMan rm = obj.GetComponent<RobotMan>();
        if (positions[rm.GetXBoard(),rm.GetYBoard()]==null) //Vérifie si il n'y a pas un autre pion sur x,y
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

    public void SetSolverRunning(bool cond)
    {
        solverRunning = cond;
    }


}
