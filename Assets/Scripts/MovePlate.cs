using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    // Board positions, not world positions
    int matrixX;
    int matrixY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<RobotMan>().GetXBoard(),reference.GetComponent<RobotMan>().GetYBoard());

        reference.GetComponent<RobotMan>().SetXBoard(matrixX);
        reference.GetComponent<RobotMan>().SetYBoard(matrixY);
        reference.GetComponent<RobotMan>().SetCoords();

        controller.GetComponent<Game>().SetPositionRobot(reference);

        reference.GetComponent<RobotMan>().DestroyMovePlates();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
