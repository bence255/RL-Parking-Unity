using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;


public class MyAgent : Agent
{

    


    //Azert publicok mert a car controller igy tudja majd olvasni
    public float steering = 0;
    public float brake = 0;
    public float gas = 0;

    private GameObject parkingCar;
    public GameObject parkingCar0;
    public GameObject parkingCar1;
    public GameObject parkingCar2;
    public GameObject parkingCar3;
    public GameObject parkingCar4;

    public Vector3[] positions;
    


    public override void OnEpisodeBegin()
    {
        int destination = Random.Range(0, 16);

        for (int i = 0; i < 16; i++)
        {
            // Skips destination position
            if (i == destination)
            {
                continue;
            }

            float carSpawn = Random.Range(0f, 1f);

            // Randomize color
            if (carSpawn < 0.2f)
            {
                parkingCar = parkingCar0;
            }
            else if (carSpawn < 0.4f)
            {
                parkingCar = parkingCar1;
            }
            else if (carSpawn < 0.6f)
            {
                parkingCar = parkingCar2;
            }
            else if (carSpawn < 0.8f)
            {
                parkingCar = parkingCar3;
            }
            else
            {
                parkingCar = parkingCar4;
            }

            // Randomize which positions are occupied
            if (carSpawn < 0.25f)
            {
                Instantiate(parkingCar, positions[i], Quaternion.Euler(0,-90,0));
            }
            else if (carSpawn < 0.5f)
            {
                Instantiate(parkingCar, positions[i], Quaternion.Euler(0,90,0));
            }
            


        }
    }

    /*
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
    */

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
