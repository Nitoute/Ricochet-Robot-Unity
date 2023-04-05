using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Board : MonoBehaviour{
    private wall_list;
    private IDictionary<int, string> numberNames = new Dictionary<int, string>();
    public Board(){
        assembleBoards();
    }
    public Board(int seed){
        //creates board with seed.
    }
    ///assembly of boards if seed = 0, then random, else follow seed.
    private Board assembleBoards(){
        //reads from files
        //assembles boards
        string path = "boards/";
        string topLeftFileName = "top_left/board3";
        string topRightFileName = "top_right/board4_flip";
        string bottomLeftFileName = "bottom_left/board1";
        string bottomRightFileName = "bottom_right/board2_flip";
        string[] topLeft = File.ReadAllText(topLeftFileName).Split(' ');
        string[] topRight = File.ReadAllText(topRightFileName).Split(' ');
        string[] bottomLeft = File.ReadAllText(bottomLeftFileName).Split(' ');
        string[] bottomRight = File.ReadAllText(bottomRightFileName).Split(' ');

        //Reads files.
        int i, j;
        for(j = 0; j<16; j++){
            for(i=0; i<16; i++){
                if(i<8){
                    if(j<8){
                        //topleft
                    }else{
                        //bottomleft
                    }
                }else{
                    if(j<8){
                        //topright
                    }else{
                        //bottomright
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
    //prends la première et dernière valeur de file_str[i+8*j(checker dans cahier)], transforme en int
        if (file_str[i+8*j].StartsWith("1")) // mur en haut : (,1)
        {
            if (file_str[i+8*j].EndsWith("1")) // mur à gauche : (-1,)
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

    static void ReadString()
    {
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}