using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using System.Threading.Tasks;
using System;


public class MyAgent : Agent
{
    //Azert publicok mert a car controller igy tudja majd olvasni
    public float steering = 0f;
    public float brakes = 0f;
    public float gas = 0f;

    public GameObject parkingCar;
    /*
    public GameObject parkingCar0;
    public GameObject parkingCar1;
    public GameObject parkingCar2;
    public GameObject parkingCar3;
    public GameObject parkingCar4;
    */

    public Vector3[] positions;
    //List<GameObject> cars = new List<GameObject>();
    private GameObject[] parkedCars = new GameObject[16];

    private int destination = 0;
    private int counter = 0;

    GameObject go;
    PrometeoCarController carController;


    public override void OnEpisodeBegin()
    {
        
        go = GameObject.Find("Car");
        carController = go.GetComponent<PrometeoCarController>();
        //Debug.Log("speed is: " + carController.localVelocityZ);

        Debug.Log("episode"+ counter);
        
        //Visszateszi az autot a kezdo pozicioba
        transform.localPosition = new Vector3(-2f, -5.88f, -10f);
        transform.localRotation = Quaternion.Euler(0, 0, 0);


        //destination = Random.Range(0, 16);
        //Debug.Log(positions[destination]);
        destination = 6;

            for (int i = 0; i < 16; i++)
            {
                bool carSpawn = false;
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f && i!=destination)
                {
                    carSpawn = true;
                    //Debug.Log(carSpawn);
                }
                // Kitorli a kocsit
                if (!carSpawn && parkedCars[i])
                {
                    //Debug.Log("Spawn a car");
                    Destroy(parkedCars[i]);
                }
                // Hozzadja a kocsit
                if (carSpawn && !parkedCars[i])
                {
                    //Debug.Log("Despawn a car");
                    GameObject car = Instantiate(parkingCar, positions[i], Quaternion.Euler(0, 90, 0));
                    parkedCars[i] = car;
                }
            }


        counter++;


        




    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Az agens pozicioja
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        // A target pozicioja
        sensor.AddObservation(positions[destination][0]);
        sensor.AddObservation(positions[destination][2]);

        //Az agens es a target tavolsaga
        float distance = Vector3.Distance(transform.localPosition, positions[destination]);
        sensor.AddObservation(distance);

        // Az agens iranya
        sensor.AddObservation(transform.localRotation.eulerAngles.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log("Paraszt debug");
        var actionTaken = actions.ContinuousActions;
        steering = actionTaken[0];
        //brakes = actionTaken[1];
        gas = actionTaken[1];

        //Debug.Log("steering: "+actionTaken[0]);
        //Debug.Log("gas: "+actionTaken[1]);

        float distance = Vector3.Distance(transform.localPosition, positions[destination]);
        float reward = 1/distance;
        reward = reward / 10;
        AddReward(reward);

        CheckForOutOfBounds();
        CheckForSuccessfulParking();

    }
    private void CheckForOutOfBounds()
    {
        if (transform.localPosition.y < -7)
        {
            Debug.Log("y out of bounds");
            AddReward(-3);
            EndEpisode();
        }

        if (transform.localPosition.z < -15)
        {
            Debug.Log("z out of bounds");
            AddReward(-3);
            EndEpisode();
        }

        if (transform.localPosition.x < -10 | transform.localPosition.x > 10)
        {
            Debug.Log("z out of bounds");
            AddReward(-3);
            EndEpisode();
        }


        if (transform.localPosition.z > 8)
        {
            Debug.Log("z out of bounds");
            AddReward(-3);
            EndEpisode();
        }

        if (transform.localPosition.y < -5.9)
        {
            Debug.Log("y out of bounds");
            AddReward(-3);
            EndEpisode();
        }
    }
    private void CheckForSuccessfulParking()
    {

        //&& ((transform.localRotation.eulerAngles.y < 90 + marginOfErrorRotation && transform.localRotation.eulerAngles.y > 90 - marginOfErrorRotation) || (transform.localRotation.eulerAngles.y < 270 + marginOfErrorRotation && transform.localRotation.eulerAngles.y > 270 - marginOfErrorRotation))
        float marginOfErrorPosition = 2f;
        float marginOfErrorRotation = 15f;
        if (transform.localPosition.x < positions[destination][0]-2.5f + marginOfErrorPosition && transform.localPosition.x > positions[destination][0]-2.5f - marginOfErrorPosition
            && transform.localPosition.z < positions[destination][2]-2f + marginOfErrorPosition && transform.localPosition.z > positions[destination][2]-2f - marginOfErrorPosition
            
            )
        {
            float slownessReward = 1 / (carController.localVelocityZ);
            AddReward(slownessReward);
            AddReward(10);
            EndEpisode();
        }
    }


    
    




    // Debug
    public override void Heuristic(in ActionBuffers actionsOut)
    {
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "ParkingCar") 
        {
            AddReward(-3);
            EndEpisode();
        }

        if (collision.collider.tag == "Barrier")
        {
            AddReward(-3);
            EndEpisode();
        }


    }
}
