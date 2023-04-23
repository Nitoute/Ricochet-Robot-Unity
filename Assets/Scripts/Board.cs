using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Random=System.Random;
using System.Linq;


public class Board{
    //Init of Wall Dictionary(Key, value) -> key should be transformed into Position instance in
    // later implementation.
    private List<string> boardList;
    private (int pos, int isFlipped) board3;
    private IDictionary<(int i, int j),(int right, int top)> wallsDict = new Dictionary<(int i, int j),(int right, int top)>();
    private IDictionary<(int i, int j), int > goalsDict = new Dictionary<(int i, int j), int >();
    private int TopLeft, TopRight, BottomLeft, BottomRight;

    public Board(){
        //creates random board.
        boardList = assembleBoards(0,0,0,0,"Assets/Scripts/goals/");
        assembleWallsBoards(0,0,0,0, boardList);
        assembleGoalsBoards(0,0,0,0, boardList);
    }
    /* Seeding works as following : corresponding number for whichever board it is affiliated to,
    * and for the flipped version, number+4. If 0, means it generates random.
    */
    public Board(int topleft, int topright, int bottomleft, int bottomright){
        //creates board with seed.
        boardList = assembleBoards(topleft,topright,bottomleft,bottomright,"Assets/Scripts/goals/");
        assembleWallsBoards(topleft,topright,bottomleft,bottomright, boardList);
        assembleGoalsBoards(topleft,topright,bottomleft,bottomright, boardList);
    }
    /*Adds goals into board.*/
    private List<string> assembleBoards(int topleft, int topright, int bottomleft, int bottomright, string path){
        List<string> boardList = new List<string>();
        string topLeftFileName;
        string topRightFileName;
        string bottomLeftFileName;
        string bottomRightFileName;
        board3 = (-1,-1);
        /*randomize*/
        if(topleft ==0 || topright == 0 || bottomleft ==0 || bottomright ==0){
            string flip;
            int[] numberBoards = new int[]{1,2,3,4};
            Random random = new Random();
            numberBoards = numberBoards.OrderBy(x => random.Next()).ToArray();
            Random rand = new Random();
            if(rand.Next(0, 2) == 0){flip = "_flip"; TopLeft = numberBoards[0] + 4;}
            else{ flip= ""; TopLeft = numberBoards[0];}
            if(numberBoards[0]==3){
                if(flip==""){
                    board3 = (0, 0); //soucis -> mur droite
                }
                else{
                    board3 = (0, 1); //soucis -> mur bas
                }
            }
            topLeftFileName = path + "top_left/board" + numberBoards[0] + flip;
            if(rand.Next(0, 2) == 0){flip = "_flip"; TopRight = numberBoards[1]+4;}
            else{flip= ""; TopRight = numberBoards[1];}
            if(numberBoards[1]==3 && flip==""){
                    board3 = (1, 0); // soucis -> mur bas
                }
            topRightFileName = path + "top_right/board" + numberBoards[1] + flip;
            if(rand.Next(0, 2) == 0){flip = "_flip"; BottomLeft = numberBoards[2] + 4;}
            else{ flip= ""; BottomLeft = numberBoards[2]; }
            if (numberBoards[2] == 3 && flip == "_flip") {
                board3 = (2, 1); //soucis -> mur droite
                }
            bottomLeftFileName = path + "bottom_left/board" + numberBoards[2] + flip;
            if(rand.Next(0, 2) == 0){flip = "_flip"; BottomRight = numberBoards[3] + 4;}
            else{ flip= ""; BottomRight = numberBoards[3]; }

            bottomRightFileName = path + "bottom_right/board" + numberBoards[3] + flip;
            }
        else{
            if(topleft==3){
                board3 = (0, 0);
            }
            else if(topleft==7){
                board3 = (0, 1);
            }
            else if(topright==3){
                board3 = (1, 0);
            }
            else if(bottomleft==7){
                board3 = (2, 1);
            }
            if(topleft>4){topLeftFileName = path + "top_left/board"+ (topleft-4).ToString()+"_flip";}
            else{topLeftFileName = path + "top_left/board"+ topleft.ToString();}
            if(topright>4){topRightFileName = path + "top_right/board"+ (topright-4).ToString()+"_flip";}
            else{topRightFileName = path + "top_right/board"+ topright.ToString();}
            if(bottomleft>4){bottomLeftFileName = path + "bottom_left/board"+ (bottomleft-4).ToString()+"_flip";}
            else{bottomLeftFileName = path + "bottom_left/board"+ bottomleft.ToString();}
            if(bottomright>4){bottomRightFileName = path + "bottom_right/board"+ (bottomright-4).ToString()+"_flip";}
            else{bottomRightFileName = path + "bottom_right/board"+ bottomright.ToString();}
            TopLeft = topleft; TopRight = topright; BottomLeft = bottomleft; BottomRight = bottomright;
        } 
        
        boardList.Add(topLeftFileName);
        boardList.Add(topRightFileName);
        boardList.Add(bottomLeftFileName);
        boardList.Add(bottomRightFileName);
        
        return boardList;
       
    }


