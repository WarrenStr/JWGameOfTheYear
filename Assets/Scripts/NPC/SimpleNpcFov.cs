 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for handling NPC field of view logic.
/// </summary>
/// <remarks>
/// NPC can't see through things in the "Obstacle" layer and marks things within the "Target" layer.
/// </remarks>
public class SimpleNpcFov : MonoBehaviour
{
	public float viewRadius;
	[Range(0,360)] public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector] public List<Transform> visibleTargets = new List<Transform>();

	[Range(1,10)] public float meshResolution;

	public int edgeResolveIterations;
	public float edgeDistanceThreshold;

	public bool showView;
	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

    public event EventHandler<OnDetectedEventArgs> OnDetected;
    public class OnDetectedEventArgs : EventArgs 
	{
		public Transform target;
	}


	private void Start() 
	{
		viewMesh = new Mesh();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;

		StartCoroutine ("FindTargetsWithDelay", .2f);
	}


    private void LateUpdate()
    {
		if (showView)
		{
            DrawFieldOfView();
        }
    }


    private IEnumerator FindTargetsWithDelay(float delay) 
	{
		while (true) 
		{
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}


    private void FindVisibleTargets() {
		visibleTargets.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++) 
		{
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) 
			{
				float dstToTarget = Vector3.Distance (transform.position, target.position);

				if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, obstacleMask)) 
				{
					visibleTargets.Add (target);

					TargetDetected(target);
				}
			}
		}
	}


    public void TargetDetected(Transform target)
    {
		OnDetected?.Invoke(this, new OnDetectedEventArgs { target = target });
        Debug.Log("target hit");
    }


    private void DrawFieldOfView()
	{
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3> ();
		ViewCastInfo oldViewCast = new ViewCastInfo ();

		for (int i = 0; i <= stepCount;i++)
		{
			float angle = transform.localEulerAngles.y - viewAngle / 2 + stepAngleSize * i;

			//Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
					
			ViewCastInfo newViewCast = ViewCast(angle);

			if (i > 0) // Add additional mesh along corners to get rid of jitter.
			{
				bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
				{
					EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
					if (edge.pointA != Vector3.zero)
					{
						viewPoints.Add (edge.pointA);
					}
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
			}
			viewPoints.Add(newViewCast.point);
			oldViewCast = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero; // View mesh is a child of the chatacter object. This means the vertices position needs to be in local space local to the character.

		for (int i = 0; i < vertexCount - 1; i++)
		{
			vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);

			if (i < vertexCount - 2)
			{
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
		}
		viewMesh.Clear();
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();
	}


    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
	{
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeResolveIterations; i++)
		{
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
			{
				minAngle = angle;
				minPoint = newViewCast.point;
			}
			else
			{
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}
		return new EdgeInfo(minPoint, maxPoint);
	}


	private ViewCastInfo ViewCast(float globalAngle)
	{
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit hit;

		if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
		{
			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		}
		else
		{
			return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}
	}


	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) 
	{
		if (!angleIsGlobal) 
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}


	public struct ViewCastInfo
	{
		public bool hit;
		public Vector3 point;
		public float distance;
		public float angle;

		public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
		{
			hit = _hit;
			point = _point;
			distance = _distance;
			angle = _angle;
		}
	}


	public struct EdgeInfo
	{
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo( Vector3 _pointA, Vector3 _pointB)
		{
			pointA = _pointA;
			pointB = _pointB;
		}
	}
}
