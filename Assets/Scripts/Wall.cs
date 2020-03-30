using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    //Debug
    //public List<string> adjList;
    //public List<int> x, y;
    //public List<Vector3> objs;
    //
    //[SerializeField]
    //int checks = 0;

    //Check if there is anything adjacent to the wall
    public int CheckAdjacent()
    {
        RaycastHit hitInfo;
        int adjCount = 0;

        //Iterate through 9 tiles around the wall
        for (int i = -1; i <= 1; i += 1)
        {
            for (int j = -1; j <= 1; j += 1)
            {
                //if (placement == false)
                //{
                //    checks++;
                //}

                //Ignore the tile the wall is currently on
                if (i == 0 && j == 0)
                {
                    continue;
                }
                else
                {
                    if (Physics.Raycast(transform.position, transform.TransformDirection(i, 0, j), out hitInfo, 2f))
                    {
                        //if (placement == false)
                        //{
                        //    adjList.Add(hitInfo.transform.gameObject.tag);
                        //    x.Add(i);
                        //    y.Add(j);
                        //    objs.Add(hitInfo.transform.position);
                        //}
                        //Debug.DrawRay(transform.position, transform.TransformDirection(i, 0, j), Color.green, 2f, false);

                        //Increase adjacent count if there is an adjacent wall
                        if (gameObject.tag == hitInfo.transform.gameObject.tag)
                        {
                            adjCount++;
                        }
                    }
                }
            }
        }
        return adjCount;
    }
}
