using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    float fall = 0;
    private float fallSpeed;

    public bool allowRotation = true;
    public bool limitRotation = false;
    public string prefabName;
    private bool isPlaced = false;

    private float continuousVerticalSpeed = 0.025f;
    private float continuousHorizontalSpeed = 0.05f;
    private float buttonDownWaitMax = 0.05f;

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimerHorizontal = 0;
    private float buttonDownWaitTimerVertical = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    public int individualScore = 100;

    private float individualScoreTime;


    void Start()
    {
        fallSpeed = GameObject.Find("GameScript").GetComponent<Game>().fallSpeed;
    }

    void Update()
    {
        if (!Game.isPaused)
        {
            CheckUserInput();
            UpdateIndividualScore();
        }
    }

    void UpdateIndividualScore()
    {

        if (individualScoreTime < 1)
        {
            individualScoreTime += Time.deltaTime;

        }
        else
        {

            individualScoreTime = 0;

            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }

    void CheckUserInput()
    {

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            movedImmediateHorizontal = false;

            horizontalTimer = 0;
            buttonDownWaitTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            movedImmediateVertical = false;

            verticalTimer = 0;
            buttonDownWaitTimerVertical = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();

        }
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            MoveDown();

        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RotateLeft();

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RotateRight();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SlamDown();
        }

    }

    public void SlamDown()
    {
        while (CheckIsInValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }
        if (!CheckIsInValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<Game>().UpdateGrid(this);

            FindObjectOfType<Game>().DeleteRow();

            if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Game>().GameOver();
            }

            FindObjectOfType<Game>().SpawnNextTetromino();

            Game.currentScore += individualScore;

            enabled = false;
            tag = "Untagged";
        }
    }

    void MoveLeft()
    {

        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

        transform.position += new Vector3(-1, 0, 0);

        if (CheckIsInValidPosition())
        {

            FindObjectOfType<Game>().UpdateGrid(this);

        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    void MoveRight()
    {

        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

        transform.position += new Vector3(1, 0, 0);

        if (CheckIsInValidPosition())
        {

            FindObjectOfType<Game>().UpdateGrid(this);

        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    void MoveDown()
    {
        if (movedImmediateVertical)
        {
            if (buttonDownWaitTimerVertical < buttonDownWaitMax)
            {
                buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < continuousVerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateVertical)
            movedImmediateVertical = true;

        verticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);

        if (CheckIsInValidPosition()) {
            FindObjectOfType<Game>().UpdateGrid(this);

        } else {
            transform.position += new Vector3(0, 1, 0);

            if (!isPlaced) {
                isPlaced = true;
                StartCoroutine(DelayBeforeNextTetromino());
            }
        }

        fall = Time.time;
    }

    IEnumerator DelayBeforeNextTetromino()
    {
        yield return new WaitForSeconds(0.5f); // Adjust the delay time

        FindObjectOfType<Game>().DeleteRow();

        if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
        {
            FindObjectOfType<Game>().GameOver();
        }
 
        FindObjectOfType<Game>().SpawnNextTetromino();

        Game.currentScore += individualScore;

        enabled = false;
        tag = "Untagged";

        FindObjectOfType<Game>().UpdateHighScore();

        isPlaced = false; // Reset the flag for the next Tetromino
    }

    void RotateLeft()
    {

        if (allowRotation)
        {
            if (limitRotation)
            {

                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {

                    transform.Rotate(0, 0, 90);
                }

            }
            else
            {

                transform.Rotate(0, 0, 90);
            }

            if (CheckIsInValidPosition())
            {

                FindObjectOfType<Game>().UpdateGrid(this);

            }
            else
            {

                if (limitRotation)
                {

                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.position += new Vector3(0, 0, -90);
                    }

                }
                else
                {

                    transform.Rotate(0, 0, -90);
                }
            }
        }
    }

    void RotateRight()
    {

        if (allowRotation)
        {
            if (limitRotation)
            {

                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }

            }
            else
            {

                transform.Rotate(0, 0, 90);
            }

            if (CheckIsInValidPosition())
            {

                FindObjectOfType<Game>().UpdateGrid(this);

            }
            else
            {

                if (limitRotation)
                {

                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.position += new Vector3(0, 0, -90);
                    }

                }
                else
                {

                    transform.Rotate(0, 0, -90);
                }
            }
        }
    }

    bool CheckIsInValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().round(mino.position);

            if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false)
            {
                return false;
            }

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            {

                return false;
            }
        }
        return true;
    }
}