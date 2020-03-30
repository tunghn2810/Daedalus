using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    //References
    GameObject prevObj; //Previously hovered object
    GameObject currentObj; //Hovered object

    GameObject selectedObj; //Currently selected object
    GameObject unselectedObj; //Just unselected object
    Vector3 backPos; //Undo position

    //Checks
    bool doWallCheck;
    bool doGladCheck;
    bool doMinoCheck;

    public static SelectionManager selectionManager { get; set; }
    void Awake()
    {
        if (selectionManager == null)
        {
            selectionManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        //Clear previous object reference
        if (prevObj != null && prevObj.tag != "InviWall")
        {
            prevObj.GetComponent<Selectable>().isHovered = false;
            prevObj = null;
        }
        
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Check where the mouse is
        if (Physics.Raycast(ray, out hitInfo, 100f))
        {
            currentObj = hitInfo.transform.gameObject;

            if (currentObj != null)
            {
                //If the game is in setup state
                if (GameManager.gameManager.stateNum == 0)
                {
                    HoverCheck("ChessTile", false); //Chess tiles can be hovered but cannot be selected
                }
                //If the game is in main state
                else if (GameManager.gameManager.stateNum == 1)
                {
                    if (GameManager.gameManager.isWhiteTurn == true) //White turn
                    {
                        //If at least a white wall can still move
                        if (GameManager.gameManager.wallTurn > 0)
                        {
                            HoverCheck("WhiteWall", true);
                        }
                        //If the white gladiator can still move
                        if (GameManager.gameManager.gladTurn)
                        {
                            HoverCheck("WhiteGlad", true);
                        }
                        //If the white minotaur can still move
                        if (GameManager.gameManager.whiteMinoTurn == 0)
                        {
                            HoverCheck("WhiteMino", true);
                        }
                    }
                    else //Black turn
                    {
                        //If at least a black wall can still move
                        if (GameManager.gameManager.wallTurn > 0)
                        {
                            HoverCheck("BlackWall", true);
                        }
                        //If the black gladiator can still move
                        if (GameManager.gameManager.gladTurn)
                        {
                            HoverCheck("BlackGlad", true);
                        }
                        //If the black minotaur can still move
                        if (GameManager.gameManager.blackMinoTurn == 0)
                        {
                            HoverCheck("BlackMino", true);
                        }
                    }

                    //If a piece is being selected
                    if (selectedObj != null)
                    {
                        HoverCheck("ChessTile", false);
                        MovePiece();
                    }
                }

                prevObj = currentObj;
            }
        }
    }

    void LateUpdate()
    {
        //After moving a wall
        if (doWallCheck)
        {
            //If the gladiators are not blocked
            if (BlockedCheck() == true)
            {
                if (WallCheck() == true) //If the move is valid
                {
                    //Decrease a wall turn
                    GameManager.gameManager.wallTurn -= 1;

                    //Remove 1 wall token on the screen
                    if (GameManager.gameManager.wall1Token.GetComponent<Image>().color.a == 1.0f)
                    {
                        GameManager.gameManager.wall1Token.GetComponent<Image>().color = new Color(255, 255, 255, 0.1f);
                    }
                    else
                    {
                        GameManager.gameManager.wall2Token.GetComponent<Image>().color = new Color(255, 255, 255, 0.1f);
                    }

                    //Clear the selected object reference
                    selectedObj.GetComponent<Selectable>().isClicked = !selectedObj.GetComponent<Selectable>().isClicked;
                    selectedObj = null;

                    //Clear the error text
                    GameManager.gameManager.errorText.GetComponent<Text>().text = "";
                }
                else //If the move in invalid
                {
                    selectedObj.transform.position = backPos; //Move the wall back to initial position
                }
            }

            //If a gladiator is blocked
            else
            {
                GameManager.gameManager.errorText.GetComponent<Text>().text = "Can't block Gladiator!";
                selectedObj.transform.position = backPos; //Move the wall back to initial position
            }
            doWallCheck = false; //End of a moving wall check
        }

        //After moving a gladiator
        if (doGladCheck)
        {
            //If the gladiators are not blocked
            if (BlockedCheck() == true)
            {
                //If the white gladiator reaches the other end of the board
                if (selectedObj.tag == "WhiteGlad" && selectedObj.transform.position.z == 14)
                {
                    GameManager.gameManager.gameover = 1; //End the game with white victory
                }
                //If the black gladiator reaches the other end of the board
                if (selectedObj.tag == "BlackGlad" && selectedObj.transform.position.z == 0)
                {
                    GameManager.gameManager.gameover = 2; //End the game with black victory
                }

                //End the gladiator turn
                GameManager.gameManager.gladTurn = false;
                //Remove the gladiator token on the screen
                GameManager.gameManager.gladToken.GetComponent<Image>().color = new Color(255, 255, 255, 0.1f);

                //Clear the selected object reference
                selectedObj.GetComponent<Selectable>().isClicked = !selectedObj.GetComponent<Selectable>().isClicked;
                selectedObj = null;

                //Clear the error text
                GameManager.gameManager.errorText.GetComponent<Text>().text = "";
            }

            //If a gladiator is blocked
            else
            {
                GameManager.gameManager.errorText.GetComponent<Text>().text = "Can't block Gladiator!";
                selectedObj.transform.position = backPos; //Move the gladiator back to initial position
            }
            doGladCheck = false; //End of a gladiator moving check
        }

        //After moving a minotaur
        if (doMinoCheck)
        {
            //If the gladiators are not blocked
            if (BlockedCheck() == true) 
            {
                //If the white minotaur moves
                if (selectedObj.tag == "WhiteMino" && selectedObj.transform.position.z <= 6)
                {
                    //If the white minotaur takes the black gladiator
                    if (selectedObj.transform.position == GameManager.gameManager.blackGlad.transform.position)
                    {
                        Destroy(GameManager.gameManager.blackGlad); //Kill the black gladiator
                        GameManager.gameManager.gameover = 1; //End the game with white victory
                    }

                    //End the white minotaur turn
                    GameManager.gameManager.minoTurn = false;
                    //Remove the minotaur token on the screen
                    GameManager.gameManager.minoToken.GetComponent<Image>().color = new Color(255, 255, 255, 0.1f);
                    //White minotaur move counter
                    GameManager.gameManager.whiteMinoTurn = 2;

                    //Clear the selected object reference
                    selectedObj.GetComponent<Selectable>().isClicked = !selectedObj.GetComponent<Selectable>().isClicked;
                    selectedObj = null;

                    //Clear the error text
                    GameManager.gameManager.errorText.GetComponent<Text>().text = "";

                }

                //If the black minotaur moves
                else if (selectedObj.tag == "BlackMino" && selectedObj.transform.position.z >= 8)
                {
                    //If the black minotaur takes the white gladiator
                    if (selectedObj.transform.position == GameManager.gameManager.whiteGlad.transform.position)
                    {
                        Destroy(GameManager.gameManager.whiteGlad); //Kill the white gladiator
                        GameManager.gameManager.gameover = 2; //End the game with black victory
                    }

                    //End the black minotaur turn
                    GameManager.gameManager.minoTurn = false;
                    //Remove the minotaur token on the screen
                    GameManager.gameManager.minoToken.GetComponent<Image>().color = new Color(255, 255, 255, 0.1f);
                    //Black minotaur move counter
                    GameManager.gameManager.blackMinoTurn = 2;

                    //Clear the selected object reference
                    selectedObj.GetComponent<Selectable>().isClicked = !selectedObj.GetComponent<Selectable>().isClicked;
                    selectedObj = null;

                    //Clear the error text
                    GameManager.gameManager.errorText.GetComponent<Text>().text = "";
                }
                
                //If the minotaur tries to move to the other half of the board
                else
                {
                    GameManager.gameManager.errorText.GetComponent<Text>().text = "Minotaur can't cross the half!";
                    selectedObj.transform.position = backPos; //Move the minotaur back to initial position
                }
            }

            //If a gladiator is blocked
            else
            {
                GameManager.gameManager.errorText.GetComponent<Text>().text = "Can't block Gladiator!";
                selectedObj.transform.position = backPos; //Move the minotaur back to initial position
            }
            doMinoCheck = false; //End of a minotaur moving check
        }

        //Reset when all pieces have moved in a turn
        if (GameManager.gameManager.wallTurn == 0 && GameManager.gameManager.gladTurn == false && GameManager.gameManager.minoTurn == false)
        {
            //Reset wall and gladiator tokens
            GameManager.gameManager.wallTurn = 2;
            GameManager.gameManager.gladTurn = true;

            GameManager.gameManager.wall1Token.GetComponent<Image>().color = new Color(255, 255, 255, 1.0f);
            GameManager.gameManager.wall2Token.GetComponent<Image>().color = new Color(255, 255, 255, 1.0f);
            GameManager.gameManager.gladToken.GetComponent<Image>().color = new Color(255, 255, 255, 1.0f);

            //Check on minotaur token
            if (GameManager.gameManager.isWhiteTurn == true) //At the end of white turn
            {
                GameManager.gameManager.whiteMinoTurn -= 1; //Decrease white minotaur counter

                //If black minotaur counter already reaches 0, it can move in the upcoming turn
                if (GameManager.gameManager.blackMinoTurn == 0)
                {
                    GameManager.gameManager.minoTurn = true;
                    GameManager.gameManager.minoToken.GetComponent<Image>().color = new Color(255, 255, 255, 1.0f);
                }
            }
            else if (GameManager.gameManager.isWhiteTurn == false) //At the end of black turn
            {
                GameManager.gameManager.blackMinoTurn -= 1; //Decrease black minotaur counter

                //If white minotaur counter already reaches 0, it can move in the upcoming turn
                if (GameManager.gameManager.whiteMinoTurn == 0)
                {
                    GameManager.gameManager.minoTurn = true;
                    GameManager.gameManager.minoToken.GetComponent<Image>().color = new Color(255, 255, 255, 1.0f);
                }
            }
            GameManager.gameManager.isWhiteTurn = !GameManager.gameManager.isWhiteTurn; //Change turn
        }
    }

    //Check if certain objects can be hovered and selected
    void HoverCheck(string tag, bool clickCheck)
    {
        if (currentObj.CompareTag(tag))
        {
            currentObj.GetComponent<Selectable>().isHovered = true;
            if (clickCheck) //If the object can be selected
            {
                ClickCheck();
            }
        }
    }

    //Handle selecting an object
    void ClickCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Unselect a currently selected object
            if (selectedObj == currentObj)
            {
                selectedObj.GetComponent<Selectable>().isClicked = !selectedObj.GetComponent<Selectable>().isClicked;
                selectedObj = null;
            }
            //Select an object if there is no object being selected already
            else if (selectedObj == null)
            {
                selectedObj = currentObj;
                selectedObj.GetComponent<Selectable>().isHovered = false;
                selectedObj.GetComponent<Selectable>().isClicked = true;
            }
            //Select an object when another object is being selected
            else
            {
                selectedObj.GetComponent<Selectable>().isClicked = false;
                selectedObj = currentObj;
                selectedObj.GetComponent<Selectable>().isHovered = false;
                selectedObj.GetComponent<Selectable>().isClicked = true;
            }

        }
    }

    //Moving a piece on the board
    void MovePiece()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //If the player click on a tile
            if (currentObj.tag == "ChessTile")
            {
                //If the piece moves vertically
                if (currentObj.transform.position.x == selectedObj.transform.position.x)
                {
                    if (currentObj.transform.position.z == selectedObj.transform.position.z + 2 ||
                        currentObj.transform.position.z == selectedObj.transform.position.z - 2)
                    {
                        backPos = selectedObj.transform.position; //Save current position in case of undo

                        //If a wall is moving
                        if (selectedObj.tag == "WhiteWall" || selectedObj.tag == "BlackWall")
                        {
                            doWallCheck = true;
                            selectedObj.transform.position = currentObj.transform.position + GameManager.gameManager.wallOffset;
                        }

                        //If a gladiator is moving
                        else if (selectedObj.tag == "WhiteGlad" || selectedObj.tag == "BlackGlad")
                        {
                            doGladCheck = true;
                            selectedObj.transform.position = currentObj.transform.position + GameManager.gameManager.fighterOffset;
                        }

                        //If a minotaur is moving
                        else if (selectedObj.tag == "WhiteMino" || selectedObj.tag == "BlackMino")
                        {
                            doMinoCheck = true;
                            selectedObj.transform.position = currentObj.transform.position + GameManager.gameManager.fighterOffset;
                        }
                    }
                }

                //If the piece moves horizontally
                else if (currentObj.transform.position.z == selectedObj.transform.position.z)
                {
                    if (currentObj.transform.position.x == selectedObj.transform.position.x + 2 ||
                        currentObj.transform.position.x == selectedObj.transform.position.x - 2)
                    {
                        backPos = selectedObj.transform.position; //Save current position in case of undo

                        //If a wall is moving
                        if (selectedObj.tag == "WhiteWall" || selectedObj.tag == "BlackWall")
                        {
                            doWallCheck = true;
                            selectedObj.transform.position = currentObj.transform.position + GameManager.gameManager.wallOffset;
                        }

                        //If a gladiator is moving
                        else if (selectedObj.tag == "WhiteGlad" || selectedObj.tag == "BlackGlad")
                        {
                            doGladCheck = true;
                            selectedObj.transform.position = currentObj.transform.position + GameManager.gameManager.fighterOffset;
                        }

                        //If a minotaur is moving
                        else if (selectedObj.tag == "WhiteMino" || selectedObj.tag == "BlackMino")
                        {
                            doMinoCheck = true;
                            selectedObj.transform.position = currentObj.transform.position + GameManager.gameManager.fighterOffset;
                        }
                    }
                }
            }
        }
    }

    //Check if a wall movement is valid
    bool WallCheck()
    {
        //Walls reference
        GameObject[] walls;
        if (GameManager.gameManager.isWhiteTurn == true)
        {
            walls = GameManager.gameManager.wallsW;
        }
        else
        {
            walls = GameManager.gameManager.wallsB;
        }

        //Iterate through the list of walls
        for (int i = 0; i < walls.Length ; i++)
        {
            //If there are 3 or more adjacent walls
            if (walls[i].GetComponent<Wall>().CheckAdjacent() > 1)
            {
                GameManager.gameManager.errorText.GetComponent<Text>().text = "3 or more adjacent walls!";
                return false;
            }
            //If the white wall tries to cross the half of the board
            else if (walls[i].tag == "WhiteWall" && walls[i].transform.position.z > 6)
            {
                GameManager.gameManager.errorText.GetComponent<Text>().text = "Walls can't cross the half!";
                return false;
            }
            //If the black wall tries to cross the half of the board
            else if (walls[i].tag == "BlackWall" && walls[i].transform.position.z < 8)
            {
                GameManager.gameManager.errorText.GetComponent<Text>().text = "Walls can't cross the half!";
                return false;
            }
        }
        return true;
    }

    //Check if a gladiator is blocked
    bool BlockedCheck()
    {
        if (GameManager.gameManager.whiteGlad.GetComponent<Gladiator>().CheckBlock() > 3)
        {
            return false;
        }
        if (GameManager.gameManager.blackGlad.GetComponent<Gladiator>().CheckBlock() > 3)
        {
            return false;
        }
        return true;
    }
}
