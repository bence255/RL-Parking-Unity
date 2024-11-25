using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;


public class MyAgent1 : Agent
{

    


    //Azert publicok mert a car controller igy tudja majd olvasni
    public float steering = 0;
    public float brake = 0;
    public float gas = 0;

    public Transform TargetTransform;


    


    //barmennyi lehet
    private int cars = 4;



    public override void OnEpisodeBegin()
    {


       

    }

    public List<Vector2> ParkingCarsCoords()
    {
        //visszaad egy listat amin Vector2() tipusu objektumok vannak. Ezek lesznek a parkolo
        //autokanka a koordinatai



        //Nem randomizalo szkriptet csinaltam... Szerintem nincs ra szukseg mert
        //osszesen csak 16 hely van es mindig meg kene nezni hogy a random generalt hely
        //nem-e foglalt mar. 

        List<Vector2> spaces = new List<Vector2>();

        // bal oldal
        spaces.Add(new Vector2(-4, 7));
        spaces.Add(new Vector2(-4, 5));
        spaces.Add(new Vector2(-4, 3));
        spaces.Add(new Vector2(-4, 1));
        spaces.Add(new Vector2(-4, -1));
        spaces.Add(new Vector2(-4, -3));
        spaces.Add(new Vector2(-4, -5));
        spaces.Add(new Vector2(-4, -7));

        // jobb oldal
        spaces.Add(new Vector2(4, 7));
        spaces.Add(new Vector2(4, 5));
        spaces.Add(new Vector2(4, 3));
        spaces.Add(new Vector2(4, 1));
        spaces.Add(new Vector2(4, -1));
        spaces.Add(new Vector2(4, -3));
        spaces.Add(new Vector2(4, -5));
        spaces.Add(new Vector2(4, -7));

        //majd kivalasztunk x darabot a listabol
        
        List<Vector2> ChosenCords = new List<Vector2>();
        for (int i = 0; i < cars; i++)
        {
            int index = UnityEngine.Random.Range(0, spaces.Count);
            ChosenCords.Add(spaces[index]);
            spaces.RemoveAt(index);

        }

        return ChosenCords;


    }

    public override void CollectObservations(VectorSensor sensor)
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
       
    }


    private void OnCollisionEnter(Collision collision)
    {
       
    }







}
