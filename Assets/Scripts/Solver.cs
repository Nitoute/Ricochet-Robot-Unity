using System;
using UnityEngine;
using System.Collections.Generic;

public class Solver : MonoBehaviour
{
    public GameObject controller;
    private GameObject currentRobot;

    private Game game;

    int seq=0;
    int len=0;
    IDictionary<(int,int), List<(int,int)>> posMap =
            new Dictionary<(int,int), List<(int,int)>>();

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
    }

    public void makeMove1(int val) {
        int pion=val/4;
        int dir=val%4;
        switch (dir){
            case 0:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(0,1);
                break;
            case 1:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(1,0);
                break;
            case 2:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(0,-1);
                break;
            case 3:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(-1,0);
                break;
        }
    }

    public void makeSeq(int seq, int len){
        for (int i=len-1;i>=0;i--){
            int a = seq/ (int)Math.Pow(16, i);
            seq = seq % (int)Math.Pow(16, i);
            makeMove1(a);
        }
    }

    public int makeSeq2(int seq, int len){
        int prec=-1;
        int tmp=seq;
        for (int i=len-1;i>=0;i--){
            int a = tmp/ (int)Math.Pow(16, i);
            tmp = tmp % (int)Math.Pow(16, i);
            if (prec!=-1 &&(a==prec || (a/4==prec/4 && a%4==(prec+2)%4 ))){
                seq+=(int)Math.Pow(16, i)-1;
                return seq;
            }
            prec=a;
            makeMove1(a);
        }
        return seq;
    }


    public void makeSeq3(int seq, int len){
        int tmp=seq;
        if (len==1){
            makeMove1(tmp%16);
            posMap[(seq,len)]=game.getPositionRobot();
        }
        else if (!((tmp/16)%16==(tmp%16)|| ((tmp%16)/4==((tmp/16)%16)/4 && (tmp%16)%4==(((tmp/16)%16)+2)%4 ))){ // le coup que l'on souhaite ajouté n'est pas le meme que le coup précédent
            try {
                game.setPositionRobot(posMap[(tmp/16,len-1)]);
                makeMove1(tmp%16);
                posMap[(seq,len)]=game.getPositionRobot();
            }
            catch (KeyNotFoundException){}
        }
    }

    public (int,int) nextSeq(int seq,int len ){
        if (seq>=Math.Pow(16,len)-1){
            print("new len "+(len+1));
            return (0,len+1);            
        }
        return (seq+1,len);
    }

    public (int,int) nextSeq2(int seq,int len ){
        if (seq>=Math.Pow(16,len)-1){
        List<(int,int)> tmp=new List<(int,int)>();
            foreach (KeyValuePair<(int,int), List<(int,int)>> kvp in posMap){
                (int a,int b)=kvp.Key;
                if (a==len){
                    tmp.Add(kvp.Key);
                }
            }
            foreach ((int,int) a in tmp){
                posMap.Remove(a);
            }
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

    public void printSeq(int seq,int len){
        String result = "";
        for (int i=len-1;i>=0;i--){
            int a = seq/ (int)Math.Pow(16, i);
            seq = seq % (int)Math.Pow(16, i);
            int pion=a/4;
            int dir=a%4;
            //print("pion="+pion+"  dir="+dir);
            result+="(";
            switch (pion){
                case 0:
                result+="Blue ";
                    break;
                case 2:
                result+="Green ";
                    break;
                case 1:
                result+="Red ";
                    break;
                case 3:
                result+="Yellow ";
                    break;
            }
            switch (dir){
                case 0:
                result+="Right";
                    break;
                case 2:
                result+="Left";
                    break;
                case 1:
                result+="Up";
                    break;
                case 3:
                result+="Down";
                    break;
            }
            result+=") ";
        }
        print(result);
    }

    // Update is called once per frame
    void Update()
    {
        if(game.getContinueSolveV1()){
            currentRobot = game.GetActiveRobot();
            game.restartPosition();
            makeSeq(seq,len);
            if(game.hasWin(currentRobot)){
                printSeq(seq,len);
                print(len);
                game.restartPosition();
                game.switchContinueSolveV1();
                //return (seq,len);
            }
            //yield return null;
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();
        }
        else if(game.getContinueSolveV2()){
            currentRobot = game.GetActiveRobot();
            game.restartPosition();
            seq=makeSeq2(seq,len);
            if(game.hasWin(currentRobot)){
                printSeq(seq,len);
                print(len);
                game.restartPosition();
                game.switchContinueSolveV2();
                //return (seq,len);
            }
            //yield return null;
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();
        }
        else if(game.getContinueSolveV3()){
            currentRobot = game.GetActiveRobot();
            game.restartPosition();
            makeSeq3(seq,len);
            if(game.hasWin(currentRobot)){
                printSeq(seq,len);
                print(len);
                game.restartPosition();
                game.switchContinueSolveV3();
                //return (seq,len);
            }
            //yield return null;
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();
        }
        else{
            seq=0;
            len=0;
            posMap.Clear();
        }
    }
}
