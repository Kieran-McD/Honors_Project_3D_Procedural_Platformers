using UnityEngine;

public class PerlinWormPath : PerlinWorm
{
    public bool isMoving = true;

    public PathGenerator pathGenerator;

    public Transform followPoint;

    public new void Update()
    {
        if (isMoving)
        {
            MagnetizeToPoint();

            base.Update();

            if(transform.position == followPoint.position)
            {
                pathGenerator.PerlinWormFinished();
            }

        }    
    }

    private void MagnetizeToPoint()
    {
        Vector3 directionToPoint = followPoint.position - transform.position; 

        float dotProduct = Vector3.Dot(transform.forward, directionToPoint);
        float angleToPoint = Mathf.Acos(dotProduct / (transform.forward.magnitude * directionToPoint.magnitude)) / 3.14159f;

        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, directionToPoint, angleToPoint * 45f * Time.deltaTime, 0f));

        //Debug.Log("Angle to point: " + Mathf.Rad2Deg * angleToPoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HITTT");
        if (!pathGenerator) return;
        if(other.TryGetComponent<PathNode>(out PathNode pathNode)){

            if (other.transform != followPoint) return;

            if (!pathNode.NextNode)
            {
                pathGenerator.PerlinWormFinished();
            }
            else
            {
                followPoint = pathNode.NextNode.transform;
            }
        }
    }

}
