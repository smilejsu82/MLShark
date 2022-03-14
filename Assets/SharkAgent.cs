using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UI;

public class SharkAgent : Agent
{
    public Transform target;
    public float rotSpeed = 100f;
    public float moveSpeed = 15f;
    public GameObject plane;
    public Material[] arrMat;
    public Text txtMovement;

    private float movement;
    private float boundary;

    private bool isEnd;

    public override void OnEpisodeBegin()
    {
        this.isEnd = false;
        this.boundary = 5f;
        this.boundary *= this.plane.transform.localScale.x;

        //Debug.LogFormat("boundary: {0}", this.boundary);

        this.plane.GetComponent<MeshRenderer>().material = this.arrMat[0];

        this.transform.localPosition = new Vector3(0, 2.5f, 0);
        this.movement = 0;
        this.transform.rotation = Quaternion.Euler(Vector3.zero);


        // Move the target to a new spot
        Vector3 tPos = Vector3.zero;
        while (true) {
            tPos = new Vector3(Random.value * 8 - 4, Random.Range(0.5f, 5.5f), Random.value * 8 - 4);
            var dis = Vector3.Distance(tPos, this.transform.localPosition);
            if (dis > 2f) {
                break;
            }
        }
        this.target.localPosition = tPos;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(this.transform.localPosition);    //3
        //sensor.AddObservation(this.target.transform.localPosition); //3
        
        //sensor.AddObservation(this.transform.rotation); //x,y,z,w   (4)

        var distance = Vector3.Distance(this.target.localPosition, this.transform.localPosition);
        sensor.AddObservation(distance);    //1

        //sensor.AddObservation(this.transform.forward);  //3         //내가 바라보는 앞방향 
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var rotXDir = actions.DiscreteActions[0] - 1;       //0, 1, 2 -> -1, 0, 1
        var rotYDir = actions.DiscreteActions[1] - 1;       //0, 1, 2 -> -1, 0, 1
        //var rotZDir = actions.DiscreteActions[2] - 1;       //0, 1, 2 -> -1, 0, 1


        var v = actions.DiscreteActions[2]; //0, 1

        //rotate
        var rotationX = rotXDir * rotSpeed * Time.deltaTime;
        var rotationY = rotYDir * rotSpeed * Time.deltaTime;
        //var rotationZ = rotZDir * rotSpeed * Time.deltaTime;


        this.transform.Rotate(rotationX, rotationY, 0);

        //move
        this.movement = v * this.moveSpeed * Time.deltaTime;
        this.transform.Translate(0, 0, movement);


        AddReward(-1.0f / this.MaxStep);    //-0.0002

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);

        if (other.tag == "Fish") {
            AddReward(1.0f);
            EndEpisode();
        }
        else if (other.tag == "Wall") {
            AddReward(-0.1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionOut = actionsOut.DiscreteActions;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //-1, 0, 1 -> 0, 1, 2
        discreteActionOut[0] = (int)h + 1;

        //0, 1
        if (v >= 0) {
            discreteActionOut[1] = (int)v;
        }
        
    }
}
