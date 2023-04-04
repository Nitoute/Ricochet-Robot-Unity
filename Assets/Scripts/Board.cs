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
        StreamReader topLeftRdr = new StreamReader(path + topLeftStr);
        StreamReader topRightRdr = new StreamReader(path + topRightStr);
        StreamReader bottomLeftRdr = new StreamReader(path + bottomLeftStr);
        StreamReader bottomRightRdr = new StreamReader(path + bottomRightStr);
        StreamReader sr = new StreamReader(dlg.FileName); 
        string[] topLeft = File.ReadAllText(topLeftFileName).Split(' ');
        printf("%s", topLeft);
    }

    /*readWalls reads at corresponding i,j coordinates of files and returns
    * corresponding walls.
    */
    private (int, int) readWalls(int i, int j, int position){
        //Updates
        if(i>=8){i-=8;}
        if(j>=8){j-=8;}

    };

    static void ReadString()
    {
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}