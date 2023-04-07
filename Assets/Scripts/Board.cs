using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Board : MonoBehaviour{
    //Init of Wall Dictionary(Key, value) -> key should be transformed into Position instance in
    // later implementation.
    private IDictionary<(int i, int j),(int right, int top)> wallsDict
                = new Dictionary<(int i, int j),(int right, int top)>();
    public Board(){
        //creates random board.
        assembleBoards(0,0,0,0);
    }
    /* Seeding works as following : corresponding number for whichever board it is affiliated to,
    * and for the flipped version, number+4. If 0, means it generates random.
    */
    public Board(int topleft, int topright, int bottomleft, int bottomright){
        //creates board with seed.
        assembleBoards(topleft,topright,bottomleft,bottomright);
    }
    ///assembly of boards if seed = 0, then random, else follow seed.
    private Board assembleBoards(int topleft, int topright, int bottomleft, int bottomright){
        string path = "walls_only/";
        string topLeftFileName;
        string topRightFileName;
        string bottomLeftFileName;
        string bottomRightFileName;
        if(topleft ==0 || topright == 0 || bottomleft ==0 || bottomright ==0){
            //generates random files
        }
        else{
            if(topleft>4){topLeftFileName = "top_left/board"+ (topleft-4).ToString()+"_flip";}
            else{topLeftFileName = "top_left/board"+ topleft.ToString();}
            if(topright>4){topRightFileName = "top_right/board"+ (topright-4).ToString()+"_flip";}
            else{topRightFileName = "top_right/board"+ topleft.ToString();}
            if(bottomleft>4){bottomLeftFileName = "bottom_left/board"+ (bottomleft-4).ToString()+"_flip";}
            else{bottomLeftFileName = "bottom_left/board"+ bottomleft.ToString();}
            if(bottomright>4){bottomRightFileName = "bottom_right/board"+ (bottomright-4).ToString()+"_flip";}
            else{bottomRightFileName = "bottom_right/board"+ bottomleft.ToString();}
        }
        //turns files into 1D String arrays, that are in order of position.
        string[] topLeft = File.ReadAllText(topLeftFileName).Split(' ');
        string[] topRight = File.ReadAllText(topRightFileName).Split(' ');
        string[] bottomLeft = File.ReadAllText(bottomLeftFileName).Split(' ');
        string[] bottomRight = File.ReadAllText(bottomRightFileName).Split(' ');

        //Adds walls to dictionary.
        int i, j;
        for(j = 0; j<16; j++){
            for(i=0; i<16; i++){
                if(i<8){
                    if(j<8){
                        addToWallDict((i,j), readWalls(i, j, topLeft));
                    }else{
                        addToWallDict((i,j), readWalls(i, j, bottomLeft));
                    }
                }else{
                    if(j<8){
                        addToWallDict((i,j), readWalls(i, j, topRight));
                    }else{
                        addToWallDict((i,j), readWalls(i, j, bottomRight));
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
        if(i>=8){int x=i-8;} else{x=i;}
        if(j>=8){int y =j-8;} else{y=j;}
        int position = i+8*j;
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
        return None;

    };
    //TODO replace tuple (i,j) with class position
        //euh private void addToWallDict((int, int) pos , (int, int) walls){
        //wallsDict.Add((pos.Item1,j),(sideWall, topWall));
    private void addToWallDict((int i, int j), (int sideWall, int topWall)){
        wallsDict.Add((i,j),(sideWall, topWall));
    }

    public getWallDict(){
        return wallsDict;
    }

}