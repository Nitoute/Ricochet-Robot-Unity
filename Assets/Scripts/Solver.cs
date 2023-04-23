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

    private int seq=0;
    private int len=0;
    private IDictionary<(int,int), List<(int,int)>> posMap =new Dictionary<(int,int), List<(int,int)>>();
    private List<int> finishMove = new List<int>();
    private int finalSeq=-1;
    private int finalLen=-1;


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

    // joue le coup correspondant à val
    public void makeMove1(int val) {
        //pion correspond au premier chiffre de val considérer en base 4
        int pion=val/4;
        //dir correspond au second chiffre de val considérer en base 4
        int dir=val%4;
            //on utilise getRobot de game pour récupérer le pion correspondant à pion
            //on utilise MoveRobot de RobotMan pour déplacer le pion en fonction de Dir
        switch (dir){
            // bouger le pion vers le haut
            case 0:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(dir,0,1);
                break;
            // bouger le pion vers la droite
            case 1:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(dir,1,0);
                break;
            // bouger le pion vers le bas
            case 2:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(dir,0,-1);
                break;
            // bouger le pion vers la gauche
            case 3:
                game.getRobot(pion).GetComponent<RobotMan>().MoveRobot(dir,-1,0);
                break;
        }
    }
    //makeSeq interprete seq comme un entier hexadecimal de len chiffre
    public void makeSeq(int seq, int len){
        for (int i=len-1;i>=0;i--){
            //chaque coup est récupérer pour être jouer par makeMove1 
            int a = seq/ (int)Math.Pow(16, i);
            seq = seq % (int)Math.Pow(16, i);
            makeMove1(a);
        }
    }

    //makeSeq2 travaille comme makeSeq mais saute les séquences dans lesquels on trouve le meme coup ou le coup inverse à la suite
    public int makeSeq2(int seq, int len){
        int prec=-1;
        int tmp=seq;
        for (int i=len-1;i>=0;i--){
            int a = tmp/ (int)Math.Pow(16, i);
            tmp = tmp % (int)Math.Pow(16, i);
            // si le iième coup est le même ou l'inverse de son coup précédent alors ajoute à seq la valeur permettant de sauter les séquence jusqu'à changer le ième coup 
            if (prec!=-1 &&(a==prec || (a/4==prec/4 && a%4==(prec+2)%4 ))){
                //le -1 est présent pour considérer le NextSeq qui aura lieu pas la suite
                // il n'est pas utile de vérifié si on doit changé de len car NextSeq le fera  
                seq+=(int)Math.Pow(16, i)-1;
                return seq;
            }
            prec=a;
            makeMove1(a);
        }
        return seq;
    }
    //makeSeq3 interprete seq comme un entier hexadecimal de len chiffre et fait jouer le dernier mouv par makeMouv1 en repartant des positions donnée la séquence mère (seq privé de son dernier coup)    
    public void makeSeq3(int seq, int len){
        //dernier mouv de la séquence 
        int tmp=seq%16;
        //avant dernier mouv de la séquence 
        int prec=(seq/16)%16;
        // si len=1 alors il n'y a pas de séquence mère sur laquel se baser
        if (len==1){
            makeMove1(tmp%16);
            posMap[(seq,len)]=game.getPositionRobots();
        }
        // le coup que l'on souhaite ajouté n'est pas le meme ou l'inverse du coup précédent
        else if (len!=0 && !(prec==tmp|| (prec/4==tmp/4 && tmp%4==((prec+2)%4 )))){ 
            try {
                // si la séquence mère n'est pas dans le dictionnaire on tombera dans le catch 
                //sinon on se repositionne au position donnée par la séquence mère 
                game.setPositionRobot(posMap[(seq/16,len-1)]);
                //on joue le dernier coup
                makeMove1(tmp);
                //on met les position obtenue dans le dictionnaire
                posMap[(seq,len)]=game.getPositionRobots();
            }
            catch (KeyNotFoundException){}
        }
    }
//makeSeq31 travaille comme makeSeq3 mais saute les séquences pour lesquels on sait que l'on ne trouvera de séquence mère dans le dictionnaire
    public int makeSeq31(int seq, int len){
        int tmp=seq%16;
        int prec=(seq/16)%16;
        if (len==1){
            makeMove1(tmp%16);
            posMap[(seq,len)]=game.getPositionRobots();
            return seq;
        }
        else if (len!=0 && !(prec==tmp|| (prec/4==tmp/4 && tmp%4==((prec+2)%4 )))){ 
            try {
                game.setPositionRobot(posMap[(seq/16,len-1)]);
                makeMove1(tmp);
                posMap[(seq,len)]=game.getPositionRobots();
                return seq;
            }
            catch (KeyNotFoundException){
            //si l'on ne trouve pas de séquence mère alors on remonte dans l'arbre des séquence jusqu'à trouvé une séquence existante dans le dictionnaire
            //on saute alors les séquences pour lesquels on sait que l'on ne trouvera de séquence mèredans le dictionnaire
                int i=2;
                tmp=seq/16;
                while (true){
                    tmp=tmp/16;
                    if (posMap.ContainsKey((tmp,len-i))){
                //le -1 est présent pour considérer le NextSeq qui aura lieu pas la suite
                // il n'est pas utile de vérifié si on doit changé de len car NextSeq le fera  
                        return seq+(int)Math.Pow(16, i-1)-1;
                    }
                    i++;
                    if (i>len){
                        print("erreur");
                    }
                } 
            }
        }
        return seq;
    }

