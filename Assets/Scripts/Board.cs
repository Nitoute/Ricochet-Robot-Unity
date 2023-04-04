using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Board : MonoBehaviour{
    public Board(){
        //creates random board.
    }
    public Board(int seed){
        //creates board with seed.
    }
    ///random assembly of boards.
    private Board assembleBoards(){
        //reads from files
        //assembles boards
        string path = Application.persistentDataPath + "boards/";
        string topLeftStr = "board3";
        string topRightStr = "board4_flip";
        string bottomLeftStr = "board1";
        string bottomRightStr = "board2_flip";
        StreamReader topLeftRdr = new StreamReader(path + topLeftStr);
        StreamReader topRightRdr = new StreamReader(path + topRightStr);
        StreamReader bottomLeftRdr = new StreamReader(path + bottomLeftStr);
        StreamReader bottomRightRdr = new StreamReader(path + bottomRightStr);
        /*turns board into two arrays : vertical_walls and horizontal_walls,
        * which help add unity wall GameObjects*, Same loop twice except
        * it adds in either verticalWalls array or HorizontalWalls*/
        //WALLS :
            //for loop, reads from j < 8 top, AND i < 8 left or 8 <= i < 16  right
            //     from j >= 8 bottom, AND i < 8 left or 8 <= i < 16  right

            //Reader : reads separated by space, does not look into goals yet.

    }

    static void ReadString()
    {
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}