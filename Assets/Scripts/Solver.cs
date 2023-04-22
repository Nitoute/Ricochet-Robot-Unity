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
    static Stopwatch stopwatch = new Stopwatch();
    private static AutoResetEvent AutoEvent = new AutoResetEvent(false); 

    public GameObject controller;
    private GameObject currentRobot;

    private Game game;

    int seq=0;
    int len=0;
    IDictionary<(int,int), List<(int,int)>> posMap =
            new Dictionary<(int,int), List<(int,int)>>();
    List<int> finishMove = new List<int>();
    int finalSeq=-1;
    int finalLen=-1;

    TimeSpan elapsedTime;

    bool timerFinished;

    //pour les seeds :
    List<int[]> seeds;
    int curSeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        seeds = InitSeeds();
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

     void StopTimer(object state)
    {
        print("on arrête le chrono !");
        stopwatch.Stop();
        elapsedTime = stopwatch.Elapsed;
        FileStream fappend = File.Open(@"C:\Users\Lenovo\Desktop\pdp\Ricochet-Robot-Unity\resultatV6.txt", FileMode.Append); // will append to end of file
        StreamWriter sw = new StreamWriter(fappend);
        sw.WriteLine("Plateau numéro : "+curSeed+ " Time : " + elapsedTime + ", nbMove : " + finalLen + ", Seq : " + finalSeq);
        print("Fin du chrono. Temps écoulé : " + elapsedTime.ToString());
        sw.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if(game.getContinueSolveV1()){
            finalSeq=-1;
            finalLen=-1;
            game.restartPosition();
            makeSeq(seq,len);
            //  print(game.GetActiveRobot());
            if(game.hasWin(game.GetActiveRobot())){
                printSeq(seq,len);
                print(len);
                finalSeq=seq;
                finalLen=len;
                sendSignalStop();
                timerFinished=true;
                game.restartPosition();
                game.switchContinueSolveV1();
                // Créer le signal d'arrêt
                //return (seq,len);
            }
            else{
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();}
        }
        else if(game.getContinueSolveV2()){
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
                sendSignalStop();
                timerFinished=true;
                game.restartPosition();
                game.switchContinueSolveV2();
                //return (seq,len);
            }
            
            else{
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();}
        }
        else if(game.getContinueSolveV3()){
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
                sendSignalStop();
                timerFinished=true;
                game.restartPosition();
                game.switchContinueSolveV3();
                //return (seq,len);
            }
            
            else{
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();}
        }
        else if(game.getContinueSolveV31()){
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
                sendSignalStop();
                timerFinished=true;
                game.restartPosition();
                game.switchContinueSolveV31();
                //return (seq,len);
            }
            
            else{
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();}
        }
        else if(game.getContinueSolveV4()){
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
                sendSignalStop();
                timerFinished=true;
                game.restartPosition();
                game.switchContinueSolveV4();
                //return (seq,len);
            }
            
            else{
            (seq,len)=nextSeq(seq,len);
            game.restartPosition();}
        }
        else if(game.getContinueSolveV5()){
            finalSeq=-1;
            finalLen=-1;
            bool stop=false;
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
                        sendSignalStop();
                        timerFinished=true;
                        game.restartPosition();
                        game.switchContinueSolveV5();
                        stop=true;
                        //return (seq,len);
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
                        sendSignalStop();   
                        timerFinished=true;
                        game.restartPosition();
                        game.switchContinueSolveV5();
                        stop=true;
                        break;
                    }
                }
                if (!stop){
                    (seq,len)=nextSeq(seq,len);
                    game.restartPosition();
                }
            }
        }
        else if(game.getContinueSolveV6()){
            finalSeq=-1;
            finalLen=-1;
            bool stop=false;
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
                        sendSignalStop();
                        timerFinished=true;
                        game.restartPosition();
                        game.switchContinueSolveV6();
                        stop=true;
                        //return (seq,len);
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
                        sendSignalStop();   
                        timerFinished=true;
                        game.restartPosition();
                        game.switchContinueSolveV6();
                        stop=true;
                        break;
                    }
                }
                if (!stop){
                    (seq,len)=nextSeq(seq,len);
                    game.restartPosition();
                }
            }
        }
        else{
            seq=0;
            len=0;
            posMap.Clear();
            finishMove.Clear();
        }
    }

    public async void WaitFor15Minutes()
    {
        try
        {
            await Task.Delay(TimeSpan.FromMinutes(1), new CancellationTokenSource().Token);
        }
        catch (TaskCanceledException)
        {
            timerFinished = false;
            sendSignalStop();
        }
    }

    public void PlaySeeds(int vSolveur)
    {
        List<int[]> seeds = InitSeeds();
        for (int i = 0; i < 2; i++) {
            timerFinished = false;
            //Changing world according to seed
            game.changeBoard( seeds[i][0], seeds[i][1], seeds[i][2], seeds[i][3]);
            
            //Reseting robots' positions
            List<(int, int)> positions = new List<(int, int)>() { (seeds[i][4], seeds[i][5]), (seeds[i][6], seeds[i][7]), (seeds[i][8], seeds[i][9]), (seeds[i][10], seeds[i][11])};
            game.SetPositionDefaultRobots(positions);
            sendSignalStart(vSolveur);
            WaitFor15Minutes();
            while (!timerFinished){}
            sendSignalStop();
            //sequence et longeur dans finalLen et finalSec
        }
    }

    
    public void changeSeed()
    {
        int len = seeds.Count;
        if (curSeed < len-1)
        {
            
            curSeed = curSeed +1;
        }else{
            curSeed = 0;
        }
        print(curSeed);
        print("("+seeds[curSeed][0]+","+seeds[curSeed][1]+","+seeds[curSeed][2]+","+seeds[curSeed][3]+","+seeds[curSeed][4]+","+seeds[curSeed][5]+","+seeds[curSeed][6]+","+seeds[curSeed][7]+","+seeds[curSeed][7]+","+seeds[curSeed][8]+","+seeds[curSeed][9]+","+seeds[curSeed][10]+","+seeds[curSeed][11]+")");
        game.changeBoard( seeds[curSeed][0], seeds[curSeed][1], seeds[curSeed][2], seeds[curSeed][3]);

        //Reseting robots' positions
        List<(int, int)> positions = new List<(int, int)>() { (seeds[curSeed][4], seeds[curSeed][5]), (seeds[curSeed][6], seeds[curSeed][7]), (seeds[curSeed][8], seeds[curSeed][9]), (seeds[curSeed][10], seeds[curSeed][11])};
        game.SetPositionDefaultRobots(positions);
    }

    private List<int[]> InitSeeds()
    {
        List<int[]> list = new List<int[]>();
        list.Add(new int[] { 8,2,5,7,0,14,15,0,0,0,13,4,2});
        list.Add(new int[] { 1,2,3,4,11,10,15,3,3,3,0,3,6});
        list.Add(new int[] { 4,3,6,5,8,1,7,1,1,15,2,6,4});
        list.Add(new int[] { 8,7,6,1,13,3,6,5,5,15,0,11,12});
        list.Add(new int[] { 7,2,5,4,0,14,0,12,12,12,15,5,6});
        list.Add(new int[] { 1,7,8,2,3,4,1,6,6,14,15,2,2});
        list.Add(new int[] {2,5,4,7,1,3,14,0,0,3,5,6,2});
        list.Add(new int[] { 7,4,2,5,5,14,15,7,7,14,0,6,4});
        list.Add(new int[] { 5,3,4,6,1,4,13,1,1,0,15,10,8});
        list.Add(new int[] { 2,3,4,1,13,1,9,10,10,11,7,9,9});
        return list;
    }

}
