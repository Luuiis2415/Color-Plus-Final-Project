using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    
    int gridX = 8;
    int gridY = 5;

    int score = 0;
    int turns = 0;
    float turnLength = 3;
    float gameTimeRunner = 60;

    public GameObject cubePrefab;
    public Text scoreText;
    public Text nextCubeText;

    GameObject activeCube = null;
    GameObject[,] grid;
    GameObject nextCube;
    Color[] myColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };

    Vector3 cubePos;
    // the vector location right here is specified for my game, since thats the position its in
    Vector3 nextCubePos = new Vector3(9, 10, 0);

    // have these here so we can change them later if we need to
    // rather than define them right away inside code
    int rainbowPoints = 5;
    int sameColorPoints = 10;

    bool gameOver = false;


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
        nextCube = Instantiate(cubePrefab, nextCubePos, Quaternion.identity);
        nextCube.GetComponent<Renderer>().material.color = myColors[Random.Range(0, myColors.Length)];
        nextCube.GetComponent<CubeController>().nextCube = true;
        
    }

    // method that will show what happens when the game reaches its end
    // based on time, no available cubes/rows by either pressing or not pressing
    // keyboard 1-5
    void EndGame(bool win)
    {
        // because i named it Victory Scene in my code and Win Scene in unity it didnt load
        // i had to go back and double check everything
        if (win)
        {
            nextCubeText.text = "You Win!";
            
        }
        else
        {

            nextCubeText.text = "You Lose. Try Again!";

        }
        //this is just in case timing is such that next cube still exists
        Destroy(nextCube);
        nextCube = null;

        // here we disable all of the cubes
        for(int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                grid[x, y].GetComponent<CubeController>().nextCube = true;
            }
        }

        gameOver = true;
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
    // checking if there is an available cube in the row
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


    // in this method we SetCubeColor but it does more than that, be careful!
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
        
        // a color value that is beyond the max
        SetCubeColor(whiteCube, Color.black);

    }


    void ProcessKeyInput()
    {
        int keyNumPressed = 0;


        // here we write an if statement about 'if the player presses 1-5 and there's still a nextcube at the top
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

    bool IsRainbowPlus(int x, int y)
    {
        // gets convoluted because of how many colors there are
        Color a = grid[x, y].GetComponent<Renderer>().material.color;
        Color b = grid[x + 1, y].GetComponent<Renderer>().material.color;
        Color c = grid[x - 1, y].GetComponent<Renderer>().material.color;
        Color d = grid[x, y + 1].GetComponent<Renderer>().material.color;
        Color e = grid[x, y - 1].GetComponent<Renderer>().material.color;
        
        // if any of the colors are either white or black , then there's no rainbow plus
        if(a == Color.white || a == Color.black ||
           b == Color.white || b == Color.black ||
           c == Color.white || c == Color.black ||
           d == Color.white || d == Color.black ||
           e == Color.white || e == Color.black)
        {
            return false;
        }


        // we are ensuring that every color is different from every other color
        if(a != b && a != c && a != d && a != e &&
            b != c && b != d && b != e &&
            c != d && c != e &&
            d != e)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    bool IsSameColorPlus(int x, int y)
    {
        if (grid[x,y].GetComponent<Renderer>().material.color != Color.white &&
            grid[x,y].GetComponent<Renderer>().material.color != Color.black &&
            grid[x, y].GetComponent<Renderer>().material.color == grid[x + 1, y].GetComponent<Renderer>().material.color &&
            grid[x, y].GetComponent<Renderer>().material.color == grid[x - 1, y].GetComponent<Renderer>().material.color &&
            grid[x, y].GetComponent<Renderer>().material.color == grid[x, y + 1].GetComponent<Renderer>().material.color &&
            grid[x, y].GetComponent<Renderer>().material.color == grid[x, y - 1].GetComponent<Renderer>().material.color)
        {
            return true;
        }
        else
        {
            return false;
        }


    }

    // when  we want to make a black plus based on the rainbow color or based on same color
    void MakeBlackPlus(int x, int y)
    {
        // this is an error check to ensure that x and y arent on the edge of the grid
        // because if they were then something would go out of bounds
        if(x == 0 || y == 0 || x == gridX - 1 || y == gridY - 1)
        {
            return;
        }


        grid[x, y].GetComponent<Renderer>().material.color = Color.black;
        grid[x + 1, y].GetComponent<Renderer>().material.color = Color.black;
        grid[x - 1, y].GetComponent<Renderer>().material.color = Color.black;
        grid[x, y + 1].GetComponent<Renderer>().material.color = Color.black;
        grid[x, y - 1].GetComponent<Renderer>().material.color = Color.black;

        // here if we had an active cube and it was involved in the plus
        if(activeCube != null && activeCube.GetComponent<Renderer>().material.color == Color.black) 
        {
            //deactive it here
            activeCube.transform.localScale /= 1.5f;
            activeCube.GetComponent<CubeController>().active = false;
            activeCube = null;

        }


    }


    // a method to check rainbow and came color plus points
    void Score()
    {
        

        for(int x = 1; x < gridX - 1; x++)
        {
            for(int y = 1; y < gridY - 1; y++)
            {
                // saying is this a rainbow plus with the x and y centered arounf there
                // if it is what should we do
                if (IsRainbowPlus(x, y))
                {

                    score += rainbowPoints;
                    MakeBlackPlus(x, y);

                }
                if(IsSameColorPlus(x, y))
                {

                    score += sameColorPoints;
                    MakeBlackPlus(x, y);

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < gameTimeRunner)
        {

            ProcessKeyInput();
            Score();

            if (Time.time > turnLength * turns)
            {
                turns++;

                if (nextCube != null)
                {
                    score -= 1;
                    // we are ensuring it does not go negative
                    if (score < 0)
                    {
                        score = 0;
                    }
                    AddBlackCube();
                }
                CreateNextCube();
            }

            scoreText.text = "Score: " + score;
        }
        else if(!gameOver)
        {
            if(score > 0)
            {
                EndGame(true);
            }
            else
            {
                EndGame(false);
            }
        }

    }
}
