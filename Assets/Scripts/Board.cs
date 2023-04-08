using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Board{
    //Init of Wall Dictionary(Key, value) -> key should be transformed into Position instance in
    // later implementation.
    private IDictionary<(int i, int j),(int right, int top)> wallsDict
                = new Dictionary<(int i, int j),(int right, int top)>();

    private IDictionary<(int i, int j), string color> goalsDict
                = new Dictionary<(int i, int j),string color>();

    public Board(){
        //creates random board.
        assembleBoards(2,7,1,8);
    }
    /* Seeding works as following : corresponding number for whichever board it is affiliated to,
    * and for the flipped version, number+4. If 0, means it generates random.
    */
    public Board(int topleft, int topright, int bottomleft, int bottomright){
        //creates board with seed.
        assembleBoards(topleft,topright,bottomleft,bottomright);
    }
    /*Adds goals into board.*/
    private void findGoals(){
        string path = "walls";
    }

    ///assembly of boards if seed = 0, then random, else follow seed.
    private void assembleBoards(int topleft, int topright, int bottomleft, int bottomright){
        string wall_path = "walls_only/";
        string goal_path = "goals/";
        string topLeftFileName;
        string topRightFileName;
        string bottomLeftFileName;
        string bottomRightFileName;
        if(topleft ==0 || topright == 0 || bottomleft ==0 || bottomright ==0){
            topLeftFileName = wall_path + "top_left/board4_flip";
            topRightFileName = wall_path + "top_right/board3_flip";
            bottomLeftFileName = wall_path + "bottom_left/board1_flip";
            bottomRightFileName = wall_path + "bottom_right/board2_flip";
        }
        else{
            if(topleft>4){topLeftFileName = wall_path + "top_left/board"+ (topleft-4).ToString()+"_flip";}
            else{topLeftFileName = wall_path + "top_left/board"+ topleft.ToString();}
            if(topright>4){topRightFileName = wall_path + "top_right/board"+ (topright-4).ToString()+"_flip";}
            else{topRightFileName = wall_path + "top_right/board"+ topleft.ToString();}
            if(bottomleft>4){bottomLeftFileName = wall_path + "bottom_left/board"+ (bottomleft-4).ToString()+"_flip";}
            else{bottomLeftFileName = wall_path + "bottom_left/board"+ bottomleft.ToString();}
            if(bottomright>4){bottomRightFileName = wall_path + "bottom_right/board"+ (bottomright-4).ToString()+"_flip";}
            else{bottomRightFileName = wall_path + "bottom_right/board"+ bottomleft.ToString();}
        }
        //turns files into 1D String arrays, that are in order of position.
        string[] topLeft = File.ReadAllText(topLeftFileName).Split(' ');
        string[] topRight = File.ReadAllText(topRightFileName).Split(' ');
        string[] bottomLeft = File.ReadAllText(bottomLeftFileName).Split(' ');
        string[] bottomRight = File.ReadAllText(bottomRightFileName).Split(' ');

        //Adds walls to dictionary.
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
    * This function goes in pair with function that draws all right and all bottom walls of board.
    */
    private (int, int) readWalls(int i, int j, string[] file_str){
    //Updates
        int x, y;
        if(i>=8){x=i-8;} else{x=i;}
        if(j>=8){y =j-8;} else{y=j;}
        int position = x+8*y;
    //Takes first and last value of file_str[i+8*j(checker dans cahier)], translates it to int tuple.
        if (file_str[x+8*y].StartsWith("1")) // top wall : (,1)
        {
            if (file_str[x+8*y].EndsWith("1")) // left wall : (-1,) -> TODO delete goals from files
            {
                return (-1,1);
            }
            else{
                return (0,1);
            }
        }
        else{
        if (file_str[i+8*j].EndsWith("1")) // mur à gauche : (-1,)
            {
                return (-1,0);
            }
        }
        return (0,0);

    }
    //TODO replace tuple (i,j) with class position
    private void addToWallDict((int, int) pos, (int, int) walls){
        wallsDict.Add((pos.Item1,pos.Item2),(walls.Item1, walls.Item2));
    }

    public IDictionary<(int i, int j),(int right, int top)> getWallDict(){
        return wallsDict;
    }

}