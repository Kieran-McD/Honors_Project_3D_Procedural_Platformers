using UnityEngine;

public class PerlinWormPath : PerlinWorm
{
    public bool isMoving = true;

    public bool finalPoint;

    public PathGenerator pathGenerator;

    public LevelGeneratorVoronoi levelGeneratorVoronoi;

    public Transform followPoint;


    public new void Awake()
    {
        base.Awake();
    }

    //public new void Start()
    //{
    //    //levelGeneratorVoronoi = FindAnyObjectByType<LevelGeneratorVoronoi>();
    //    //base.Start();
    //}

    public new void Update()
    {
        if (isMoving)
        {
            base.Update();
            //MagnetizeToPoint();
        }    
    }

    private void MagnetizeToPoint()
    {
        Vector3 directionToPoint = followPoint.position - transform.position;
        directionToPoint.Normalize();
        //directionToPoint = new Vector3(0, 1, 0);
        //Horizontal Direction
        float dotProduct = Vector3.Dot(transform.forward, directionToPoint);
        float angleToPoint = Mathf.Acos(dotProduct / (transform.forward.magnitude * directionToPoint.magnitude)) / 3.14159f;
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, directionToPoint, angleToPoint * 45f * Time.deltaTime, 0f));

        //Quaternion toRotation = Quaternion.LookRotation(directionToPoint); // instead of LookRotation( )
        //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, angleToPoint * 30f * Time.deltaTime);

        Debug.Log("Direction to target: " + directionToPoint);

        //Debug.Log("Angle to point: " + Mathf.Rad2Deg * angleToPoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HITTT");
        if (!pathGenerator) return;
        if(other.TryGetComponent<PathNode>(out PathNode pathNode)){

            if (other.transform != followPoint) return;

            if (pathNode.isObstacle)
            {
                pathGenerator.PerlinWormFinished();
                followPoint = pathNode.NextNode.transform;
                return;
            }


            if (!pathNode.NextNode)
            {
                levelGeneratorVoronoi.isGenerationFinished = true;
                pathGenerator.PerlinWormFinished();
                
            }
            else
            {
                followPoint = pathNode.NextNode.transform;
            }
        }
    }

}