//makeSeq4 travaille comme makeSeq31 mais saute les séquences pour lesquels un coup envoie le pion dans une direction dans laquels se trouve un mur direct
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
                //si le coup envoie le pion dans une direction dans laquels se trouve un mur direct on ajoute pas la séquence dans le dictionnaire
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

//makeSeq6 travaille comme makeSeq4 mais saute les séquences pour lesquels un coup envoie le pion dans une direction dans laquelle se trouve un autre pion direct
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
                //si le coup envoie le pion dans une direction dans laquelle se trouve un mur ou un pion direct on ajoute pas la séquence dans le dictionnaire
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
//nextSeq retourne la séquence suivante 
    public (int,int) nextSeq(int seq,int len ){
        // Si seq est supérieur ou égale à la dernière séquence possible en len coup
        if (seq>=Math.Pow(16,len)-1){
            //alors on recommence à la séquence de base en incrémentant la len
            return (0,len+1);
        }
        //sinon on incrémente simplement seq
        return (seq+1,len);
    }

//printSeq écrire dans la console la sequence seq de longueur len
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


    // Update is called once per frame
    void Update()
    {
        // si un solveur doit tourner
        if (game.getSolverRunning()){
            bool stop=false;
            //en fonction du solver voulu on fait tourné le bon
            switch(game.dropdown.value){
                //si aucun solveur n'est sélectionné on désactive le fait que le solveur doit tourner
                case 0:
                    game.SetSolverRunning(false);
                break;
                case 1:
                    // on s'assure que la sequence final ( et sa len) soit bien remis à leur valeurs neutres
                    finalSeq=-1;
                    finalLen=-1;
                    //on remet les pions à leurs positions d'origine
                    game.restartPosition();
                    //on joue la séquence
                    makeSeq(seq,len);
                    //si les pions se trouve dans une situation gagnante
                    if(game.hasWin(game.GetActiveRobot())){
                        printSeq(seq,len);
                        print(len);
                        // on met à jour la séquence final
                        finalSeq=seq;
                        finalLen=len;
                        //on remet les pions à leurs positions d'origine par sécurité
                        game.restartPosition();
                        // on désactive le solveur
                        game.SetSolverRunning(false);
                    }
                    // sinon 
                    else{
                        // on récupère la séquence à étudier suivante et sa len
                        (seq,len)=nextSeq(seq,len);
                        //on remet les pions à leurs positions d'origine par sécurité
                        game.restartPosition();
                    }
                break;
                //le solveur 2 fonctionne comme le solveur 1 mais en utilisant MakeSeq2
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
                //le solveur 3 fonctionne comme le solveur 1 mais en utilisant MakeSeq3
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
                //le solveur 31 fonctionne comme le solveur 1 mais en utilisant MakeSeq31
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
                //le solveur 4 fonctionne comme le solveur 1 mais en utilisant MakeSeq4
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
                // le solveur 5 force le dernier mouvement en fonction des mouvements possibles pour atteinde le goal pour le pion de la bonne couleur
                case 6:
                    finalSeq=-1;
                    finalLen=-1;
                    currentRobot = game.GetActiveRobot();
                    //si aucun mouvement n'est présent dans la liste des mouvements finaux
                    if (finishMove.Count==0){
                    // faire le calcul des dernier move
                        for (int i=0;i<4;i++){
                            //pour les 4 direction, si il n'y à pas de mu autour du goal dans cette direction
                            if (!(game.board.isWallInPos(game.GetCurrentGoal().GetComponent<GoalMan>().GetXBoard(),15-game.GetCurrentGoal().GetComponent<GoalMan>().GetYBoard(),i))){
                                //alors on peut ajouter le mouvement pour le pion de la couleur du goal dans la direction inversé à la position du mur absent à la liste des mouvement finaux
                                finishMove.Add(game.GetCurrentGoal().GetComponent<GoalMan>().getColor()*4+((i+2)%4));
                            }
                        }
                        // on test si un des mouvement finaux seul suffit a gagner 
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
                                // ce booléen permet d'ignorer la suite du code si on à déja gagné 
                                stop =true;
                            }
                        }
                    }
                    if (!stop){
                        game.restartPosition();
                        //on joue la séquence grace a makeSeq4
                        seq=makeSeq4(seq,len);
                        // on récupère la position des pions
                        List<(int,int)> pos = game.getPositionRobots();
                        //pour chaque mouvement dans la liste des mouvement finaux
                        foreach(int a in finishMove){
                            //on remet la position des robots à leur positions à la fin de la séquence joué
                            game.setPositionRobot(pos);
                            //on joue le mouvement final
                            makeMove1(a);
                            //si les pions se trouve dans une situation gagnante
                            if(game.hasWin(currentRobot)){
                                printSeq(seq*16+a,len+1);
                                print(len+1);
                                // on met à jour la séquence final en ajoutant le mouv final à la séquence
                                finalSeq=seq*16+a;
                                finalLen=len+1;
                                game.restartPosition();
                                game.SetSolverRunning(false);
                                // ce booléen permet d'ignorer la suite du code si on à déja gagné 
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
                //le solveur 6 fonctionne comme le solveur 5 mais en utilisant MakeSeq6
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
        }// si aucun solveur ne doit tourner
        else{
            //alors on met la séquence courante au départ
            seq=0;
            len=0;
            //on vite le dictionnaire de l'arbre des séquence et les mouvements de fin
            posMap.Clear();
            finishMove.Clear();
        }
    }
}
