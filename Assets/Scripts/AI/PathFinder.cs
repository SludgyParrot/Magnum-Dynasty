using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField]
    private List<Transform> paths = new List<Transform>();

    [SerializeField, Space(5)]
    private Color pathColor = Color.white;

    [SerializeField, Space(5)]
    private Vector3 pathPointSize;

    public List<Transform> Paths => paths;

    private void OnDrawGizmos()
    {
        Gizmos.color = pathColor;

        for (int i = 0; i < paths.Count; i++)
        {
            var pathPos = paths[i].position;

            if(i > 0)
            {
                var nextPathPos = paths[i - 1].position;
                Gizmos.DrawLine(pathPos, nextPathPos);

                if(i == paths.Count - 1)
                {
                    var startPathPos = paths[0].position;
                    Gizmos.DrawLine(pathPos, startPathPos);
                }
            }

            Gizmos.DrawCube(pathPos, pathPointSize);
        }
    }
}