    private void assembleGoalsBoards(int topleft, int topright, int bottomleft, int bottomright, List<string> boardListe){
        List <string> boardList = boardListe;
        string[] topLeft = File.ReadAllText(boardList[0]).Split(' ');
        string[] topRight = File.ReadAllText(boardList[1]).Split(' ');
        string[] bottomLeft = File.ReadAllText(boardList[2]).Split(' ');
        string[] bottomRight = File.ReadAllText(boardList[3]).Split(' ');
        //Adds goals to dictionary.
        int i, j;
        int temp_goal;
        for(j = 0; j<16; j++){
            for(i=0; i<16; i++){
                if(i<8){
                    if(j<8){
                        temp_goal = readGoals(i, j, topLeft);
                        if(temp_goal!=-1){addToGoalDict((i,j), temp_goal);}
                    }else{
                        temp_goal = readGoals(i, j, bottomLeft);
                        if(temp_goal!=-1){addToGoalDict((i,j), temp_goal);}
                    }
                }else{
                    if(j<8){
                        temp_goal = readGoals(i, j, topRight);
                        if(temp_goal!=-1){addToGoalDict((i,j), temp_goal);}
                    }else{
                        temp_goal = readGoals(i, j, bottomRight);
                        if(temp_goal!=-1){addToGoalDict((i,j), temp_goal);}
                    }
                }
            }
        }
    }

    ///assembly of boards if seed = 0, then random, else follow seed.
    private void assembleWallsBoards(int topleft, int topright, int bottomleft, int bottomright, List<string> boardListe){
        List <string> boardList = boardListe;
        string[] topLeft = File.ReadAllText(boardList[0]).Split(' ');
        string[] topRight = File.ReadAllText(boardList[1]).Split(' ');
        string[] bottomLeft = File.ReadAllText(boardList[2]).Split(' ');
        string[] bottomRight = File.ReadAllText(boardList[3]).Split(' ');
        //Adds left-top walls to dictionary.
        int i, j;
        (int,int) temp_wall;
        for(j = 0; j<16; j++){
            for(i=0; i<16; i++){
                if(i<8){
                    if(j<8){
                        temp_wall = readWalls(i, j, topLeft);
                        if(temp_wall!=(0,0)){addToWallDict((i,j), temp_wall);}
                    }else{
                        temp_wall = readWalls(i, j, bottomLeft);
                        if(temp_wall!=(0,0)){addToWallDict((i,j), temp_wall);}
                    }
                }else{
                    if(j<8){
                        temp_wall = readWalls(i, j, topRight);
                        if(temp_wall!=(0,0)){addToWallDict((i,j), temp_wall);}
                    }else{
                        temp_wall = readWalls(i, j, bottomRight);
                        if(temp_wall!=(0,0)){addToWallDict((i,j), temp_wall);}
                    }
                }
            }
        }
        wallsToComplete();
    }

