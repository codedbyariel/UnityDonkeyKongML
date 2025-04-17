using UnityEngine;
using UnityEditor;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Collections;
using System.Drawing;


public class PlayerController : Agent
{
    private float horizontal, vertical;
    private float speed = 2f;
    private float jumpingPower = 3.2f;
    private bool isFacingRight = true;
    private bool isLadder;
    private bool isClimbing;
    private int tempScore;
    private bool isHammerHitBarrel;
    public float targetTime;
    private bool isEnemy = false;
    private bool isWall = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform playerPOS;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckAir;
    [SerializeField] private Transform barrelCheck;
    [SerializeField] private LayerMask barrelLayer;
    [SerializeField] private Transform HammerHitBarrel;
    [SerializeField] private LayerMask HammerLayer;
    [SerializeField] private float HammerTimer;
    [SerializeField] private LayerMask FireEnemyLayer;
    [SerializeField] private int Score;
    [SerializeField] private UnityEngine.UI.Text ScoreText;
    [SerializeField] private LayerMask LadderLayer;
    //MlAgent Inputs
    private float moveX = 0;
    private float moveY = 0;
    private float jump = 0;
    [SerializeField] private Transform Target;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float maxYPlayer;
    [SerializeField] private GameObject movementXsquare;
    [SerializeField] private GameObject movementYsquare;
    [SerializeField] private GameObject movementJumpsquare;
    [SerializeField] private GameObject Restarter;
    [SerializeField] private string SceneName;
     private Vector3 StartplayerPOS;
    [SerializeField] private GameObject Hammer1;
    [SerializeField] private GameObject Hammer2;
    [SerializeField] private float radius;
    //
    //
    [SerializeField] private Transform sceneOrigin;
    [SerializeField] private Transform sceneOrigin2;
    [SerializeField] private float minDistance;




    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(playerPOS.position, 500f);
        foreach (Collider col in colliders)
        {
            Debug.Log("Detected: " + col.gameObject.name + " in layer: " + col.gameObject.layer);
        }
        maxYPlayer = ConvertToLocalPosition(playerPOS.position).y;
        StartplayerPOS = playerPOS.position;

