using Photon.Pun;
using UnityEngine;

public class Tetramino : MonoBehaviourPunCallbacks
{
    #region //// Fall Params ////
    private float fallSpeed;
    private float fallTime;
    #endregion
    #region //// Smooth Move Params ////
    private float continiousVerticalSpeed = 0.05f;
    private float continiousHorizontalSpeed = 0.1f;
    private float buttonDownWaitMax = 0.2f;

    private float verticalTimer = 0f;
    private float horizontalTimer = 0;
    private float buttonDownTimerHorizontal = 0;
    private float buttonDownTimerVertical = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;
    #endregion
    #region //// Rotation Params ////
    public Vector3 rotatePoint;
    public bool allowRotation = true;
    public bool limitRotation = false;
    #endregion

    // PhotonView ph;
    void Start()
    {
        fallTime = Time.time;
        //ph = GetComponent<PhotonView>();
        fallSpeed = GameObject.Find("GM").GetComponent<Game>().fallSpeed;
    }
    void Update()
    {
        //if (!ph.IsMine)
        //{ return; }
        CheckInputs();
    }
    public void CheckInputs()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            movedImmediateHorizontal = false;
            buttonDownTimerHorizontal = 0;
            horizontalTimer = 0;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            verticalTimer = 0;
            movedImmediateVertical = false;
            buttonDownTimerVertical = 0;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fallTime >= fallSpeed)
        {
            MoveDown();
        }

    }
    #region ==== Move Methods ====
    public void MoveLeft()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continiousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateHorizontal)
        { movedImmediateHorizontal = true; }

        horizontalTimer = 0;

        transform.position += new Vector3(-1f, 0f, 0f);
        if (isPositionValid())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
        }
        else transform.position += new Vector3(1f, 0f, 0f);
    }
    public void MoveRight()
    {

        
        if (movedImmediateHorizontal)
        {
            if (buttonDownTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continiousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateHorizontal)
        { movedImmediateHorizontal = true; }

        horizontalTimer = 0;

        transform.position += new Vector3(1f, 0f, 0f);
        if (isPositionValid())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
        }
        else transform.position += new Vector3(-1f, 0f, 0f);
    }
    public void MoveDown()
    {

        if (movedImmediateVertical)
        {
            if (buttonDownTimerVertical < buttonDownWaitMax)
            {
                buttonDownTimerVertical += Time.deltaTime;
                return;
            }
            if (verticalTimer < continiousVerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }
        if (!movedImmediateVertical)
        { movedImmediateVertical = true; }
        verticalTimer = 0;

        transform.position += new Vector3(0f, -1f, 0f);
        if (isPositionValid())
        {
            FindObjectOfType<Game>().UpdateGrid(this);

        }
        else
        {
            transform.position += new Vector3(0f, 1f, 0f);
            FindObjectOfType<Game>().DeleteRow();
            if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Game>().LoadGameOver();


            }
            enabled = false;

            //FindObjectOfType<Game>().SpawnNextTetromino();
            FindObjectOfType<Game>().SpawnNextTetroFromList();
        }
        fallTime = Time.time;
    }
    public void Rotate()
    {
        if (allowRotation)
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), -90f);
                }
                else { transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), 90f); }
            }
            else
            {
                transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), 90f);

            }

        }
        if (isPositionValid())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
        }
        else
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                { transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), -90f); }
                else
                { transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), 90f); }
            }
            else { transform.RotateAround(transform.TransformPoint(rotatePoint), new Vector3(0f, 0f, 1f), -90f); }
        }
    }

    bool isPositionValid()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Rounding(mino.position /*- FindObjectOfType<Game>().parent.transform.position*/);
            if (!FindObjectOfType<Game>().IsInsideGrid(pos))
            {
                return false;
            }


            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            { return false; }
        }
        return true;

    }

    #endregion
    
    
}
