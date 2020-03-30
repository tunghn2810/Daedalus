using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    //Materials
    public Material defaultMat;
    public Material greenOutline;
    public Material redOutline;

    //Public attributes
    public bool isHovered;
    public bool isClicked;

    //Attributes
    Renderer objRenderer;
    Renderer[] partsRenderer;
    Collider coll;

    bool hasRenderer;

    void Start()
    {
        //If the gameobject itself has a renderer
        if (gameObject.GetComponent<Renderer>() != null)
        {
            objRenderer = gameObject.GetComponent<Renderer>();
            hasRenderer = true;
        }
        //If the child gameobjects have a renderer
        else
        {
            partsRenderer = gameObject.GetComponentsInChildren<Renderer>();
            hasRenderer = false;
        }
        
        coll = gameObject.GetComponent<Collider>();
    }

    void Update()
    {
        if (hasRenderer)
            MaterialUpdate(objRenderer);
        else
            MaterialUpdate(partsRenderer);

        //Collider issue (minor)
        //if (GameManager.gameManager.isWhiteTurn == true)
        //{
        //    if (gameObject.tag == "WhiteGlad" || gameObject.tag == "WhiteMino")
        //    {
        //        coll.enabled = true;
        //    }
        //    else if (gameObject.tag == "BlackGlad" || gameObject.tag == "BlackMino")
        //    {
        //        coll.enabled = false;
        //    }
        //}
        //else if (GameManager.gameManager.isWhiteTurn == false)
        //{
        //    if (gameObject.tag == "WhiteGlad" || gameObject.tag == "WhiteMino")
        //    {
        //        coll.enabled = false;
        //    }
        //    else if (gameObject.tag == "BlackGlad" || gameObject.tag == "BlackMino")
        //    {
        //        coll.enabled = true;
        //    }
        //}
    }

    //Update the material when the gameobject is hovered or clicked
    void MaterialUpdate(Renderer obj)
    {
        obj.material = defaultMat;

        if (isHovered)
        {
            obj.material = redOutline;
        }

        if (isClicked)
        {
            obj.material = greenOutline;
        }
    }

    //Update the material of each child in the gameobject
    void MaterialUpdate(Renderer[] list)
    {
        foreach (Renderer ren in partsRenderer)
        {
            MaterialUpdate(ren);
        }
    }
}
