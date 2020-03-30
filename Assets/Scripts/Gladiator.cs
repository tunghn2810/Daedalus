using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gladiator : MonoBehaviour
{
    //Debug
    //public List<string> adjList;
    //public List<int> x, y;
    //public List<Vector3> objs;
    
    //Check if the gladiator is blocked in all 4 directions
    public int CheckBlock()
    {
        RaycastHit hitInfo;
        int blocked = 0;
        
        //Check right
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(1, 0, 0), out hitInfo, 2f))
        {
            if (hitInfo.transform.gameObject.tag != gameObject.tag)
            {
                //adjList.Add(hitInfo.transform.gameObject.tag);
                //x.Add(1);
                //y.Add(0);
                //objs.Add(hitInfo.transform.position);
                //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(1, 0, 0), Color.green, 2f, false);
                blocked++;
            }
        }

        //Check left
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(-1, 0, 0), out hitInfo, 2f))
        {
            if (hitInfo.transform.gameObject.tag != gameObject.tag)
            {
                //adjList.Add(hitInfo.transform.gameObject.tag);
                //x.Add(-1);
                //y.Add(0);
                //objs.Add(hitInfo.transform.position);
                //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(-1, 0, 0), Color.green, 2f, false);
                blocked++;
            }
        }

        //Check front
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(0, 0, 1), out hitInfo, 2f))
        {
            if (hitInfo.transform.gameObject.tag != gameObject.tag)
            {
                //adjList.Add(hitInfo.transform.gameObject.tag);
                //x.Add(0);
                //y.Add(1);
                //objs.Add(hitInfo.transform.position);
                //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(0, 0, 1), Color.green, 2f, false);
                blocked++;
            }
        }

        //Check behind
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(0, 0, -1), out hitInfo, 2f))
        {
            if (hitInfo.transform.gameObject.tag != gameObject.tag)
            {
                //adjList.Add(hitInfo.transform.gameObject.tag);
                //x.Add(0);
                //y.Add(-1);
                //objs.Add(hitInfo.transform.position);
                //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(0, 0, -1), Color.green, 2f, false);
                blocked++;
            }
        }

        return blocked;
    }
}
