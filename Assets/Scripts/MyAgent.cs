using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using System.Threading.Tasks;


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
    List<GameObject> cars = new List<GameObject>();

    private int destination = 0;
    private int counter = 0;


    public override void OnEpisodeBegin()
    {

        Debug.Log("fut-episodebegin");
        
        //Visszateszi az autot a kezdo pozicioba
        transform.localPosition = new Vector3(-2f, -5.88f, -10f);
        transform.localRotation = Quaternion.Euler(0, 0, 0);


        //destination = Random.Range(0, 16);
        //Debug.Log(positions[destination]);
        destination = 6;

        

            // Kitorli az kocsikat az elozo episodbol
            foreach (var item in cars)
            {
                //Debug.Log(item.ToString());
                //Destroy(item);
            }
            

            for (int i = 0; i < 4; i++)
            {

                // Skips destination position
                if (i == destination)
                {
                    continue;
                }

                float carSpawn = Random.Range(0f, 1f);


                if (carSpawn < 0.5f && counter == 0)
                {
                    GameObject car = Instantiate(parkingCar, positions[i], Quaternion.Euler(0, 90, 0));
                    cars.Add(car);
                   
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
            AddReward(-1);
            EndEpisode();
        }

        if (transform.localPosition.z < -11)
        {
            Debug.Log("z out of bounds");
            AddReward(-10);
            EndEpisode();
        }
    }
    private void CheckForSuccessfulParking()
    {
        float marginOfErrorPosition = 1f;
        float marginOfErrorRotation = 15f;
        if (transform.localPosition.x < positions[destination][0] + marginOfErrorPosition && transform.localPosition.x > positions[destination][0] - marginOfErrorPosition
            && transform.localPosition.z < positions[destination][2] + marginOfErrorPosition && transform.localPosition.z > positions[destination][2] - marginOfErrorPosition
            && ((transform.localRotation.eulerAngles.y < 90 + marginOfErrorRotation && transform.localRotation.eulerAngles.y > 90 - marginOfErrorRotation) || (transform.localRotation.eulerAngles.y < 270 + marginOfErrorRotation && transform.localRotation.eulerAngles.y > 270 - marginOfErrorRotation))
             )
        {
            AddReward(1000);
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
            AddReward(-1);
            EndEpisode();
        }
        
    
    }
}
