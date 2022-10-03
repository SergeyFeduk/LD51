using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    ApproachingGrid,
    Approaching,
    Attacking,
    Waiting
}

public class EnemyAI : MonoBehaviour
{
    private const float sqrMinDistance = 0.01f;
    private const float MinSearchDistance = 0.05f;

    [SerializeField] private bool drawGizmos = false;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float cellWanderRadius;
    [Header("Attack")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackAnticipationTime;

    [Header("Animation")]
    [SerializeField] private float anticipationTime;
    [SerializeField] private float randomAnticipationTime;
    [SerializeField] private float jumpHorizontalSpeed;
    [SerializeField] private float jumpVerticalAcceleration;
    [SerializeField] private AnimationCurve jumpCurve;
    [Header("Imports")]
    [SerializeField] private float pitchVariance;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private EnemyAnimator animator;

    private Pathfinder pathfinder = new Pathfinder();
    public State currentState;
    private List<Vector2Int> path = new List<Vector2Int>();
    private List<Vector2> pathRandomness = new List<Vector2>();
    private int currentPathIndex = 0;
    private Timer attackTimer = new Timer();
    private int targetIndex = 0;
    [HideInInspector]public bool isJumping = false;

    public void RegeneratePath() {
        if (currentState == State.ApproachingGrid) return;
        path = GeneratePath(TileManager.instance.GetTiles().GetCellAtWorld(transform.position), TileManager.instance.GetTiles().GetCellAtWorld(Player.instance.transform.position));
        currentPathIndex = 0;
        if (path.Count <= 0) {
            SwitchToState(State.Waiting);
            return;
        }
        SwitchToState(State.Approaching);
    }

    private void Update()
    {
        //There goes mess
        if (path.Count <= 0) { SwitchToState(State.Waiting); }
        //if (path.Count <= 0 && TileManager.instance.GetTiles().isPositionValid(TileManager.instance.GetTiles().GetCellAtWorld(transform.position)) && TileManager.instance.GetTiles().GetValueAtWorld(transform.position).isWarned) { RegeneratePath(); print("reg"); return; }
        switch (currentState) {
            case State.ApproachingGrid:
                break;
            case State.Approaching:
                if (Vector2.SqrMagnitude(Player.instance.transform.position - transform.position) < attackDistance - sqrMinDistance) return;/*SwitchToState(State.Attacking);*/
                if (path.Count <= currentPathIndex + 1 || path.Count <= 0) {
                    if (Vector2.SqrMagnitude(Player.instance.transform.position - transform.position) > attackDistance - sqrMinDistance) RegeneratePath();
                    //Last node reached, wait for update
                }
                targetIndex = 0;

                for (int i = 0; i < 2; i++) {
                    if (path.Count - 1 > currentPathIndex + i)
                    {
                        targetIndex++;
                    }
                    else {
                        break;
                    }
                }
                Vector3 targetPosition = pathRandomness[currentPathIndex + targetIndex] + TileManager.instance.GetTiles().GetWorldPosition(path[currentPathIndex + targetIndex]) - new Vector2(0.5f, 0.5f);
                float sqrDistanceToNextNode = Vector2.SqrMagnitude(transform.position - targetPosition);
                if (sqrDistanceToNextNode < sqrMinDistance) currentPathIndex += targetIndex;
                //Evaluate path
                JumpTo(targetPosition);

                break;
                
            case State.Attacking:
                //Obsolete
                if (Vector2.SqrMagnitude(Player.instance.transform.position - transform.position) > attackDistance + sqrMinDistance) SwitchToState(State.Approaching);
                if (attackTimer.ExecuteTimer()) {
                    //Attack();
                    attackTimer.SetTimer(attackCooldown);
                }
                break;
            case State.Waiting:
                break;
        }
    }

    private void Start()
    {
        RegeneratePath();
        SwitchToState(State.ApproachingGrid);
    }

    private IEnumerator ApproachGridRoutine() {
        //Check if below or above
        bool isAbove = transform.position.y - TileManager.instance.GetTiles().GetWorldPosition(0, 0).y > 0;
        int layerY = isAbove ? TileManager.instance.GetTiles().GetSize().y - 1 : 0;
        //Check top or bottom layer of grid to find walkable tile. iF there is no - wait
        Vector2 targetPosition;
        Vector2? targetTestPosition;

        bool isRight = transform.position.x - TileManager.instance.GetTiles().GetWorldPosition(0, 0).x > 0;
        while (true) {
            targetTestPosition = ScanOnGridApproach(layerY, isRight);
            if (targetTestPosition != null) {
                targetPosition = (Vector2)targetTestPosition - new Vector2(0.5f,0.5f);
                break;
            }
            yield return null;
        }

        //Go to this tile
        int directionSign = transform.position.x - targetPosition.x > 0 ? -1 : 1;
        JumpTo(targetPosition);
        while (isJumping) {
            yield return null;
        }
        /*while (Mathf.Abs(transform.position.x - targetPosition.x) > MinSearchDistance) {
            transform.position += Vector3.right * directionSign * movementSpeed * Time.deltaTime;
            yield return null;
        }*/
        SwitchToState(State.Approaching);
        RegeneratePath();
    }

    private void JumpTo(Vector3 position) {
        if (isJumping) return;
        isJumping = true;
        Vector3 normDir = (transform.position - position).normalized;
        StartCoroutine(JumpRoutine(normDir, position));
    }

    private IEnumerator JumpRoutine(Vector3 normDir, Vector3 targetPosition) {
        int dx = normDir.x > 0 ? 1 : -1;
        if (Mathf.Abs(normDir.x) < 0.1f) dx = 0;
        int dy = normDir.y > 0 ? 1 : -1;
        //Anticipation
        SpriteDirData data = animator.GetDirByDelta(dx,dy);
        data.index = 0;
        animator.SetSprite(data);
        Timer timer = new Timer(anticipationTime + (Random.value * 2 - 1) * randomAnticipationTime);
        while (!timer.ExecuteTimer()) {
            //Animate anticipation
            yield return null;
        }
        // actual jump
        data.index = 1;
        animator.SetSprite(data);
        float jumpTime = Vector3.Distance(transform.position, targetPosition) / jumpHorizontalSpeed;
        jumpTime = Mathf.Clamp(jumpTime, 0.75f, 1.25f);
        timer = new Timer(jumpTime);
        Vector3 initialPosition = transform.position;
        while (!timer.ExecuteTimer())
        {
            //Animate jump
            animator.SetRendererTransform(new Vector3(0,Mathf.Lerp(0, jumpVerticalAcceleration, jumpCurve.Evaluate(timer.GetTimeLeft() / jumpTime)),0));
            transform.position = Vector3.Lerp(targetPosition, initialPosition, timer.GetTimeLeft() / jumpTime);
            yield return null;
        }
        data.index = 0;
        animator.SetSprite(data);
        isJumping = false;

        if (Vector3.Distance(transform.position, Player.instance.transform.position) < attackDistance) {
            Player.instance.heath.Damage(1);
        }
    }

    private Vector2? ScanOnGridApproach(int layerY, bool reverse) {
        if (reverse)
        {
            for (int x = TileManager.instance.GetTiles().GetSize().x - 1; x > 0; x--)
            {
                if (TileManager.instance.GetTiles().GetValueAt(x, layerY).isWalkable)
                {
                    return TileManager.instance.GetTiles().GetWorldPosition(x, layerY);
                }
            }
        }
        else
        {
            for (int x = 0; x < TileManager.instance.GetTiles().GetSize().x; x++)
            {
                if (TileManager.instance.GetTiles().GetValueAt(x, layerY).isWalkable)
                {
                    return TileManager.instance.GetTiles().GetWorldPosition(x, layerY);
                }
            }
        }
        return null;
    }

    private void SwitchToState(State newState) {
        currentState = newState;
        switch (currentState)
        {
            case State.ApproachingGrid:
                StartCoroutine(ApproachGridRoutine());
                break;
            case State.Approaching:
                break;
            case State.Attacking:
                currentPathIndex = path.Count + 1;
                break;
            case State.Waiting:
                break;
        }
    }

    private List<Vector2Int> GeneratePath(Vector2Int startingPosition, Vector2Int endingPosition)
    {
        Vector2Int initialStartingPosition = startingPosition;
        Grid<Tile> tileGrid = TileManager.instance.GetTiles();
        Grid<bool> grid = new Grid<bool>(tileGrid.GetSize().x, tileGrid.GetSize().y, tileGrid.GetCellSize());
        for (int x = 0; x < grid.GetSize().x; x++)
        {
            for (int y = 0; y < grid.GetSize().y; y++)
            {
                grid.SetValueAt(x, y, !tileGrid.GetValueAt(x, y).isWarned);
            }
        }

        //Find closest walkable cell for start
        List<Vector2Int> cellsToLookUp = new List<Vector2Int>();
        List<Vector2Int> cellsDone = new List<Vector2Int>();
        int searchIteration = tileGrid.GetSize().x * tileGrid.GetSize().y - 1;
        while (!grid.GetValueAt(startingPosition) && searchIteration > 0) {
            searchIteration--;
            List<Vector2Int> listToAdd = grid.Get4NeighboursPositions(startingPosition);
            for (int i = 0; i < listToAdd.Count; i++) {
                if (!cellsDone.Contains(listToAdd[i])) {
                    cellsToLookUp.Add(listToAdd[i]);
                }
            }
            if (cellsToLookUp[0] == null) break;
            startingPosition = cellsToLookUp[0];
            cellsDone.Add(cellsToLookUp[0]);
            cellsToLookUp.RemoveAt(0);
        }
        //Find closest walkable cell for end
        cellsToLookUp = new List<Vector2Int>();
        cellsDone = new List<Vector2Int>();
        searchIteration = tileGrid.GetSize().x * tileGrid.GetSize().y - 1;
        while (!grid.GetValueAt(endingPosition) && searchIteration > 0)
        {
            searchIteration--;
            List<Vector2Int> listToAdd = grid.Get4NeighboursPositions(endingPosition);
            for (int i = 0; i < listToAdd.Count; i++)
            {
                if (!cellsDone.Contains(listToAdd[i]))
                {
                    cellsToLookUp.Add(listToAdd[i]);
                }
            }
            if (cellsToLookUp[0] == null) break;
            endingPosition = cellsToLookUp[0];
            cellsDone.Add(cellsToLookUp[0]);
            cellsToLookUp.RemoveAt(0);
        }

        pathfinder.SetGrid(grid);
        List<Vector2Int> path = pathfinder.FindPath(startingPosition, endingPosition);
        pathRandomness = new List<Vector2>();
        for (int i = 0; i < path.Count; i++) {
            pathRandomness.Add(new Vector2(Random.value * 2 - 1, Random.value * 2 - 1) * cellWanderRadius);
        }
        if (path.Count <= 0) {
            path = new List<Vector2Int>() { initialStartingPosition , startingPosition };
            pathRandomness.Add(new Vector2(Random.value * 2 - 1, Random.value * 2 - 1) * cellWanderRadius);
            pathRandomness.Add(new Vector2(Random.value * 2 - 1, Random.value * 2 - 1) * cellWanderRadius);
        }
        return path;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || path.Count <= 0) return;
        for (int i = 0; i < path.Count - 1; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(TileManager.instance.GetTiles().GetWorldPosition(path[i]) - Vector2.one * 0.5f, TileManager.instance.GetTiles().GetWorldPosition(path[i+1]) - Vector2.one * 0.5f);
        }
    }

    
}
