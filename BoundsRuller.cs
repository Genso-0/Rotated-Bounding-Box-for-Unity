using System.Collections.Generic;
using UnityEngine;
namespace RotatedBoundingVolume
{
    public class BoundsRuller : MonoBehaviour
    {
        public Vector3 sizeOfEachVolume = Vector3.one;
        public Vector3 offset;
        void OnDrawGizmos()
        {
            var mainRBB = Encapsulate.EncapsulateMeshRendererBounds(transform, offset);
            mainRBB.DrawBounds(Color.yellow);
            RBB edgeRBB = new RBB(transform, new Bounds(Vector3.zero, sizeOfEachVolume));
            var corners = Encapsulate.GetBoundCorners(transform, mainRBB, edgeRBB);

            for (int i = 0; i < corners.Length; i++)
            {
                Gizmos.DrawWireSphere(corners[i], 0.1f);
            }

            DrawEdgeVolumes(corners, edgeRBB);
        }

        private void DrawEdgeVolumes(Vector3[] corners, RBB edgeRBB)
        {
            foreach (var center in GetBoundedEdgePosition(corners[0], corners[1], edgeRBB.Size.z))
            {
                edgeRBB.Center = center;
                edgeRBB.DrawBounds(Color.blue);
            }
            foreach (var center in GetBoundedEdgePosition(corners[0], corners[3], edgeRBB.Size.x))
            {
                edgeRBB.Center = center;
                edgeRBB.DrawBounds(Color.red);
            }
            foreach (var center in GetBoundedEdgePosition(corners[0], corners[4], edgeRBB.Size.y))
            {
                edgeRBB.Center = center;
                edgeRBB.DrawBounds(Color.green);
            }
        }
        private IEnumerable<Vector3> GetBoundedEdgePosition(Vector3 start, Vector3 end, float sizeMagnitudeOnAxis)
        {
            float distance = Vector3.Distance(start, end);
            float step = sizeMagnitudeOnAxis / distance;
            for (float i = 0; i < 1; i += step)
            {
                yield return Vector3.Lerp(start, end, i);
            }
        }
    }
}
