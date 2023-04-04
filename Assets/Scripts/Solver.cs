using System;
using UnityEngine;

public class Solver : MonoBehaviour
{
    public GameObject controller;
    private GameObject currentRobot;

    private Game game;

    int seq=0;
    int len=0;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
    }

    public void makeMove1(int val) {
        int pion=val/4;
        int dir=val%4;
        //print("pion="+pion+"  dir="+dir);
        switch (dir){
            case 0:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(1,0);
                break;
            case 2:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(-1,0);
                break;
            case 1:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(0,1);
                break;
            case 3:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(0,-1);
                break;
        }
    }

    public void makeSeq(int seq, int len){
        for (int i=len-1;i>=0;i--){
            int a = seq/ (int)Math.Pow(16, i);
            seq = seq % (int)Math.Pow(16, i);
            makeMove1(a);
        }
        //print ("\n\n\n");
    }

    public (int,int) nextSeq(int seq,int len ){
        if (seq==Math.Pow(16,len)-1){
            print("new len "+(len+1));
            return (0,len+1);            
        }
        return (seq+1,len);
    }

    // public void v1(){
    //     game.SetSolverRunning(true);
    //     GameObject currentRobot = game.GetCurrentRobotGoal();
    //     int seq=0;
    //     int len=0;
    //     while(game.getContinueSolve()){//faire detection win or not
    //         game.restartPosition();
    //         makeSeq(seq,len);
    //         if(game.hasWin(currentRobot)){
    //             print(seq);
    //             print(len);
    //             game.restartPosition();
    //             game.SetSolverRunning(false);
    //             //return (seq,len);
    //         }
    //         //yield return null;
    //         (seq,len)=nextSeq(seq,len);
    //     }
    //     game.restartPosition();
    //     game.SetSolverRunning(false);
    //     game.switchContinueSolve();
    // }


    // Update is called once per frame
    void Update()
    {
        if(game.getContinueSolveV1()){
            currentRobot = game.GetActiveRobot();
            game.restartPosition();
            makeSeq(seq,len);
            if(game.hasWin(currentRobot)){
                print(seq);
                print(len);
                game.restartPosition();
                game.switchContinueSolveV1();
                //return (seq,len);
            }
            //yield return null;
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();
        }
        else{
            seq=0;
            len=0;
        }
    }
}
