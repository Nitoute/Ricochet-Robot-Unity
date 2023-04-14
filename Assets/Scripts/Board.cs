using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Board{
    //Init of Wall Dictionary(Key, value) -> key should be transformed into Position instance in
    // later implementation.
    private IDictionary<(int i, int j),(int right, int top)> wallsDict = new Dictionary<(int i, int j),(int right, int top)>();
    private IDictionary<(int i, int j), int > goalsDict = new Dictionary<(int i, int j), int >();

    public Board(){
        //creates random board.
        assembleWallsBoards(0,0,0,0);
        assembleGoalsBoards(0,0,0,0);
    }
    /* Seeding works as following : corresponding number for whichever board it is affiliated to,
    * and for the flipped version, number+4. If 0, means it generates random.
    */
    public Board(int topleft, int topright, int bottomleft, int bottomright){
        //creates board with seed.
        assembleWallsBoards(topleft,topright,bottomleft,bottomright);
        assembleGoalsBoards(topleft,topright,bottomleft,bottomright);
    }
    /*Adds goals into board.*/
    private List<string> assembleBoards(int topleft, int topright, int bottomleft, int bottomright, string path){
        List<string> boardList = new List<string>();
        string topLeftFileName;
        string topRightFileName;
        string bottomLeftFileName;
        string bottomRightFileName;
        /*randomize*/
        if(topleft ==0 || topright == 0 || bottomleft ==0 || bottomright ==0){
            topLeftFileName = path + "top_left/board4_flip";
            topRightFileName = path + "top_right/board3_flip";
            bottomLeftFileName = path + "bottom_left/board1_flip";
            bottomRightFileName = path + "bottom_right/board2_flip";
        }
        else{
            if(topleft>4){
                Debug.Log("position TROUVZZZE");
                topLeftFileName = path + "top_left/board"+ (topleft-4).ToString()+"_flip";}
            else{topLeftFileName = path + "top_left/board"+ topleft.ToString();}
            if(topright>4){topRightFileName = path + "top_right/board"+ (topright-4).ToString()+"_flip";}
            else{topRightFileName = path + "top_right/board"+ topright.ToString();}
            if(bottomleft>4){bottomLeftFileName = path + "bottom_left/board"+ (bottomleft-4).ToString()+"_flip";}
            else{bottomLeftFileName = path + "bottom_left/board"+ bottomleft.ToString();}
            if(bottomright>4){bottomRightFileName = path + "bottom_right/board"+ (bottomright-4).ToString()+"_flip";}
            else{bottomRightFileName = path + "bottom_right/board"+ bottomright.ToString();}
        }
        boardList.Add(topLeftFileName);
        boardList.Add(topRightFileName);
        boardList.Add(bottomLeftFileName);
        boardList.Add(bottomRightFileName);
        return boardList;
    }

    private void assembleGoalsBoards(int topleft, int topright, int bottomleft, int bottomright){
        string path = "Assets/Scripts/goals/";
        List <string> boardList = assembleBoards(topleft, topright, bottomleft, bottomright, path);
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
    private void assembleWallsBoards(int topleft, int topright, int bottomleft, int bottomright){
        string path = "Assets/Scripts/walls_only/";
        List <string> boardList = assembleBoards(topleft, topright, bottomleft, bottomright, path);
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
    //Takes first and last value of file_str[i+8*j(checker dans cahier)], translates it to int tuple.
        //Debug.Log("position (" + i + "," + j + ") calculated " + position + " in board " + which_board);
        if (file_str[position].StartsWith("1")) // top wall : (,1)
        {
            if (file_str[position].EndsWith("1")) // left wall : (-1,)
            {
                //Debug.Log("position (" + i + "," + j + ") calculated " + position + " in board " + which_board + ", with walls: " + file_str[position]);
                return (-1,1);
            }
            else{
                //Debug.Log("position (" + i + "," + j + ") calculated " + position + " in board " + which_board + ", with walls: " + file_str[position]);
                return (0,1);
            }
        }
        else{
        if (file_str[position].EndsWith("1")) // mur à gauche : (-1,)
            {
                //Debug.Log("position (" + i + "," + j + ") calculated " + position + " in board " + which_board + ", with walls: " + file_str[position]);
                return (-1,0);
            }
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
        if (file_str[position].EndsWith("X")) // top wall : (,1)
        {
            color_c = file_str[position][4];
            switch(color_c){
                case 'A':
                    color = 0;
                break;
                case 'B':
                    color = 1;
                break;
                case 'G':
                    color = 2;
                break;
                case 'R':
                    color = 3;
                break;
                case 'Y':
                    color = 4;
                break;
            }
        }
        return color;
    }

    //TODO replace tuple (i,j) with class position
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

}