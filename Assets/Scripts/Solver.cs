using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using static UnityEditor.PlayerSettings;
using System.IO;
using System.Threading.Tasks;

public class Solver : MonoBehaviour
{
    private GameObject currentRobot;

    private Game game;

    int seq=0;
    int len=0;
    IDictionary<(int,int), List<(int,int)>> posMap =new Dictionary<(int,int), List<(int,int)>>();
    List<int> finishMove = new List<int>();
    int finalSeq=-1;
    int finalLen=-1;


    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
    }

    public int getFinalLen() { 
        return finalLen;
    }

    public int getFinalSeq()
    {
        return finalSeq;
    }
    public void makeMove1(int val) {
        int pion=val/4;
        int dir=val%4;
        switch (dir){
            case 0:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(dir,0,1);
                break;
            case 1:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(dir,1,0);
                break;
            case 2:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(dir,0,-1);
                break;
            case 3:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(dir,-1,0);
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
        //printSeq(seq,len);
        int tmp=seq%16;
        int prec=(seq/16)%16;
        if (len==1){
            makeMove1(tmp%16);
            posMap[(seq,len)]=game.getPositionRobots();
        }
        else if (len!=0 && !(prec==tmp|| (prec/4==tmp/4 && tmp%4==((prec+2)%4 )))){ // le coup que l'on souhaite ajouté n'est pas le meme que le coup précédent
            try {
                game.setPositionRobot(posMap[(seq/16,len-1)]);
                List<(int,int)> pos =game.getPositionRobots();
                    //print(pos[0]+","+pos[1]+","+pos[2]+","+pos[3]);
                makeMove1(tmp);
                pos =game.getPositionRobots();
                    //print(pos[0]+","+pos[1]+","+pos[2]+","+pos[3]);
                posMap[(seq,len)]=game.getPositionRobots();
            }
            catch (KeyNotFoundException){}
        }
    }

    public int makeSeq31(int seq, int len){
        int tmp=seq%16;
        int prec=(seq/16)%16;
        if (len==1){
            makeMove1(tmp%16);
            posMap[(seq,len)]=game.getPositionRobots();
            return seq;
        }
        else if (len!=0 && !(prec==tmp|| (prec/4==tmp/4 && tmp%4==((prec+2)%4 )))){ // le coup que l'on souhaite ajouté n'est pas le meme ( ou l'opposé) que le coup précédent
            try {
                game.setPositionRobot(posMap[(seq/16,len-1)]);
                makeMove1(tmp);
                posMap[(seq,len)]=game.getPositionRobots();
                return seq;
            }
            catch (KeyNotFoundException){
                int i=2;
                tmp=seq/16;
                while (true){
                    tmp=tmp/16;
                    if (posMap.ContainsKey((tmp,len-i))){
                        return seq+(int)Math.Pow(16, i-1)-1;
                    }
                    i++;
                    if (i>len){
                        print("houla");
                    }
                } 
            }
        }
        return seq;
    }

     public int makeSeq4(int seq, int len){
        int tmp=seq%16;
        int prec=(seq/16)%16;
        int pion=tmp/4;
        int dir= tmp%4;
        if (len==1){
            makeMove1(tmp);
            posMap[(seq,len)]=game.getPositionRobots();
            return seq;
        }
        else if (len!=0 && !(prec==tmp|| (prec/4==tmp/4 && tmp%4==((prec+2)%4 )))){ // le coup que l'on souhaite ajouté n'est pas le meme ( ou l'opposé) que le coup précédent
            try {
                game.setPositionRobot(posMap[(seq/16,len-1)]);
                if (!(game.board.isWallInPos(game.getRobot(pion).GetComponent<RobotMan>().GetXBoard(),15-game.getRobot(pion).GetComponent<RobotMan>().GetYBoard(),dir))){
                    makeMove1(tmp);
                    posMap[(seq,len)]=game.getPositionRobots();
                    return seq;
                }
            }
            catch (KeyNotFoundException){
                int i=2;
                tmp=seq/16;
                while (true){
                    tmp=tmp/16;
                    if (posMap.ContainsKey((tmp,len-i))){
                        return seq+(int)Math.Pow(16, i-1)-1;
                    }
                    i++;
                    if (i>len){
                        print("houla");
                    }
                } 
            }
        }
        return seq;
    }

    public int makeSeq6(int seq, int len){
        int tmp=seq%16;
        int prec=(seq/16)%16;
        int pion=tmp/4;
        int dir= tmp%4;
        if (len==1){
            makeMove1(tmp);
            posMap[(seq,len)]=game.getPositionRobots();
            return seq;
        }
        else if (len!=0 && !(prec==tmp|| (prec/4==tmp/4 && tmp%4==((prec+2)%4 )))){ // le coup que l'on souhaite ajouté n'est pas le meme ( ou l'opposé) que le coup précédent
            try {
                game.setPositionRobot(posMap[(seq/16,len-1)]);
                if (!(game.board.isWallInPos(game.getRobot(pion).GetComponent<RobotMan>().GetXBoard(),15-game.getRobot(pion).GetComponent<RobotMan>().GetYBoard(),dir))&& !(game.isPionInDir(game.getRobot(pion).GetComponent<RobotMan>().GetXBoard(),game.getRobot(pion).GetComponent<RobotMan>().GetYBoard(),dir))){
                    makeMove1(tmp);
                    posMap[(seq,len)]=game.getPositionRobots();
                    return seq;
                }
            }
            catch (KeyNotFoundException){
                int i=2;
                tmp=seq/16;
                while (true){
                    tmp=tmp/16;
                    if (posMap.ContainsKey((tmp,len-i))){
                        return seq+(int)Math.Pow(16, i-1)-1;
                    }
                    i++;
                    if (i>len){
                        print("houla");
                    }
                } 
            }
        }
        return seq;
    }

    public (int,int) nextSeq(int seq,int len ){
        if (seq>=Math.Pow(16,len)-1){
            print("new len "+(len+1));
            return (0,len+1);
        }
        return (seq+1,len);
    }

    public String printSeq(int seq,int len){
        String result = "";
        for (int i=len-1;i>=0;i--){
            int a = seq/ (int)Math.Pow(16, i);
            seq = seq % (int)Math.Pow(16, i);
            int pion=a/4;
            int dir=a%4;
            switch (pion){
                case 0:
                result+="Bleu ";
                    break;
                case 2:
                result+="Vert ";
                    break;
                case 1:
                result+="Rouge ";
                    break;
                case 3:
                result+="Jaune ";
                    break;
            }
            switch (dir){
                case 1:
                result+="Droite\n";
                    break;
                case 3:
                result+="Gauche\n";
                    break;
                case 0:
                result+="Haut\n";
                    break;
                case 2:
                result+="Bas \n";
                    break;
            }
        }
        return result;
    }


    // Update is called once per frame
    void Update()
    {
        if (game.getSolverRunning()){
            bool stop=false;
            switch(game.dropdown.value){
                case 0:
                    game.SetSolverRunning(false);
                break;
                case 1:
                    finalSeq=-1;
                    finalLen=-1;
                    game.restartPosition();
                    makeSeq(seq,len);
                    if(game.hasWin(game.GetActiveRobot())){
                        printSeq(seq,len);
                        print(len);
                        finalSeq=seq;
                        finalLen=len;
                        game.restartPosition();
                        game.SetSolverRunning(false);
                    }
                    else{
                        (seq,len)=nextSeq(seq,len);
                        game.restartPosition();
                    }
                break;
                case 2 :
                    finalSeq=-1;
                    finalLen=-1;
                    currentRobot = game.GetActiveRobot();
                    game.restartPosition();
                    seq=makeSeq2(seq,len);
                    if(game.hasWin(currentRobot)){
                        printSeq(seq,len);
                        print(len);
                        finalSeq=seq;
                        finalLen=len;
                        game.restartPosition(); 
                        game.SetSolverRunning(false);
                    }
                    else{
                        (seq,len)=nextSeq(seq,len);
                        game.restartPosition();
                    }
                break;
                case 3:
                    finalSeq=-1;
                    finalLen=-1;
                    currentRobot = game.GetActiveRobot();
                    game.restartPosition();
                    makeSeq3(seq,len);
                    if(game.hasWin(currentRobot)){
                        printSeq(seq,len);
                        print(len);
                        finalSeq=seq;
                        finalLen=len;
                        game.restartPosition();
                        game.SetSolverRunning(false);
                    }
                    else{
                        (seq,len)=nextSeq(seq,len);
                        game.restartPosition();
                    }
                break;
                case 4:
                    finalSeq=-1;
                    finalLen=-1;
                    currentRobot = game.GetActiveRobot();
                    game.restartPosition();
                    seq=makeSeq31(seq,len);
                    if(game.hasWin(currentRobot)){
                        printSeq(seq,len);
                        print(len);
                        finalSeq=seq;
                        finalLen=len;
                        game.restartPosition();
                        game.SetSolverRunning(false);
                    }
                    
                    else{
                        (seq,len)=nextSeq(seq,len);
                        game.restartPosition();
                    }
                break;
                case 5:
                    finalSeq=-1;
                    finalLen=-1;
                    currentRobot = game.GetActiveRobot();
                    game.restartPosition();
                    seq=makeSeq4(seq,len);
                    if(game.hasWin(currentRobot)){
                        printSeq(seq,len);
                        print(len);
                        finalSeq=seq;
                        finalLen=len;
                        game.restartPosition();
                        game.SetSolverRunning(false);
                    }
                    
                    else{
                        (seq,len)=nextSeq(seq,len);
                        game.restartPosition();
                    }
                break;
                case 6:
                    finalSeq=-1;
                    finalLen=-1;
                    currentRobot = game.GetActiveRobot();
                    if (finishMove.Count==0){
                    // faire le calcul des dernier move
                        print("début");
                        for (int i=0;i<4;i++){

                            if (!(game.board.isWallInPos(game.GetCurrentGoal().GetComponent<GoalMan>().GetXBoard(),15-game.GetCurrentGoal().GetComponent<GoalMan>().GetYBoard(),i))){
                                finishMove.Add(game.GetCurrentGoal().GetComponent<GoalMan>().getColor()*4+((i+2)%4));
                            }
                        }
                        foreach(int a in finishMove){
                            game.restartPosition();
                            makeMove1(a);
                            if(game.hasWin(currentRobot)){
                                printSeq(a,1);
                                print(1);
                                finalSeq=a;
                                finalLen=1;
                                game.restartPosition();
                                game.SetSolverRunning(false);
                                stop =true;
                            }
                        }
                    }
                    if (!stop){
                        game.restartPosition();
                        seq=makeSeq4(seq,len);
                        //ajouté les mouv finaux a la fin
                        List<(int,int)> pos = game.getPositionRobots();
                        foreach(int a in finishMove){
                            game.setPositionRobot(pos);
                            makeMove1(a);
                            if(game.hasWin(currentRobot)){
                                printSeq(seq*16+a,len+1);
                                print(len+1);
                                finalSeq=seq*16+a;
                                finalLen=len+1;
                                game.restartPosition();
                                game.SetSolverRunning(false);
                                stop =true;
                                break;
                            }
                        }
                        if (!stop){
                            (seq,len)=nextSeq(seq,len);
                            game.restartPosition();
                        }
                    }
                break;
                case 7:
                    finalSeq=-1;
                    finalLen=-1;
                    currentRobot = game.GetActiveRobot();
                    if (finishMove.Count==0){
                    // faire le calcul des dernier move
                        print("début");
                        for (int i=0;i<4;i++){

                            if (!(game.board.isWallInPos(game.GetCurrentGoal().GetComponent<GoalMan>().GetXBoard(),15-game.GetCurrentGoal().GetComponent<GoalMan>().GetYBoard(),i))){
                                finishMove.Add(game.GetCurrentGoal().GetComponent<GoalMan>().getColor()*4+((i+2)%4));
                            }
                        }
                        foreach(int a in finishMove){
                            game.restartPosition();
                            makeMove1(a);
                            if(game.hasWin(currentRobot)){
                                printSeq(a,1);
                                print(1);
                                finalSeq=a;
                                finalLen=1;
                                game.restartPosition();
                                game.SetSolverRunning(false);
                                stop =true;
                            }
                        }
                    }
                    if (!stop){
                        game.restartPosition();
                        seq=makeSeq6(seq,len);
                        //ajouté les mouv finaux a la fin
                        List<(int,int)> pos = game.getPositionRobots();
                        foreach(int a in finishMove){
                            game.setPositionRobot(pos);
                            makeMove1(a);
                            if(game.hasWin(currentRobot)){
                                printSeq(seq*16+a,len+1);
                                print(len+1);
                                finalSeq=seq*16+a;
                                finalLen=len+1;
                                game.restartPosition();
                                game.SetSolverRunning(false);
                                stop =true;
                                break;
                            }
                        }
                        if (!stop){
                            (seq,len)=nextSeq(seq,len);
                            game.restartPosition();
                        }
                    }
                break;
            }
        }
        else{
            seq=0;
            len=0;
            posMap.Clear();
            finishMove.Clear();
        }
    }
}
