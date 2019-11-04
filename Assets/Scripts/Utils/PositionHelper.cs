using UnityEngine;
using UnityEditor;

public static class PositionHelper
{
    static public int ClosestCompare(Vector3 x, Vector3 y, Vector3 referencePosition)
    {
        float diff = Vector3.Distance(x, referencePosition) - Vector3.Distance(y, referencePosition);
        if (diff < 0)
            return -1;
        else if (diff > 0)
            return 1;
        else
            return 0;
    }


    /**
     * Check if a straight line between origin and target would be blocked by obstacle
     **/
    public static bool IsTargetObscured(Transform origin, Transform target, float obstacleCheckHeight, int obstacleMask)
    {
        Vector3 direction = new Vector3(target.position.x - origin.position.x, 0, target.position.z - origin.position.z);
        //Debug.DrawRay(new Vector3(origin.position.x, obstacleCheckHeight, origin.position.z), direction);
        return Physics.Raycast(new Vector3(origin.position.x, obstacleCheckHeight, origin.position.z), direction, direction.magnitude, obstacleMask);
    }
}