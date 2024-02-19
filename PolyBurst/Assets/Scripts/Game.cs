using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Game : MonoBehaviour
{

    public static int gridWidth = 10;
    public static int gridHeight = 18;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public Canvas hud_canvas;
    public Canvas pause_canvas;

    public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200;
    public int scoreComboLine = 2400;

    public int currentLevel = 0;
    private int numLinesCleared = 0;

    public float fallSpeed = 1.0f;
    public static bool isPaused = false;

    public TextMeshProUGUI hud_score;
    public TextMeshProUGUI hud_level;
    public TextMeshProUGUI hud_lines;

    private int numberOfRowsThisTurn = 0;
    private int numberOfRowsLastTurn = 0;

    public static int currentScore = 0;

    private GameObject previewTetromino;
    private GameObject nextTetromino;
    private GameObject savedTetromino;
    private GameObject ghostTetromino;

    private bool gameStarted = false;
    private int startingHighScore;
    private int startingScore;

    private Vector2 previewTetrominoPosition = new Vector2(13.5f, 17);
    private Vector2 savedTetrominoPosition = new Vector2(-4.5f, 17);

    public int MaxSwaps = 2;
    private int currentSwaps = 0;

    private Queue<string> tetrominoBag = new Queue<string>();

    public static Game Instance;

    private void Awake()
    {
        instance = this;
    }

    public static Game instance;

    public Transform GetCurrentActiveTetrominoTransform()
    {
        return GameObject.FindGameObjectWithTag("currentActiveTetromino").transform;
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }


    void Start()
    {
        pause_canvas.enabled = false;
        hud_score.text = "0";

        currentScore = 0;

        hud_level.text = "0";
        hud_lines.text = "0";

        FillTetrominoBag();
        SpawnNextTetromino();

        startingHighScore = PlayerPrefs.GetInt("highscore");
        startingScore = PlayerPrefs.GetInt("score");
    }

    void Update()
    {
        UpdateScore();

        UpdateUI();

        UpdateLevel();

        UpdateSpeed();

        CheckUserInput();
    }

    void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
                PauseGame();
            else
                ResumeGame();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            SaveTetromino(tempNextTetromino.transform);
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        pause_canvas.enabled = true;
        //add auido pause
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        pause_canvas.enabled = false;
        //add audio play
    }

    void UpdateLevel()
    {
        currentLevel = numLinesCleared / 10;
    }

    void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void UpdateUI()
    {

        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
        hud_lines.text = numLinesCleared.ToString();
    }

    public void UpdateScore()
    {

        if (numberOfRowsThisTurn > 0)
        {

            if (numberOfRowsThisTurn == 1)
            {
                ClearedOneLine();
                numberOfRowsLastTurn = 1;

            }
            else if (numberOfRowsThisTurn == 2)
            {
                ClearedTwoLine();
                numberOfRowsLastTurn = 2;

            }
            else if (numberOfRowsThisTurn == 3)
            {
                ClearedThreeLine();
                numberOfRowsLastTurn = 3;

            }
            else if (numberOfRowsThisTurn == 4)
            {
                ClearedFourLine();
                numberOfRowsLastTurn = 4;

            }

            numberOfRowsThisTurn = 0;

        }

        if (numberOfRowsLastTurn == 4 && numberOfRowsThisTurn == 4)
        {
            ClearedComboLine();
            numberOfRowsLastTurn = 0;
            Game.Instance.UpdateHighScore();
        }
    }

    public void ClearedOneLine()
    {

        currentScore += scoreOneLine + (currentLevel * 20);
        numLinesCleared++;
    }

    public void ClearedTwoLine()
    {

        currentScore += scoreTwoLine + (currentLevel * 25);
        numLinesCleared += 2;
    }

    public void ClearedThreeLine()
    {

        currentScore += scoreThreeLine + (currentLevel * 30);
        numLinesCleared += 3;
    }

    public void ClearedFourLine()
    {

        currentScore += scoreFourLine + (currentLevel * 40);
        numLinesCleared += 4;
    }

    public void ClearedComboLine()
    {

        currentScore += scoreComboLine + (currentLevel * 80);
        numLinesCleared += 4;
    }

    public void UpdateHighScore()
    {

        if (currentScore > startingHighScore)
        {
            PlayerPrefs.SetInt("highscore", currentScore);
        }

        PlayerPrefs.SetInt("score", currentScore);
    }

    bool CheckIsValidPosition(GameObject tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = round(mino.position);

            if (!CheckIsInsideGrid(pos))
                return false;

            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
                return false;
        }

        return true;
    }

    public bool CheckIsAboveGrid(Tetromino tetromino)
    {

        for (int x = 0; x < gridWidth; ++x)
        {

            foreach (Transform mino in tetromino.transform)
            {

                Vector2 pos = round(mino.position);

                if (pos.y > gridHeight)
                {

                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckIsInsideGrid(Vector2 pos)
    {

        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public bool IsFullRowAt(int y)
    {

        for (int x = 0; x < gridWidth; ++x)
        {

            if (grid[x, y] == null)
            {

                return false;
            }
        }

        numberOfRowsThisTurn++;

        return true;
    }

    public void DeleteMinoAt(int y)
    {

        for (int x = 0; x < gridWidth; ++x)
        {

            Destroy(grid[x, y].gameObject);

            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {

            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];

                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowsDown(int y)
    {

        for (int i = y; i < gridHeight; ++i)
        {

            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {

        for (int y = 0; y < gridHeight; ++y)
        {

            if (IsFullRowAt(y))
            {

                DeleteMinoAt(y);

                MoveAllRowsDown(y + 1);

                --y;
            }
        }
    }

    public void UpdateGrid(Tetromino tetromino)
    {
        for (int y = 0; y < gridHeight; ++y)
        {

            for (int x = 0; x < gridWidth; ++x)
            {

                if (grid[x, y] != null)
                {

                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetromino.transform)
        {

            Vector2 pos = round(mino.position);

            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {

        if (pos.y > gridHeight - 1)
        {

            return null;
        }

        return grid[(int)pos.x, (int)pos.y];
    }

    public Vector2 round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public void SpawnNextTetromino()
    {
        if (!gameStarted)
        {

            gameStarted = true;

            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 18.0f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
            nextTetromino.tag = "currentActiveTetromino";

            SpawnGhostTetromino();

        }
        else
        {

            previewTetromino.transform.localPosition = new Vector2(5.0f, 18.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.tag = "currentActiveTetromino";
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;

            SpawnGhostTetromino();
        }

        currentSwaps = 0;
    }

    public void SpawnGhostTetromino()
    {
        if (GameObject.FindGameObjectWithTag("currentGhostTetromino") != null)
            Destroy(GameObject.FindGameObjectWithTag("currentGhostTetromino"));

        ghostTetromino = (GameObject)Instantiate(nextTetromino, nextTetromino.transform.position, Quaternion.identity);

        Destroy(ghostTetromino.GetComponent<Tetromino>());

        ghostTetromino.AddComponent<GhostTetromino>();
    }

    public void SaveTetromino(Transform t)
    {
        currentSwaps++;

        if (currentSwaps > MaxSwaps)
            return;

        if (savedTetromino != null)
        {

            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSavedTetromino");

            tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);

            if (!CheckIsValidPosition(tempSavedTetromino))
            {

                tempSavedTetromino.transform.localPosition = savedTetrominoPosition;

                return;
            }

            savedTetromino = (GameObject)Instantiate(t.gameObject);
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";

            nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActiveTetromino";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetromino);

            SpawnGhostTetromino();

        }
        else
        {

            savedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = ("currentSavedTetromino");

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));

            SpawnNextTetromino();
        }
    }

    public void GameOver()
    {

        SceneManager.LoadScene("GameOver");
    }

    void FillTetrominoBag()
    {
        List<string> tetrominoes = new List<string>
        {
            "Prefabs/Tetrimino_T",
            "Prefabs/Tetromino_I",
            "Prefabs/Tetrimino_O",
            "Prefabs/Tetronimo_J",
            "Prefabs/Tetronimo_L",
            "Prefabs/Tetronimo_S",
            "Prefabs/Tetrimino_Z",
            "Prefabs/Tetromino_ZZ 1",
            "Prefabs/Tetromino_SS",
            "Prefabs/Tetromino_T",
            "Prefabs/Tetromino_JJ",
            "Prefabs/Tetromino_LL",
            "Prefabs/Tetromino_P",
            "Prefabs/Tetromino_PP",
            "Prefabs/Tetromino_U",
            "Prefabs/Tetromino_V",
            "Prefabs/Tetromino_F",
            "Prefabs/Tetromino_FF",
            "Prefabs/Tetromino_II",
            "Prefabs/Tetromino_NN",
            "Prefabs/Tetromino_N",
            "Prefabs/Tetromino_W",
            "Prefabs/Tetromino_WW",
            "Prefabs/Tetromino_X",
            "Prefabs/Tetromino_Y",
            "Prefabs/Tetromino_YY"
        };

        // Shuffle the tetrominoes using algorithm
        for (int i = tetrominoes.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = tetrominoes[i];
            tetrominoes[i] = tetrominoes[j];
            tetrominoes[j] = temp;
        }

        foreach (string tetromino in tetrominoes)
        {
            tetrominoBag.Enqueue(tetromino);
        }
    }

    string GetRandomTetromino()
    {
        if (tetrominoBag.Count == 0)
        {
            FillTetrominoBag();
        }

        return tetrominoBag.Dequeue();
    }

}