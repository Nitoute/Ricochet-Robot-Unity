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
        FileStream fappend = File.Open(@"C:\Users\Lenovo\Desktop\pdp\Ricochet-Robot-Unity\resultatV4.txt", FileMode.Append); // will append to end of file
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

    public void test()
    {
        String fn = @"C:\Users\Lenovo\Desktop\pdp\Ricochet-Robot-Unity\Assets\Scripts/goals/resultatV.txt";
        StreamWriter sw = new StreamWriter(fn);
        sw.WriteLine("Seed");
        sw.WriteLine("seed2");
        sw.Close();
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
        //Supprimez les seeds qui vous concernent pas, gardez que les 10 seeds qui sont à vous
        /*//Moad
        list.Add(new int[] { 1, 3, 2, 8, 5, 4, 14, 11, 9, 7, 6, 8 });
        list.Add(new int[] { 6, 5, 7, 8, 13, 5, 14, 5, 6, 3, 10, 12 });
        list.Add(new int[] { 1, 2, 3, 8, 7, 2, 11, 8, 7, 6, 4, 9});
        list.Add(new int[] { 5, 8, 6, 7, 4, 11, 11, 13, 10, 12, 6, 7});
        list.Add(new int[] { 3, 5, 4, 2, 8, 14, 7, 5, 3, 11, 12, 6});
        list.Add(new int[] { 4, 2, 7, 1, 3, 10, 11, 7, 11, 10, 2, 4});
        list.Add(new int[] { 1, 2, 8, 7, 5, 2, 3, 5, 6, 5, 13, 7});
        list.Add(new int[] { 6, 1, 3, 8, 1, 15, 11, 8, 7, 0, 6, 2});
        list.Add(new int[] { 4, 3, 5, 2, 8, 12, 4, 7, 15, 4, 5, 13});
        list.Add(new int[] { 2, 8, 1, 3, 7, 15, 12, 0, 5, 0, 11, 5});
        //Moad (2)
        list.Add(new int[] { 2, 4, 1, 7, 0, 10, 6, 3, 12, 2, 5, 9});
        list.Add(new int[] { 1, 2, 7, 4, 0, 13, 11, 5, 9, 5, 4, 2});
        list.Add(new int[] { 4, 7, 2, 5, 2, 6, 3, 4, 9, 12, 9, 6});
        list.Add(new int[] { 1, 8, 2, 3, 3, 6, 13, 9, 13, 12, 2, 9});
        list.Add(new int[] { 1, 7, 2, 4, 12, 13, 9, 12, 12, 5, 12, 7});
        list.Add(new int[] { 8, 6, 7, 5, 3, 1, 2, 11, 2, 14, 3, 6});
        list.Add(new int[] { 2, 3, 5, 8, 2, 5, 13, 0, 9, 10, 13, 11});
        list.Add(new int[] { 8, 7, 2, 5, 5, 14, 7, 8, 10, 12, 5, 3});
        list.Add(new int[] { 3, 2, 5, 4, 13, 10, 6, 2, 11, 6, 5, 15});
        list.Add(new int[] { 4, 6, 7, 5, 8, 12, 8, 14, 11, 3, 1, 0});
        //Hamza
        list.Add(new int[] { 6, 4, 1, 3, 0, 9, 12, 13, 9, 15, 4, 11});
        list.Add(new int[] { 5, 4, 7, 2, 0, 10, 12, 10, 8, 8, 8, 6});
        list.Add(new int[] { 1, 8, 2, 7, 5, 5, 7, 15, 2, 14, 5, 11});
        list.Add(new int[] { 3, 2, 1, 4, 11, 15, 13, 11, 2, 13, 11, 2});
        list.Add(new int[] { 3, 5, 2, 8, 2, 15, 6, 12, 8, 5, 11, 11});
        list.Add(new int[] { 8, 2, 7, 5, 4, 0, 1, 1, 14, 5, 0, 14});
        list.Add(new int[] { 5, 4, 2, 3, 11, 3, 6, 0, 13, 6, 1, 3});
        list.Add(new int[] { 6, 8, 7, 1, 1, 2, 12, 2, 12, 0, 15, 11});
        list.Add(new int[] { 2, 1, 3, 8, 5, 11, 4, 4, 10, 10, 2, 1});
        list.Add(new int[] { 2, 8, 1, 7, 7, 11, 0, 10, 15, 3, 10, 6});*/
        //Vincent
        list.Add(new int[] { 1, 6, 3, 4, 11, 10, 0, 3, 6, 1, 3, 15});
        list.Add(new int[] { 8, 2, 5, 7, 0, 14, 15, 0, 0, 13, 4, 2});
        list.Add(new int[] { 8, 3, 6, 5, 1, 5, 3, 1, 3, 0, 11, 2 });
        list.Add(new int[] { 2, 8, 7, 5, 0, 7, 13, 5, 13, 8, 0, 1});
        list.Add(new int[] { 5, 6, 4, 7, 9, 14, 0, 2, 14, 11, 3, 1});
        list.Add(new int[] { 5, 4, 6, 3, 12, 6, 0, 0, 1, 6, 15, 3});
        list.Add(new int[] { 2, 4, 5, 3, 4, 13, 2, 0, 2, 13, 15, 0});
        list.Add(new int[] { 1, 2, 3, 4, 11, 10, 15, 3, 3, 0, 3, 6});
        list.Add(new int[] { 4, 3, 6, 5, 8, 1, 7, 1, 15, 2, 6, 4});
        list.Add(new int[] { 6, 1, 8, 7, 2, 0, 7, 1, 3, 2, 6, 11});/*
        //Radja
        list.Add(new int[] { 5, 2, 7, 8, 7, 6, 1, 14, 0, 15, 4, 15});
        list.Add(new int[] { 2, 4, 1, 3, 13, 7, 7, 14, 5, 1, 13, 2});
        list.Add(new int[] { 6, 4, 3, 1, 7, 12, 14, 10, 2, 12, 3, 3});
        list.Add(new int[] { 8, 3, 6, 5, 2, 12, 2, 1, 6, 9, 5, 2});
        list.Add(new int[] { 1, 4, 2, 7, 8, 7, 10, 1, 15, 10, 7, 0});
        list.Add(new int[] { 2, 1, 3, 4, 6, 1, 9, 9, 5, 9, 14, 2});
        list.Add(new int[] { 4, 2, 5, 7, 7, 7, 10, 9, 13, 10, 7, 1});
        list.Add(new int[] { 2, 1, 4, 7, 12, 13, 3, 15, 5, 8, 4, 3});
        list.Add(new int[] { 2, 3, 4, 1, 13, 1, 9, 10, 11, 7, 9, 9});
        list.Add(new int[] { 1, 2, 4, 7, 0, 6, 4, 8, 11, 13, 14, 2});*/
        list.Add(new int[] {  8, 7, 6, 1, 13, 3, 6, 5, 15, 0, 11, 12});
        list.Add(new int[] {  7, 2, 5, 4, 0, 14, 0, 12, 12, 15, 5, 6});
        list.Add(new int[] {  2, 3, 1, 4, 0, 3, 1, 3, 15, 8, 1, 12});
        list.Add(new int[] {  1, 7, 8, 2, 3, 4, 1, 6, 14, 15, 2, 2});
        list.Add(new int[] {  4, 2, 5, 7, 12, 7, 0, 1, 11, 8, 1, 8});
        list.Add(new int[] {  5, 4, 7, 6, 2, 7, 2, 3, 2, 7, 2, 0});
        list.Add(new int[] {  2, 5, 4, 7, 1, 3, 14, 0, 3, 5, 6, 2});
        list.Add(new int[] {  5, 7, 4, 2, 5, 8, 10, 8, 5, 2, 11, 14});
        list.Add(new int[] {  4, 2, 1, 3, 14, 6, 12, 12, 6, 11, 15, 12});
        list.Add(new int[] {  3, 1, 8, 6, 1, 0, 4, 6, 11, 11, 15, 15});
        list.Add(new int[] {  3, 4, 2, 5, 9, 10, 14, 3, 4, 5, 10, 6});
        list.Add(new int[] {  5, 6, 3, 8, 7, 2, 2, 7, 11, 14, 12, 8});
        list.Add(new int[] {  1, 4, 2, 3, 13, 8, 9, 4, 12, 5, 11, 1});
        list.Add(new int[] {  7, 4, 2, 5, 5, 14, 15, 7, 14, 0, 6, 4});
        list.Add(new int[] {  5, 7, 4, 2, 6, 11, 11, 8, 6, 8, 12, 9});
        list.Add(new int[] {  2, 5, 7, 4, 9, 9, 13, 13, 13, 11, 14, 7});
        list.Add(new int[] {  6, 3, 8, 5, 2, 4, 12, 13, 8, 15, 7, 2});
        list.Add(new int[] {  5, 3, 4, 6, 1, 4, 13, 1, 0, 15, 10, 8});
        list.Add(new int[] {  6, 3, 5, 8, 12, 13, 11, 1, 9, 4, 10, 0});
        list.Add(new int[] {  4, 5, 6, 3, 15, 11, 14, 2, 11, 6, 11, 14});
        list.Add(new int[] { 8,2,1,3,4,3,3,0,13,11,0,0});
        return list;
    }

}
