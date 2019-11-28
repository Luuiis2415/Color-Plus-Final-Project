using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    int gridX = 8;
    int gridY = 5;

    int score = 0;
    int turns = 0;
    float turnLength = 2;
    float gameTimeRunner = 60;

    GameObject activeCube = null;
    public GameObject cubePrefab, nextCubePrefab;
    GameObject[,] grid;
    GameObject nextCube;
    Color[] myColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };

    Vector3 cubePos;
    // the vector right here is specified for my game, since thats the position its in
    Vector3 nextCubePos = new Vector3(9, 10, 0);


    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();

    }


    void CreateGrid()
    {

        grid = new GameObject[gridX, gridY];

        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {

                cubePos = new Vector3(x * 2, y * 2, 0);
                grid[x,y] = Instantiate(cubePrefab, cubePos, Quaternion.identity);
                grid[x, y].GetComponent<CubeController>().myX = x;
                grid[x, y].GetComponent<CubeController>().myY = y;
            }
        }
    }

    void CreateNextCube()
    {
        nextCube = Instantiate(nextCubePrefab, nextCubePos, Quaternion.identity);
        nextCube.GetComponent<Renderer>().material.color = myColors[Random.Range(0, myColors.Length)];
        
    }

    // method that will show what happens when the game reaches its end
    // based on time, no available cubes/rows by either pressing or not pressing
    // keyboard 1-5
    void EndGame(bool victory)
    {
        if (victory)
        {
            print("You're Victorious!");

        }
        else
        {
            print("You Lose. Keep Trying!");
        }

    }

    GameObject ChooseWhiteCube(List<GameObject> whiteCubes)
    {
        if(whiteCubes.Count == 0)
        {
            return null;
        }
        return whiteCubes[Random.Range(0, whiteCubes.Count)];

    }

    // new method that will return a game object
    // which is there an available cube/row
    GameObject LocateAvailableCube (int y)
    {
        List<GameObject> whiteCubes = new List<GameObject> ();


        for(int x = 0; x < gridX; x++)
        {
            if(grid[x,y].GetComponent<Renderer>().material.color == Color.white)
            {
                whiteCubes.Add(grid[x,y]);
            }
        }
        return ChooseWhiteCube(whiteCubes);


    }

    GameObject LocateAvailableCube()
    {
        List<GameObject> whiteCubes = new List<GameObject>();

        for(int y = 0; y < gridY; y++)
        {
            for(int x = 0; x < gridX; x++)
            {
                if(grid[x,y].GetComponent<Renderer>().material.color == Color.white)
                {
                    whiteCubes.Add(grid[x, y]);
                }
            }
        }
        return ChooseWhiteCube(whiteCubes);

    }

    void SetCubeColor (GameObject myCube, Color color)
    {
        if(myCube == null)
        {
            EndGame(false);
        }
        else
        {
            myCube.GetComponent<Renderer>().material.color = color;
            Destroy(nextCube);
            nextCube = null;
        }

    }



    void SetNextCube(int y)
    {
        //where to put it and tell it what row to look in
        GameObject whiteCube = LocateAvailableCube(y);

        SetCubeColor(whiteCube, nextCube.GetComponent<Renderer>().material.color);
    }

    void AddBlackCube()
    {
        GameObject whiteCube = LocateAvailableCube();

        SetCubeColor(whiteCube, Color.black);

    }


    void ProcessKeyInput()
    {
        int keyNumPressed = 0;


        // here we write an if statement about 'if the player presses 1-5 and there's still a nextcube
        // it is not pretty but this checks each one
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            keyNumPressed = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            keyNumPressed = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            keyNumPressed = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            keyNumPressed = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            keyNumPressed = 5;
        }


        // if we still have a next cube and player pressed valid number key
        if (nextCube != null && keyNumPressed != 0)
        {
            //setting the next cube in the specified row, subtracting 1 since grid array has a 0-based index
            SetNextCube(keyNumPressed - 1);
        }

    }

    public void ProcessClick(GameObject clickedCube, int x, int y, Color cubeColor, bool active)
    {
        if(cubeColor != Color.white && cubeColor != Color.black)
        {
            //this is when we have an active cube already
            if (active)
            {
                clickedCube.transform.localScale /= 1.5f;
                clickedCube.GetComponent<CubeController>().active = false;
                activeCube = null;
            }
            // this is when we don't have one yet
            else
            {
                if(activeCube != null)
                {

                    activeCube.transform.localScale /= 1.5f;
                    activeCube.GetComponent<CubeController>().active = false;

                }

                clickedCube.transform.localScale *= 1.5f;
                clickedCube.GetComponent<CubeController>().active = true;
                activeCube = clickedCube;

            }
        }
        else if (cubeColor == Color.white && activeCube != null)
        {
            int xDist = clickedCube.GetComponent<CubeController>().myX - activeCube.GetComponent<CubeController>().myX;
            int yDist = clickedCube.GetComponent<CubeController>().myY - activeCube.GetComponent<CubeController>().myY;

            // within one including diagnols
            if(Mathf.Abs(yDist)<= 1 && Mathf.Abs(xDist) <= 1)
            {
                // here we set the clicked cube to be active
                clickedCube.GetComponent<Renderer>().material.color = activeCube.GetComponent<Renderer>().material.color;
                clickedCube.transform.localScale *= 1.5f;
                clickedCube.GetComponent<CubeController>().active = true;

                // here we set old active cube to be white and not active
                activeCube.GetComponent<Renderer>().material.color = Color.white;
                activeCube.transform.localScale /= 1.5f;
                activeCube.GetComponent<CubeController>().active = false;

                // we are keeing track of the new active cube
                activeCube = clickedCube;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessKeyInput();

        if(Time.time > turnLength * turns)
        {
            turns++;

            if (nextCube != null)
            {
                score -= 1;
                AddBlackCube();

            }

            CreateNextCube();
        }
    }
}
