/*using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;
using System.Diagnostics;
using System.Threading;


public class Timer : MonoBehaviour
{

   private static System.Timers.Timer aTimer;
   List<List<int>> seedList=new List<List<int>>();
   System.Random rand = new Random();
   
   void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
    }

    public void Activate()
    {
        for (int i=0;i<20;i++){
            List<int> tmp =new List<int>();
            for (int j =0 ;j<4;j++){
                if (rand.Next(0,2)==0){
                    tmp.Add(i);
                }
                else{
                    tmp.Add(i+4);
                }
            }
            tmp=ShuffleIntList(tmp);
            seedList[i]=new List<int>(tmp[0],tmp[1],tmp[2],tmp[3]);
        }
    }
    
    
    public static List<int> ShuffleIntList(List<int> list){
    var random = new System.Random();
    var newShuffledList = new List();
    var listcCount = list.Count;
    for (int i = 0; i < listcCount; i++)
    {
        var randomElementInList = random.Next(0, list.Count);
        newShuffledList.Add(list[randomElementInList]);
        list.Remove(list[randomElementInList]);
    }
    return newShuffledList;
}
    public void chronoV1(){
        StreamWriter sw = new StreamWriter("/result/resultatV1.txt");
        foreach(List<int> seed in seedList){

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            



            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            try
            {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);


            
                //Pass the filepath and filename to the StreamWriter Constructor
                //Write a line of text
                sw.WriteLine("Seed : "+ seed +"Time :" + elapsedTime );
                //Close the file
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
        sw.Close();
    }

    

}*/