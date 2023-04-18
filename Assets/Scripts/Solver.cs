using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

public class Solver : MonoBehaviour
{
    static Stopwatch stopwatch = new Stopwatch();

    public GameObject controller;
    private GameObject currentRobot;

    private Game game;

    int seq=0;
    int len=0;
    IDictionary<(int,int), List<(int,int)>> posMap =
            new Dictionary<(int,int), List<(int,int)>>();
    List<int> finishMove = new List<int>();

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
        int tmp=seq%16;
        int prec=(seq/16)%16;
        if (len==1){
            makeMove1(tmp%16);
            posMap[(seq,len)]=game.getPositionRobots();
        }
        else if (!(prec==tmp|| (prec/4==tmp/4 && tmp%4==((prec+2)%4 )))){ // le coup que l'on souhaite ajouté n'est pas le meme que le coup précédent
            try {
                game.setPositionRobot(posMap[(seq/16,len-1)]);
                makeMove1(tmp);
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
        else if (!(prec==tmp|| (prec/4==tmp/4 && tmp%4==((prec+2)%4 )))){ // le coup que l'on souhaite ajouté n'est pas le meme ( ou l'opposé) que le coup précédent
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
        else if (!(prec==tmp|| (prec/4==tmp/4 && tmp%4==((prec+2)%4 )))&& (game.board.isWallInPos(game.getRobot(pion).GetComponent<RobotMan>().GetXBoard(),game.getRobot(pion).GetComponent<RobotMan>().GetYBoard(),dir))){ // le coup que l'on souhaite ajouté n'est pas le meme ( ou l'opposé) que le coup précédent
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
                case 1:
                result+="Right";
                    break;
                case 3:
                result+="Left";
                    break;
                case 0:
                result+="Up";
                    break;
                case 2:
                result+="Down";
                    break;
            }
            result+=") ";
        }
        print(result);
    }

    // Créer le signal de démarrage
    public void sendSignalStart(int numero_Solver)
    {
        TimerCallback startCallback = new TimerCallback(StartTimer);
        Timer startTimer = new Timer(startCallback, null, 0, Timeout.Infinite);
        switch(numero_Solver)
        {
            
            case 1: game.switchContinueSolveV1(); break;
            case 2: game.switchContinueSolveV2(); break;
            case 3: game.switchContinueSolveV3(); break;
            case 31: game.switchContinueSolveV31(); break;
            case 4: game.switchContinueSolveV4();break;
            case 5: game.switchContinueSolveV5(); break;
        }
    }

    public void sendSignalStop()
    {
        TimerCallback stopCallback = new TimerCallback(StopTimer);
        Timer stopTimer = new Timer(stopCallback, null, Timeout.Infinite, Timeout.Infinite);
        stopTimer.Change(50, Timeout.Infinite);
    }

    static void StartTimer(object state)
    {
        //print("HEEEEYYYY");
        print("Début du chrono...");
        stopwatch.Reset();
        stopwatch.Start();
        
    }

    static void StopTimer(object state)
    {
        print("on arrête le chrono !");
        stopwatch.Stop();
        TimeSpan elapsedTime = stopwatch.Elapsed;
        print("Fin du chrono. Temps écoulé : " + elapsedTime.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if(game.getContinueSolveV1()){
            game.restartPosition();
            makeSeq(seq,len);
            //  print(game.GetActiveRobot());
            if(game.hasWin(game.GetActiveRobot())){
                printSeq(seq,len);
                print(len);
                sendSignalStop();
                game.restartPosition();
                game.switchContinueSolveV1();
                // Créer le signal d'arrêt
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
                sendSignalStop();
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
        else if(game.getContinueSolveV31()){
            currentRobot = game.GetActiveRobot();
            game.restartPosition();
            seq=makeSeq31(seq,len);
            if(game.hasWin(currentRobot)){
                printSeq(seq,len);
                print(len);
                sendSignalStop();
                game.restartPosition();
                game.switchContinueSolveV31();
                //return (seq,len);
            }
            //yield return null;
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();
        }
        else if(game.getContinueSolveV4()){
            currentRobot = game.GetActiveRobot();
            game.restartPosition();
            seq=makeSeq4(seq,len);
            if(game.hasWin(currentRobot)){
                printSeq(seq,len);
                print(len);
                game.restartPosition();
                game.switchContinueSolveV4();
                //return (seq,len);
            }
            //yield return null;
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();
        }
        else if(game.getContinueSolveV5()){
            currentRobot = game.GetActiveRobot();
            if (finishMove.Count==0){
// faire le calcul des dernier move
                print("début");
                        print(game.GetCurrentGoal().GetComponent<GoalMan>().getColor());
                        print(game.GetCurrentGoal().GetComponent<GoalMan>().GetXBoard()+""+game.GetCurrentGoal().GetComponent<GoalMan>().GetYBoard());
                for (int i=0;i<4;i++){
                    if (game.board.isWallInPos(game.GetCurrentGoal().GetComponent<GoalMan>().GetXBoard(),game.GetCurrentGoal().GetComponent<GoalMan>().GetYBoard(),i)){
                        finishMove.Add(game.GetCurrentGoal().GetComponent<GoalMan>().getColor()*4+i);
                        printSeq((game.GetCurrentGoal().GetComponent<GoalMan>().getColor()*4+i),i);
                    }
                }
                foreach(int a in finishMove){
                    game.restartPosition();
                    print(a);
                    makeMove1(a);
                    if(game.hasWin(currentRobot)){
                        printSeq(a,1);
                        print(1);
                        game.restartPosition();
                        game.switchContinueSolveV5();
                        //return (seq,len);
                    }
                }
            }


            game.restartPosition();
            seq=makeSeq4(seq,len);
            //ajouté les mouv finaux a la fin
            List<(int,int)> pos = game.getPositionRobots();
            foreach(int a in finishMove){
                game.setPositionRobot(pos);
                makeMove1(a);
                    printSeq(seq*16+a,len+1);
                if(game.hasWin(currentRobot)){
                    printSeq(seq*16+a,len+1);
                    print(len+1);
                    game.restartPosition();
                    game.switchContinueSolveV5();
                    //return (seq,len);
                    break;
                }
            }
            //yield return null;
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();
        }
        else{
            seq=0;
            len=0;
            posMap.Clear();
            finishMove.Clear();
        }
    }
}