    /*readWalls reads at corresponding i,j coordinates of files and returns
    * corresponding walls.
    * Walls are defined as following (Horizontal, Vertical)
        O -> None
        1 -> Top or Right
        -1 -> Bottom or Left
    * This function only reads top and left walls.
    * Returns None if there is No Wall top and left. There could be a right or bottom wall.
    * This function goes in pair with function that draws all right and all bottom walls of board,
    * called in Game.
    */
    private (int, int) readWalls(int i, int j, string[] file_str){
    //Updates
        string which_board ="";
        int x, y;
        if(i>=8){
            x=i-8;
            which_board += "right ";
        }
        else{
            x=i;
            which_board += "left ";
        }
        if(j>=8){
            y =j-8;
            which_board += "bottom";
        }
        else{
            y=j;
            which_board += "top";
        }
        int position = x+(8*y);
        string wall= file_str[position];
        //Takes first and last value of file_str[i+8*j(checker dans cahier)], translates it to int tuple.
        if (wall.EndsWith("M") || wall.EndsWith("P")|| wall.EndsWith("C") || wall.EndsWith("X") ){
            wall = wall.Remove(wall.Length-2);
        }
        if (wall.StartsWith("1")) // top wall : (,1)
        {
            if (wall.EndsWith("1")) // left wall : (-1,)
            {
                return (-1,1);
            }
            else{
                return (0,1);
            }
        }
        else if (wall.EndsWith("1")) // mur à gauche : (-1,)
                {
                    return (-1,0);
                }
        return (0,0);

    }
    private int readGoals(int i, int j, string[] file_str){
        int x, y;
        int color = -1;
        char color_c;
        if(i>=8){x=i-8;} else{x=i;}
        if(j>=8){y =j-8;} else{y=j;}
        int position = x+8*y;
        /**Takes last value of file_str[i+8*j(checker dans cahier)], translates it to int color.
        * respective colors correspondance : 0->4 : aleatoire (spirale de la grille 3), blue, green, red, yellow
        */
        if (file_str[position].EndsWith("M")) // top wall : (,1)
        {
            color_c = file_str[position][4];
            switch(color_c){
                case 'B':
                    color = 11;
                break;
                case 'G':
                    color = 12;
                break;
                case 'R':
                    color = 13;  
                break;
                case 'Y':
                    color = 14;  
                break;
            }
        }
        else if (file_str[position].EndsWith("P")) // top wall : (,1)
        {
            color_c = file_str[position][4];
            switch (color_c)
            {
                case 'B':
                    color = 21;
                    break;
                case 'G':
                    color = 22;
                    break;
                case 'R':
                    color = 23;
                    break;
                case 'Y':
                    color = 24;
                    break;
            }
        }
        else if (file_str[position].EndsWith("C")) // top wall : (,1)
        {
            color_c = file_str[position][4];
            switch (color_c)
            {
                case 'B':
                    color = 31;
                    break;
                case 'G':
                    color = 32;
                    break;
                case 'R':
                    color = 33;
                    break;
                case 'Y':
                    color = 34;
                    break;
            }
        }

        else if (file_str[position].EndsWith("X")) // top wall : (,1)
        {
            color_c = file_str[position][4];
            switch (color_c)
            {
                case 'A':
                    color = -1;
                    break;
                case 'B':
                    color = 41;
                    break;
                case 'G':
                    color = 42;
                    break;
                case 'R':
                    color = 43;  
                    break;
                case 'Y':
                    color = 44; 
                    break;
            }
        }
        return color;
    }

    private void addToWallDict((int, int) pos, (int, int) walls){
        wallsDict.Add((pos.Item1,pos.Item2),(walls.Item1, walls.Item2));
    }

    public IDictionary<(int i, int j),(int right, int top)> getWallDict(){
        return wallsDict;
    }

    private void addToGoalDict((int, int) pos, int color){
        goalsDict.Add((pos.Item1,pos.Item2), color);
    }

    public IDictionary<(int i, int j), int> getGoalDict(){
        return goalsDict;
    }


    private int wallsToComplete(){
        (int posX, int posY) position = (-1,-1);
        (int left, int top) wallsToAdd;
        if(board3.Item1 == 0){
            if(board3.Item2==0){
                position = (8,5);
                wallsToAdd = (-1,0);
            }
            else{
                position = (3,7);
                wallsToAdd = (0,-1);
            }
        }
        else if(board3.Item1 == 1){
            position = (10,8);
            wallsToAdd = (0,1);
        }
        else if(board3.Item1 == 2){
            position = (8,12);
            wallsToAdd = (-1,0);
        }
        else{
            return -1;
        }
        if(wallsDict.ContainsKey(position)){
            if(wallsDict[position].Item1==-1){
                if(wallsDict[position].Item2==0){
                    wallsDict[position] = (-1, wallsToAdd.Item2);
                }
            }
            else{
                if(wallsDict[position].Item2==1){
                    wallsDict[position] = (wallsToAdd.Item1, 1);
                }
                else{
                    wallsDict[position] = (wallsToAdd.Item1, wallsToAdd.Item2);
                }
            }
        }
        else{
            addToWallDict(position, wallsToAdd);
        }
        return 0;
    }
    /**
    * For a position, returns if there is a wall in corresponding direction (0->3 : top,right,bottom,left)
    */
    public bool isWallInPos(int x, int y, int dir)
    {
        try{
            if(dir==0){
                    if (wallsDict[(x,y)].Item2==1){
                        return true;
                    }
            }
            if(dir==1){
                    if (wallsDict[(x+1,y)].Item1==-1){
                        return true;
                    }
            }
            if(dir==2){
                    if (wallsDict[(x,y+1)].Item2==1){
                        return true;
                    }
            }
            if(dir==3){
                    if (wallsDict[(x,y)].Item1==-1){
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

    public int[] getSeed()
    {
        int[] seed = new int[4];
        seed[0] = TopLeft;
        seed[1] = TopRight;
        seed[2] = BottomLeft;
        seed[3] = BottomRight;
        return seed;
    }

}