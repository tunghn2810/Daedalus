using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //White prefabs
    public GameObject wallWhitePrefab;
    public GameObject gladiatorWhitePrefab;
    public GameObject minotaurWhitePrefab;

    //Black prefabs
    public GameObject wallBlackPrefab;
    public GameObject gladiatorBlackPrefab;
    public GameObject minotaurBlackPrefab;

    //Text objects
    public GameObject victoryText;
    public GameObject whiteVictoryText;
    public GameObject blackVictoryText;
    public GameObject errorText;
    public GameObject turnText;

    //Icon objects
    public GameObject wall1Token;
    public GameObject wall2Token;
    public GameObject gladToken;
    public GameObject minoToken;
    
    //Gladiator references
    [System.NonSerialized] public GameObject whiteGlad;
    [System.NonSerialized] public GameObject blackGlad;

    //Offsets for walls and gladiators/minotaurs
    [System.NonSerialized] public Vector3 wallOffset = new Vector3(0, 1.5f, 0);
    [System.NonSerialized] public Vector3 fighterOffset = new Vector3(0, 0.5f, 0);

    //Keep track of white/black turn
    public bool isWhiteTurn = true;

    //Gameover conditions
    //0 = not gameover, 1 = white wins, 2 = black wins
    [System.NonSerialized] public int gameover = 0;

    //Per turn
    public int wallTurn = 2; //Counter for wall movement
    public bool gladTurn = true; //Check if gladiator has moved
    public bool minoTurn = true; //Check if minotaur has moved
    public int whiteMinoTurn = 0; //Check if the white minotaur can move this turn
    public int blackMinoTurn = 0; //Check if the black minotaur can move this turn

    int wallWhiteNum; //Number of white walls
    bool gladWhitePlaced = false; //Check if white gladiator is placed on the board
    bool minoWhitePlaced = false; //Check if white minotaur is placed on the board

    int wallBlackNum; //Number of black walls
    bool gladBlackPlaced = false; //Check if black gladiator is placed on the board
    bool minoBlackPlaced = false; //Check if black minotaur is placed on the board

    public GameObject[] wallsW; //White wall gameobjects
    public GameObject[] wallsB; //Black wall gameobjects

    //Game states
    // 0 = Setup, 1 = Main, 2 = End
    public int stateNum = 0;

    public IEnumerable gameState;

    public static GameManager gameManager { get; set; }
    void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameState = WallState();
        StartCoroutine(RunGameLoop());
    }

    public IEnumerator RunGameLoop()
    {
        while (gameState != null)
        {
            foreach (IEnumerable currState in gameState)
            {
                yield return currState;
            }
        }
    }

    //Wall placement phase
    public IEnumerable WallState()
    {
        stateNum = 0;

        if (isWhiteTurn == true) //White turn
        {
            turnText.GetComponent<Text>().text = "White Wall Placement";
            turnText.GetComponent<Text>().color = Color.white;
        }
        else //Black turn
        {
            turnText.GetComponent<Text>().text = "Black Wall Placement";
            turnText.GetComponent<Text>().color = Color.black;
        }

        //Assign wall gameobjects
        wallsW = GameObject.FindGameObjectsWithTag("WhiteWall");
        wallsB = GameObject.FindGameObjectsWithTag("BlackWall");

        wallWhiteNum = wallsW.Length;
        wallBlackNum = wallsB.Length;

        //Players can place 7 walls on their side
        if (wallWhiteNum < 7 || wallBlackNum < 7)
        {
            if (isWhiteTurn)
            {
                PlacePiece(wallWhitePrefab, wallOffset);
            }
            else
            {
                PlacePiece(wallBlackPrefab, wallOffset);
            }
            yield return null;
        }
        else
        {
            //Go to next state when all walls are placed
            gameState = GladiatorState();
        }
    }

    //Gladiator placement phase
    public IEnumerable GladiatorState()
    {
        if (isWhiteTurn == true) //White turn
        {
            turnText.GetComponent<Text>().text = "White Gladiator Placement";
            turnText.GetComponent<Text>().color = Color.white;
        }
        else //Black turn
        {
            turnText.GetComponent<Text>().text = "Black Gladiator Placement";
            turnText.GetComponent<Text>().color = Color.black;
        }

        //Check if gladiators are placed on the board
        gladWhitePlaced = GameObject.FindGameObjectWithTag("WhiteGlad");
        gladBlackPlaced = GameObject.FindGameObjectWithTag("BlackGlad");

        if (gladWhitePlaced == false && isWhiteTurn == true)
        {
            PlacePiece(gladiatorWhitePrefab, fighterOffset);
        }
        else if (gladBlackPlaced == false && isWhiteTurn == false)
        {
            PlacePiece(gladiatorBlackPrefab, fighterOffset);
        }
        else
        {
            //Go to next state when both gladiators are placed
            gameState = MinotaurState();
        }
        yield return null;
    }

    //Minotaur placement phase
    public IEnumerable MinotaurState()
    {
        if (isWhiteTurn == true) //White turn
        {
            turnText.GetComponent<Text>().text = "White Minotaur Placement";
            turnText.GetComponent<Text>().color = Color.white;
        }
        else //Black turn
        {
            turnText.GetComponent<Text>().text = "Black Minotaur Placement";
            turnText.GetComponent<Text>().color = Color.black;
        }

        //Check if minotaurs are placed on the board
        minoWhitePlaced = GameObject.FindGameObjectWithTag("WhiteMino");
        minoBlackPlaced = GameObject.FindGameObjectWithTag("BlackMino");

        if (minoWhitePlaced == false && isWhiteTurn == true)
        {
            PlacePiece(minotaurWhitePrefab, fighterOffset);
        }
        else if (minoBlackPlaced == false && isWhiteTurn == false)
        {
            PlacePiece(minotaurBlackPrefab, fighterOffset);
        }
        else
        {
            //Go to next state when both gladiators are placed
            gameState = MoveState();

            //Turn on the icons
            wall1Token.SetActive(true);
            wall2Token.SetActive(true);
            gladToken.SetActive(true);
            minoToken.SetActive(true);
        }
        yield return null;
    }

    //Main game phase
    public IEnumerable MoveState()
    {
        stateNum = 1;

        if (isWhiteTurn == true) //White turn
        {
            turnText.GetComponent<Text>().text = "White Turn";
            turnText.GetComponent<Text>().color = Color.white;
        }
        else //Black turn
        {
            turnText.GetComponent<Text>().text = "Black Turn";
            turnText.GetComponent<Text>().color = Color.white;
        }

        //If an end game condition is met
        if (gameover != 0)
        {
            //Go to the end state
            gameState = EndState();
        }
        yield return null;
    }

    //End game phase
    public IEnumerable EndState()
    {
        stateNum = 2;

        victoryText.SetActive(true);
        if (gameover == 1) //If white wins
        {
            whiteVictoryText.SetActive(true);
            blackVictoryText.SetActive(false);
        }
        else if (gameover == 2) //If black wins
        {
            whiteVictoryText.SetActive(false);
            blackVictoryText.SetActive(true);
        }
        yield return null;
    }

    //Place a wall/gladiator/minotaur on the board
    void PlacePiece(GameObject prefab, Vector3 offset)
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hitInfo, 100f))
        {
            if (hitInfo.transform.gameObject.tag == "ChessTile") //If you hover over a chess tile
            {
                if (isWhiteTurn == true) //White turn
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Place a white gladiator or minotaur
                        if (prefab == gladiatorWhitePrefab || prefab == minotaurWhitePrefab)
                        {
                            if (hitInfo.transform.position.z == 0) //Can only be placed on player's first row of the board
                            {
                                GameObject newPiece = Instantiate(prefab, hitInfo.transform.position + offset, prefab.transform.rotation) as GameObject;
                                if (prefab == gladiatorWhitePrefab)
                                {
                                    whiteGlad = newPiece;
                                }
                                errorText.GetComponent<Text>().text = "";
                                isWhiteTurn = !isWhiteTurn; //End white turn
                            }
                            else
                            {
                                errorText.GetComponent<Text>().text = "Can't place there!";
                            }
                        }
                        //Place a white wall
                        else
                        {
                            if (hitInfo.transform.position.z >= 0 &&
                            hitInfo.transform.position.z <= 6) //Can only be placed on player's half of the board
                            {
                                GameObject newPiece = Instantiate(prefab, hitInfo.transform.position + offset, prefab.transform.rotation) as GameObject;
                                wallsW = GameObject.FindGameObjectsWithTag("WhiteWall");

                                //If there are more than 3 adjacent white walls when a wall is placed, undo the placement
                                if (PlacementCheck(wallsW) == false)
                                {
                                    Destroy(newPiece);
                                }
                                else
                                {
                                    errorText.GetComponent<Text>().text = "";
                                    isWhiteTurn = !isWhiteTurn; //End white turn
                                }
                            }
                            else
                            {
                                errorText.GetComponent<Text>().text = "Can't place there!";
                            }
                        }
                    }
                }
                else //Black turn
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Place a black gladiator or minotaur
                        if (prefab == gladiatorBlackPrefab || prefab == minotaurBlackPrefab)
                        {
                            if (hitInfo.transform.position.z == 14) //Can only be placed on player's first row of the board
                            {
                                GameObject newPiece = Instantiate(prefab, hitInfo.transform.position + offset, prefab.transform.rotation) as GameObject;
                                if (prefab == gladiatorBlackPrefab)
                                {
                                    blackGlad = newPiece;
                                }
                                errorText.GetComponent<Text>().text = "";
                                isWhiteTurn = !isWhiteTurn; //End black turn
                            }
                            else
                            {
                                errorText.GetComponent<Text>().text = "Can't place there!";
                            }
                        }
                        else
                        {
                            if (hitInfo.transform.position.z >= 8 &&
                            hitInfo.transform.position.z <= 14)
                            {
                                GameObject newPiece = Instantiate(prefab, hitInfo.transform.position + offset, prefab.transform.rotation) as GameObject;
                                wallsB = GameObject.FindGameObjectsWithTag("BlackWall");

                                //If there are more than 3 adjacent black walls when a wall is placed, undo the placement
                                if (PlacementCheck(wallsB) == false)
                                {
                                    Destroy(newPiece);
                                }
                                else
                                {
                                    errorText.GetComponent<Text>().text = "";
                                    isWhiteTurn = !isWhiteTurn; //End black turn
                                }
                            }
                            else
                            {
                                errorText.GetComponent<Text>().text = "Can't place there!";
                            }
                        }
                    }
                }
            }
        }
    }

    //Check if there are 3 adjacent walls
    bool PlacementCheck(GameObject[] pieces)
    {
        foreach (GameObject obj in pieces)
        {
            if (obj.tag == "WhiteWall" || obj.tag == "BlackWall")
            {
                if (obj.GetComponent<Wall>().CheckAdjacent() > 1)
                {
                    errorText.GetComponent<Text>().text = "3 or more adjacent walls!";
                    return false;
                }
            }
        }
        errorText.GetComponent<Text>().text = "";
        return true;
    }

    //Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
