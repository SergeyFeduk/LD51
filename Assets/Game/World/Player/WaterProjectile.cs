using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticle {
    public Vector2 position;
    public Vector2 velocity;
    public ProjectileParticle(Vector2 position, Vector2 velocity) {
        this.velocity = velocity;
        this.position = position;
    }
}

public class WaterProjectile : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private EdgeCollider2D edgeCollider;
    [SerializeField] private float gravityForce;
    [SerializeField] private float directionalSpeed;
    [SerializeField] private int maxLength;
    [SerializeField] private float lifeTime;

    private List<ProjectileParticle> positions = new List<ProjectileParticle>();
    private List<Vector3> poses = new List<Vector3>();
    private Timer timer = new Timer();
    private bool timerIsOn = false;
    private Vector2 lastCollisionPosition = new Vector2();

    public void AddLength(Vector2 position, Vector2 velocityDirection) {
        if (positions.Count > maxLength) return;
        positions.Add(new ProjectileParticle(position, velocityDirection * directionalSpeed));
        poses.Add(position);
    }

    private void Update()
    {
        if (timerIsOn && timer.ExecuteTimer()) Destroy(gameObject);
        #region Movement
        for (int i = 0; i < positions.Count; i++) {
            positions[i].velocity += new Vector2(0, gravityForce * Time.deltaTime);
            positions[i].position += positions[i].velocity * Time.deltaTime;
            poses[i] = positions[i].position;
        }
        #endregion

        #region GenerateCollider
        edgeCollider.SetPoints(poses.Select(x => (Vector2)x).ToList());
        #endregion
        #region Render
        if (positions.Count < 2) return;
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(poses.ToArray());
        #endregion
    }

    public void Unparent() {
        timer = new Timer(lifeTime);
        timerIsOn = true;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            lastCollisionPosition = collision.transform.position;
            collision.GetComponent<Enemy>().Damage(1);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            lastCollisionPosition = new Vector2();
        }
     }

    private void CleanCollisionEnd() {
        for (int i = positions.Count - 1; i > 0; i--) {
            //if (i >= positions.Count) break;
            if (Vector2.SqrMagnitude(lastCollisionPosition - positions[i].position) < 2f)
            {
                positions.RemoveAt(i);
                poses.RemoveAt(i);
            }
            else {
                break;
            }
        }
    }
}