        /*Scene targetScene = SceneManager.GetSceneByName(SceneName);

        // Check if the scene is valid and loaded
        if (targetScene.IsValid() && targetScene.isLoaded)
        {
            // Find all GameObjects in the target scene
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

            // Iterate through all objects and destroy those named "Barrel(Clone)"
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "Barrel(Clone)" && obj.scene == targetScene)
                {
                    Destroy(obj);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Target scene '{SceneName}' is not loaded or invalid.");
        }*/
    }



    void Update()
    {
        HandleInput();
        HandleJump();
        ItsHammerTime();
        Flip();
        UpdateScore();
        HammerHitBarrelFunc();
        stopHammerTime();
        if ((HasJumpedOverBarrel()) && (!IsGrounded()))
        {
            tempScore++;
        }
        if (isHammerHitBarrel)
        {
            targetTime -= Time.deltaTime;
        }
        //EndGame();
        HandleLadder();
        GiveReward();
        changeColorOnMovement();
        stopHammerTime();
        //Debug.Log("playerPOS:"+ ConvertToLocalPosition(transform.position));
        //Debug.Log(ClosestObjectByLayer(barrelLayer, playerPOS.position).transform.position);
        //Debug.Log(changeY());
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode begin");
        Score = 0;
        tempScore = 0;
        targetTime = HammerTimer;
        Restart();
        maxYPlayer = 0;// ConvertToLocalPosition(StartplayerPOS).y;
        isEnemy = false;
        isHammerHitBarrel = false;
        Hammer1.SetActive(true);
        Hammer2.SetActive(true);
        StartCoroutine(TimerCoroutine());
        minDistance= Vector2.Distance((Vector2)ConvertToLocalPosition(transform.position), (Vector2)ConvertToLocalPosition(Target.position));


        //Restarter.GetComponent<SpriteRenderer>().material.color = Color.white;


    }
    IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Restarter.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.white;
            //Debug.Log("One second passed!");
        }
    }



    public override void OnActionReceived(ActionBuffers actions)
    {
        //stationary
        if (actions.DiscreteActions[0] == 0)
        {
            moveX = 0;
            moveY = 0;
        }
        //move x
        if(actions.DiscreteActions[0] == 1)
        {
            moveX = 1;
            moveY = 0;
        }
        if (actions.DiscreteActions[0] == 2)
        {
            moveX = -1;
            moveY = 0;
        }
        //move y
        if (actions.DiscreteActions[0] == 3)
        {
            moveY = 1;
            moveX = 0;
        }
        if (actions.DiscreteActions[0] == 4)
        {
            moveY = -1;
            moveX = 0;
        }
        // jump
        if (actions.DiscreteActions[1] == 0){
            jump = 0;
        }
        if (actions.DiscreteActions[1] == 1)
        {
            jump = 1;
        }
        //moveX = actions.ContinuousActions[0];
        //moveY = actions.ContinuousActions[1];
        //jump = actions.ContinuousActions[2];
    }
    //
    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(IsSomethingNear(barrelLayer));
        //sensor.AddObservation(IsSomethingNear(FireEnemyLayer));
        sensor.AddObservation(IsSomethingNear(LadderLayer));
        //Staying
        sensor.AddObservation(NormalizeY(sceneOrigin.position, sceneOrigin2.position, transform.position));
        sensor.AddObservation(NormalizeX(sceneOrigin.position, sceneOrigin2.position, transform.position));

        sensor.AddObservation(GetYDistance(transform, ClosestObjectByLayer(FireEnemyLayer, transform.position).transform) / 11.4f);
        sensor.AddObservation(GetXDistance(transform, ClosestObjectByLayer(FireEnemyLayer, transform.position).transform) / 10.1f);

        sensor.AddObservation(GetYDistance(transform, ClosestObjectByLayer(barrelLayer, transform.position).transform)/11.4f);
        sensor.AddObservation(GetXDistance(transform, ClosestObjectByLayer(barrelLayer, transform.position).transform)/10.1f);

        sensor.AddObservation(isHammerHitBarrel ? 1 : 0);
        //Staying
        //sensor.AddObservation(NormalizeX(sceneOrigin.position, sceneOrigin2.position, GetXYDistance(transform, ClosestObjectByLayer(LadderLayer, playerPOS.position).transform)));
        //sensor.AddObservation(NormalizeY(sceneOrigin.position, sceneOrigin2.position, GetXYDistance(transform, ClosestObjectByLayer(LadderLayer, playerPOS.position).transform)));



        //sensor.AddObservation(GetClosestObjectAboveXDistance(transform.position,500f,LadderLayer) < 0 ? -1 : 1);








        /*sensor.AddObservation((Vector2)ConvertToLocalPosition(transform.position));
        sensor.AddObservation(GetXYDistance(transform, Target));//distance from target
        //sensor.AddObservation(Vector2.Distance((Vector2)transform.position, (Vector2)ClosestObjectByLayer(FireEnemyLayer, playerPOS.position).transform.position));//fireEnemy Distance
        sensor.AddObservation(GetXYDistance(transform, ClosestObjectByLayer(FireEnemyLayer, playerPOS.position).transform));
        sensor.AddObservation(GetXYDistance(transform, ClosestObjectByLayer(barrelLayer, playerPOS.position).transform));//distance from target
        //sensor.AddObservation(Vector2.Distance((Vector2)transform.position, (Vector2)ClosestObjectByLayer(barrelLayer, playerPOS.position).transform.position));//barrel Distance
        /*sensor.AddObservation((Vector2)ConvertToLocalPosition(Target.position));
        sensor.AddObservation(Vector2.Distance((Vector2)ConvertToLocalPosition(transform.position), (Vector2)ConvertToLocalPosition(Target.position)));
        sensor.AddObservation((Vector2)ConvertToLocalPosition(ClosestObjectByLayer(FireEnemyLayer, playerPOS.position).transform.position));
        sensor.AddObservation(Vector2.Distance((Vector2)transform.position, (Vector2)ClosestObjectByLayer(FireEnemyLayer, playerPOS.position).transform.position));//fireEnemy Distance
        sensor.AddObservation((Vector2)ConvertToLocalPosition(ClosestObjectByLayer(HammerLayer, playerPOS.position).transform.position));
        sensor.AddObservation((Vector2)ConvertToLocalPosition(ClosestObjectByLayer(barrelLayer, playerPOS.position).transform.position));
        sensor.AddObservation(Vector2.Distance((Vector2)transform.position, (Vector2)ClosestObjectByLayer(barrelLayer, playerPOS.position).transform.position));//barrel Distance
        sensor.AddObservation((Vector2)ConvertToLocalPosition(GameObject.Find("ladder").transform.position));//ladder 0
        sensor.AddObservation((Vector2)ConvertToLocalPosition(GameObject.Find("ladder (1)").transform.position));//ladder 1
        sensor.AddObservation((Vector2)ConvertToLocalPosition(GameObject.Find("ladder (2)").transform.position));//ladder 2
        sensor.AddObservation((Vector2)ConvertToLocalPosition(GameObject.Find("ladder (3)").transform.position));//ladder 3
        sensor.AddObservation((Vector2)ConvertToLocalPosition(GameObject.Find("ladder (4)").transform.position));//ladder 4
        sensor.AddObservation((Vector2)ConvertToLocalPosition(GameObject.Find("ladder (5)").transform.position));//ladder 5
        sensor.AddObservation((Vector2)ConvertToLocalPosition(GameObject.Find("ladder (6)").transform.position));//ladder 6
        sensor.AddObservation((Vector2)ConvertToLocalPosition(GameObject.Find("ladder (7)").transform.position));//ladder 7
        sensor.AddObservation((Vector2)ConvertToLocalPosition(GameObject.Find("ladder (9)").transform.position));//ladder 9
        sensor.AddObservation(isHammerHitBarrel ? 1 : 0);//if hammer timer is big than 1 if not than 0*///changing*/


    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        // Movement actions
        if (Input.GetAxisRaw("Horizontal") > 0)
            discreteActions[0] = 1; // Move right
        else if (Input.GetAxisRaw("Horizontal") < 0)
            discreteActions[0] = 2; // Move left
        else if (Input.GetAxisRaw("Vertical") > 0)
            discreteActions[0] = 3; // Move up
        else if (Input.GetAxisRaw("Vertical") < 0)
            discreteActions[0] = 4; // Move down
        else
            discreteActions[0] = 0; // No movement

        // Jump action
        discreteActions[1] = Input.GetButton("Jump") ? 1 : 0;
    }


    private void RewardDistanceFromTarget()
    {
        float distance=Vector2.Distance((Vector2)ConvertToLocalPosition(transform.position), (Vector2)ConvertToLocalPosition(Target.position));
        if (minDistance > distance)
        {
            minDistance = distance;
            AddReward(4);
        }
        else
        {
            AddReward(-1);
        }
    }
    private static float GetXDistance(Transform origin, Transform target)
    {
        return target.position.x - origin.position.x;
    }

    // Returns the Y distance (positive if target is above, negative if below)
    private static float GetYDistance(Transform origin, Transform target)
    {
        return target.position.y - origin.position.y;
    }

    // Returns both X and Y distances as a Vector2
    private static Vector2 GetXYDistance(Transform origin, Transform target)
    {
        return new Vector2(GetXDistance(origin, target), GetYDistance(origin, target));
    }
    private void GiveReward()
    {
        RewardDistanceFromTarget();
        if (Physics2D.OverlapCircle(transform.position, 0.6f, targetLayer))
        {
            AddReward(1000);
            if (Restarter.GetComponent<SpriteRenderer>().material.color == UnityEngine.Color.white)
            {
                Restarter.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.green;

            }
            EndEpisode();
            Debug.Log("endedEpisode");
            
        }
        if (isEnemy)
        {
            Debug.Log("here");
            AddReward(-100);
            if (Restarter.GetComponent<SpriteRenderer>().material.color == UnityEngine.Color.white)
            {
                Restarter.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.green;
            }
            EndEpisode();
            Debug.Log("endedEpisode");
        }
        if (tempScore != 0 && IsGrounded())
        {
            AddReward(15);
            tempScore = 0;
        }
        if (changeY())
        {
            AddReward(30);
        }
        else
        {
            AddReward(-5);//* (Mathf.Abs(maxYPlayer)- Mathf.Abs(ConvertToLocalPosition(transform.position).y)));
        }
        if (isClimbing)
        {
            AddReward(5);
        }
        //mlagents-learn E:\ConfigMLAgent\configuration.yaml --run-id=testingNeroLarge2
    }

    public float NormalizeY(Vector2 worldOrigin, Vector2 maxPoint, Vector2 playerPosition)
    {
        // Get the min and max Y values
        float minY = worldOrigin.y;
        float maxY = maxPoint.y;

        // Clamp the player's Y position to avoid values outside the range
        float clampedY = Mathf.Clamp(playerPosition.y, minY, maxY);

        // Normalize the Y position to a range between 0 and 1
        return (clampedY - minY) / (maxY - minY);
    }

    public float NormalizeX(Vector2 worldOrigin, Vector2 maxPoint, Vector2 playerPosition)
    {
        // Get the min and max X values
        float minX = worldOrigin.x;
        float maxX = maxPoint.x;

        // Clamp the player's X position to avoid values outside the range
        float clampedX = Mathf.Clamp(playerPosition.x, minX, maxX);

        // Normalize the X position to a range between 0 and 1
        return (clampedX - minX) / (maxX - minX);
    }

    //
    public float GetClosestObjectAboveXDistance(Vector2 playerPosition, float searchRadius, LayerMask layerMask)
    {
        // Find all colliders in the given search radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerPosition, searchRadius, layerMask);

        Collider2D closestObject = null;
        float closestYDistance = Mathf.Infinity;

        // Iterate through all detected objects
        foreach (Collider2D collider in colliders)
        {
            float objectY = collider.transform.position.y;

            // Only consider objects ABOVE the player
            if (objectY > playerPosition.y)
            {
                float yDistance = objectY - playerPosition.y; // No need for Abs() since we know it's above

                // Find the closest object in the Y-axis
                if (yDistance < closestYDistance)
                {
                    closestYDistance = yDistance;
                    closestObject = collider;
                }
            }
        }

        // If no object was found above the player, return NaN
        if (closestObject == null)
            return float.NaN;

        // Return the X-axis distance to the closest object above
        return closestObject.transform.position.x - playerPosition.x;
    }






    ////


    private int IsSomethingNear(LayerMask layer)
    {
        DrawCircle(transform.position, radius, 6, UnityEngine.Color.red);
        if (Physics2D.OverlapCircle(transform.position, radius, layer))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    void DrawCircle(Vector2 center, float radius, float segments, UnityEngine.Color color)
    {
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angleA = Mathf.Deg2Rad * (i * angleStep);
            float angleB = Mathf.Deg2Rad * ((i + 1) * angleStep);

            Vector2 pointA = center + new Vector2(Mathf.Cos(angleA), Mathf.Sin(angleA)) * radius;
            Vector2 pointB = center + new Vector2(Mathf.Cos(angleB), Mathf.Sin(angleB)) * radius;

            Debug.DrawLine(pointA, pointB, color);
        }
    }
    //
    private void FixedUpdate()
    {
        HandleMovement();
        float Hammer = 1;
        if (isHammerHitBarrel)
        {
            Hammer = 0;
        }
        else
        {
            Hammer = 1;
        }

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * speed * Hammer);
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void HandleInput()
    {
        horizontal = moveX; //Input.GetAxisRaw("Horizontal");
        vertical = moveY;//Input.GetAxisRaw("Vertical");
    }

    private void HandleMovement()
    {
        float Hammer = 1;
        if (isHammerHitBarrel)
        {
            Hammer = 0;
        }
        else
        {
            Hammer = 1;
        }
        
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * speed * Hammer);
        }
        if (!isWall) // Check if player is not colliding with a wall
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
        else if (horizontal != 0) // Apply a small force to slide
        {
            rb.linearVelocity = new Vector2(horizontal * speed * 0.0f, rb.linearVelocity.y);
        }
    }

    private void HandleJump()
    {
        float Hammer = 1;
        if (isHammerHitBarrel)
        {
            Hammer = 0;
        }
        else
        {
            Hammer = 1;
        }
        if (jump>0/*Input.GetButtonDown("Jump")*/ && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower*Hammer);
        }

        if (jump <= 0/*Input.GetButtonUp("Jump")*/ && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f * Hammer);
        }
    }

    private bool IsGrounded()
    {
        if((Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer))&&(!Physics2D.OverlapCircle(groundCheckAir.position, 0.2f, groundLayer)))
        {
            return true;
        }
        else
        {
            return false;
        }
        //return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool HasJumpedOverBarrel()
    {
        return Physics2D.OverlapCircle(barrelCheck.position, 0.2f, barrelLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal > 0f || !isFacingRight && horizontal < 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void HammerHitBarrelFunc()
    {
        if (Physics2D.OverlapCircle(HammerHitBarrel.position, 0.2f, barrelLayer) && isHammerHitBarrel)
        {
            Destroy(Physics2D.OverlapCircle(HammerHitBarrel.position, 0.2f, barrelLayer).gameObject);
            AddReward(10);
            Score += 5;
        }
    }

    private void ItsHammerTime()
    {
        if (Physics2D.OverlapCircle(rb.position, 0.3f, HammerLayer))
        {
            isHammerHitBarrel = true;
            Physics2D.OverlapCircle(rb.position, 0.3f, HammerLayer).gameObject.SetActive(false);
        }
    }

    private void stopHammerTime()
    {
        if (targetTime <= 0.0f)
        {
            isHammerHitBarrel = false;
            targetTime = HammerTimer;
        }
    }

    private void UpdateScore()
    {
        if (HasJumpedOverBarrel() && !IsGrounded())
        {
            tempScore++;
        }
        if (tempScore != 0 && IsGrounded())
        {
            Score++;
            //tempScore = 0;
        }
        ScoreText.text = "Score: " + (Score * 100).ToString();
    }

    private void EndGame()
    {
        if (Physics2D.OverlapCircle(playerPOS.position, 0.2f, barrelLayer))
        {
            EditorApplication.isPlaying = false;
        }
    }

    private void HandleLadder()
    {
        vertical = moveY;//Input.GetAxisRaw("Vertical");

        if (isLadder && Mathf.Abs(moveY) > 0f)
        {
            isClimbing = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collided with: " + collision.name);
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
        if (collision.CompareTag("Barrel") || (collision.CompareTag("FireEnemy")))
        {
            isEnemy = true;
            //Debug.Log("Barrel detected.");
        }
        /*if (collision.CompareTag("Wall"))
        {
            isWall = true;
            Debug.Log("exit wall");
        }*/

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) // Check if the object has the tag "Wall"
        {
            isWall = true;
            /*foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > 0.1f) // Ensure it's a side collision, not ground or ceiling
                {
                    isWall = true;
                    return;
                }
            }*/
        }
        else
        {
            isWall = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWall = false;
            Debug.Log("exit wall");
        }
    }
    //
    GameObject ClosestObjectByLayer(LayerMask layerMask, Vector3 position)
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject closestObject = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject obj in allObjects)
        {
            if (((1 << obj.layer) & layerMask) != 0) // Correct way to check LayerMask
            {
                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestObject = obj;
                }
            }
        }

        return closestObject;
    }
    //
    private bool changeY()
    {
        if (ConvertToLocalPosition(transform.position).y > maxYPlayer)
        {
            maxYPlayer = ConvertToLocalPosition(transform.position).y;
            return true;
            
        }
        else
        {
            return false;
        }
        
        
    }
    private void changeColorOnMovement()
    {
        if (moveX == 0)
        {
            movementXsquare.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.white;
        }
        if (moveX > 0)
        {
            movementXsquare.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.green;
        }
        if (moveX < 0)
        {
            movementXsquare.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.red;
        }
        if (moveY == 0) {
            movementYsquare.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.white;
        }
        if (moveY > 0)
        {
            movementYsquare.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.green;
        }
        if (moveY < 0)
        {
            movementYsquare.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.red;
        }
        if (jump == 0)
        {
            movementJumpsquare.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.white;
        }
        if (jump > 0)
        {
            movementJumpsquare.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.green;
        }
        if (jump < 0)
        {
            movementJumpsquare.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.red;
        }

    }
    private void Restart()
    {
        playerPOS.position = StartplayerPOS;

    }
    //
    //
    //
    //
    //
    //
    //
    public Vector3 ConvertToLocalPosition(Vector3 worldPosition)
    {
        if (sceneOrigin != null)
        {
            // Convert world position to local position relative to SceneOrigin
            Vector3 localPosition = sceneOrigin.InverseTransformPoint(worldPosition);
            //Debug.Log("World Position: " + worldPosition + " -> Local Position: " + localPosition);
            return localPosition;
        }
        else
        {
            Debug.LogError("SceneOrigin is not assigned!");
            return worldPosition;  // Return the original world position if no sceneOrigin
        }
    }


}

