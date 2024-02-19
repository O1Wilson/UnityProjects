using System.Collections;
using UnityEngine;

public class GhostTetromino : MonoBehaviour
{
    private Game gameInstance;
    private SpriteRenderer[] minoRenderers;
    private Vector3 newPosition;
    private Transform currentActiveTetrominoTransform;

    void Start()
    {
        tag = "currentGhostTetromino";
        gameInstance = FindObjectOfType<Game>();
        minoRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in minoRenderers)
        {
            renderer.color = new Color(1f, 1f, 1f, .2f);
        }

        currentActiveTetrominoTransform = gameInstance.GetCurrentActiveTetrominoTransform();
    }

    void Update()
    {
        FollowActiveTetromino();
        MoveDown();
    }

    void FollowActiveTetromino()
    {
        newPosition.x = currentActiveTetrominoTransform.position.x;
        newPosition.y = transform.position.y;
        newPosition.z = transform.position.z;

        transform.SetPositionAndRotation(newPosition, currentActiveTetrominoTransform.rotation);
    }

    void MoveDown()
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        if (!CheckIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
        }
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = gameInstance.Round(mino.position);

            if (!gameInstance.CheckIsInsideGrid(pos))
                return false;

            if (gameInstance.GetTransformAtGridPosition(pos) != null)
            {
                if (gameInstance.GetTransformAtGridPosition(pos).parent.tag == "currentActiveTetromino")
                    return true;

                if (gameInstance.GetTransformAtGridPosition(pos).parent != transform)
                    return false;
            }
        }

        return true;
    }
}