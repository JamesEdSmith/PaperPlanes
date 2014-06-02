using UnityEngine;
using System.Collections.Generic;

public class MouseHandler : MonoBehaviour 
{
	bool hitLastUpdate;
	Vector2 foldPoint1;
	Vector2 foldPoint2;
	float foldAngle;
	float foldTracker;
	bool prevClick;
	bool moveClick;
	float c;
	float m;
	string str;
	string str2;
	public PolygonTester polygonTester;
	GameObject lastHit;
	public GameObject movingFold;
	public GameObject otherFold;
	public int camFoldIndex;
	public bool refacedLastUpdate;
	public List<GameObject> movingFoldSections;
	public List<MeshCollider> foldSections;
	
	// Use this for initialization
	void Start () 
	{
		movingFoldSections = new List<GameObject>();
		foldSections = new List<MeshCollider>();
		hitLastUpdate = false;
		prevClick = false;
		moveClick = false;
		refacedLastUpdate = false;
		camFoldIndex = 0;
		m = 0f;
		c = 0f;
	}
	
	void OnGUI()
	{
        GUI.TextArea (new Rect (10, 10, 400, 25), str);
		//GUI.TextArea (new Rect (10, 40, 400, 200), str2);
    }
	
	void Update()
	{
		if(movingFold)
			moving();
		else
			folding();
		

		str = "foldAngle: " + foldTracker;
		if(movingFold)
		{
			str2 = "movingVerts: ";
			foreach(Vector3 v in movingFold.GetComponent<MeshFilter>().mesh.vertices)
				str2 += " " + v.ToString();
		}
		else
			str = " ";
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(2);
		lineRenderer.SetPosition(0, new Vector3(foldPoint1.x, foldPoint1.y, 0f));
		lineRenderer.SetPosition(1, new Vector3(foldPoint2.x, foldPoint2.y, 0f));
	}
	
	void moving()
	{
		Vector3 axis = new Vector3(foldPoint1.x - foldPoint2.x, foldPoint1.y - foldPoint2.y, 0f);
		float dx = Mathf.Pow (axis.x, 2);
		float dy = Mathf.Pow (axis.y, 2);
		float dz = Mathf.Pow (axis.z, 2);
		float magnitude = Mathf.Pow((dx+dy+dz),0.5f);
		
		axis /= magnitude;
		foldAngle = Input.GetAxis("MouseX") * 20f;
		
		if(Mathf.Abs(foldAngle + foldTracker) <= 180)
		{
			foldTracker += foldAngle;
		}
		else
		{
			if(foldAngle + foldTracker >= 180f)
			{
				foldAngle = 180f - foldTracker;
				foldTracker = 180f;
			}
			else
			{
				foldAngle = -180f - foldTracker;
				foldTracker = -180f;
			}
		}
		
		//movingFold.transform.RotateAround(new Vector3(foldPoint1.x, foldPoint1.y, 0f), axis, foldAngle);
		foreach(GameObject fold in movingFoldSections)
			fold.transform.RotateAround(new Vector3(foldPoint1.x, foldPoint1.y, 0f), axis, foldAngle);
		
		if(Input.GetButton("Fire1"))
		{
			//if(Mathf.Abs(foldTracker) == 180)
			//	polygonTester.combineFolds(movingFold, otherFold);

			movingFold = null;
			otherFold = null;
			moveClick = true;
			foldTracker = 0f;

		}
	}

	void findOtherFolds (MeshCollider collider)
	{
		/*RaycastHit hit;
		Vector3 foldV1 = new Vector3(foldPoint1.x, foldPoint1.y, 0f);
		Vector3 foldV2 = new Vector3 (foldPoint2.x, foldPoint2.y, 0f);
		Vector3 direction = foldV2 - foldV1;
		direction.Normalize();
		Ray ray = new Ray(foldV1 , direction);

		List<GameObject> foldsList = new List<GameObject>();
		List<Mesh> meshList = new List<Mesh>();
		foldsList.Add(collider.gameObject);
		meshList.Add(((MeshCollider)collider).sharedMesh);
		//((MeshCollider)collider).sharedMesh = null;

		while ( Physics.Raycast(ray, out hit, 99999))
		{
			meshList.Add(((MeshCollider)hit.collider).sharedMesh);
			foldsList.Add(hit.collider.gameObject);
			((MeshCollider)hit.collider).sharedMesh = null;
		}

		direction = foldV1 - foldV2;
		direction.Normalize();
		ray = new Ray(foldV2 , direction);

		while ( Physics.Raycast(ray, out hit, 99999))
		{
			meshList.Add(((MeshCollider)hit.collider).sharedMesh);
			foldsList.Add(hit.collider.gameObject);
			((MeshCollider)hit.collider).sharedMesh = null;
		}

		for(int i = 0; i < foldsList.Count; i++)
		{
			MeshCollider mc = foldsList[i].GetComponent<MeshCollider>();
			mc.sharedMesh = meshList[i];
			foldSections.Add(mc);
		}
		*/
		foreach(GameObject go in polygonTester.foldSections)
		{
			foldSections.Add(go.GetComponent<MeshCollider>());
		}

	}

	void folding()
	{
		if(!moveClick)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //new Vector3(Input.GetAxis("MouseX"), Input.GetAxis("MouseY"), 0)
			if ( Physics.Raycast(ray, out hit, 99999))
			{
				if(lastHit != hit.collider.gameObject && lastHit)
				{
					lastHit.renderer.material.color = Color.white;
				}
				
				if ( hit.collider.gameObject.renderer )
				{
					
					hit.collider.gameObject.renderer.material.color = new Color(1f, 0f, 1f, 1.0f);
					hitLastUpdate = true;
	
				}
				
				if(Input.GetButton("Fire1"))
				{
					if(!prevClick)
					{
						foldPoint1 = new Vector2(hit.point.x, hit.point.y);
					}
					prevClick = true;
					hit.collider.gameObject.renderer.material.color = new Color(0f, 1f, 1f, 1.0f);
				}
				else 
				{
					if (prevClick)
					{
						foldPoint2 = new Vector2(hit.point.x, hit.point.y);
						findOtherFolds((MeshCollider)hit.collider);

						foldLine();
						foldSections.Clear();

						hit.collider.gameObject.renderer.material.color = new Color(1f, 0f, 1f, 1.0f);
					}
					prevClick = false;
				}
			
				lastHit = hit.collider.gameObject;	
			}
			else if (hitLastUpdate && lastHit != null)
			{
				lastHit.renderer.material.color = Color.white;	
				hitLastUpdate = false;
			}
			// paper turning 
			bool moved = false;
			
			if(refacedLastUpdate)
			{
				if(Input.GetAxis("Horizontal") == 0)
					refacedLastUpdate = false;
			}
			else
			{
				if(Input.GetAxis("Horizontal") > 0)
				{
					polygonTester.foldSections[camFoldIndex].renderer.material.color = Color.white;
					camFoldIndex++;
					if(camFoldIndex >= polygonTester.foldSections.Count)
					{
						camFoldIndex = 0;	
					}
					polygonTester.foldSections[camFoldIndex].renderer.material.color = Color.green;
					moved = true;
				}
				else if (Input.GetAxis("Horizontal") < 0)
				{
					polygonTester.foldSections[camFoldIndex].renderer.material.color = Color.white;
					camFoldIndex--;
					if(camFoldIndex < 0 )
					{
						camFoldIndex = polygonTester.foldSections.Count -1;	
					}
					polygonTester.foldSections[camFoldIndex].renderer.material.color = Color.green;
					moved = true;
				}
				
				if (moved)
				{
					Vector3 shift = Vector3.zero - polygonTester.foldSections[camFoldIndex].transform.position;
					
					str2 = "shift " + shift.x + " " + shift.y + " " + shift.z;
					
					foreach(GameObject go in polygonTester.foldSections)
					{
						go.transform.position += shift;	
					}
					
					refacedLastUpdate = true;
				}
			}
		}
		else
		{
			if(!Input.GetButton("Fire1"))
			{
				moveClick = false;
			}
		}
	}
	
	void foldLine()
	{
		movingFoldSections.Clear();

		foreach(MeshCollider collider in foldSections)
		{
			Mesh mesh = collider.sharedMesh;
			List<Vector2> poly1 = new List<Vector2>();
			List<Vector2> poly2 = new List<Vector2>();
			
			m = (foldPoint2.y - foldPoint1.y)/(foldPoint2.x - foldPoint1.x);
			c = foldPoint1.y - (m * foldPoint1.x);
			if(m > 1f)
			{
				foldPoint1.y = 100f;
				foldPoint1.x = (100f - c) / m;
				foldPoint2.y = -100f;
				foldPoint2.x = (-100f - c) / m;
			}
			else
			{
				foldPoint1.x = 100f;
				foldPoint1.y = 100f * m + c;
				foldPoint2.x = -100f;
				foldPoint2.y = -100f * m + c;
			}
			
			Vector3 prevVert = collider.transform.TransformPoint(mesh.vertices[mesh.vertices.Length - 1]);
			Vector3 currVert;
			
			for(int i = 0; i < mesh.vertices.Length; i++)
			{
				currVert = collider.transform.TransformPoint(mesh.vertices[i]);
				Vector2 intersection = new Vector2();
				if(DoLineSegmentIntersection(new Vector2(currVert.x, currVert.y), new Vector2(prevVert.x, prevVert.y), foldPoint1, foldPoint2, out intersection))
				{
					poly1.Add(new Vector2(intersection.x, intersection.y));
					poly2.Add(new Vector2(intersection.x, intersection.y));
					poly2.Add(currVert);
					prevVert = currVert;
					for(int j = i+1; j < mesh.vertices.Length; j++)
					{
						currVert = collider.transform.TransformPoint(mesh.vertices[j]);
						if(DoLineSegmentIntersection(new Vector2(currVert.x, currVert.y), new Vector2(prevVert.x, prevVert.y), foldPoint1, foldPoint2, out intersection))
						{
							poly1.Add(new Vector2(intersection.x, intersection.y));
							poly2.Add(new Vector2(intersection.x, intersection.y));
							prevVert = currVert;
							i = j;
							currVert = collider.transform.TransformPoint(mesh.vertices[i]);
							break;
						}
						poly2.Add(currVert);
						prevVert = currVert;
					}
					
				}
				poly1.Add(currVert);
				prevVert = currVert;
			}
			polygonTester.recreatePolygons(collider.gameObject, poly1, poly2);
			//find all the folds that are attached to this one so we can move all of them at once as we fold
			movingFoldSections.Add(movingFold);
			createMovingFolds(movingFold, otherFold);
			foldAngle = 0f;
		}
	}

	void createMovingFolds(GameObject currFold, GameObject prevFold)
	{

		foreach(GameObject fold in polygonTester.foldSections)
		{
			if(fold != prevFold && fold != currFold && fold != otherFold)
			{
				int sharedVerts = 0;
				foreach(Vector3 vert in currFold.GetComponent<MeshFilter>().mesh.vertices)
				{
					foreach(Vector3 vert2 in fold.GetComponent<MeshFilter>().mesh.vertices)
					{
						if (vert.x == vert2.x && vert.y == vert2.y && vert.z == vert2.z)
						{
							sharedVerts++;
							createMovingFolds(fold, currFold);
						}
					}
				}
				
				if(sharedVerts >= 2 && !movingFoldSections.Contains(fold))
				{
					movingFoldSections.Add(fold);
				}
			}
		}
	}

	public bool DoLineSegmentIntersection(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, out Vector2 output)
	{
	    LineSegment linesegment0 = new LineSegment();
		linesegment0.begin_ = p0;
		linesegment0.end_ = p1;
	    LineSegment linesegment1 = new LineSegment();
		linesegment1.begin_ = p2;
		linesegment1.end_ = p3;
	
	    Vector2 intersection = new Vector2(0f,0f);
	
	    switch(linesegment0.Intersect(linesegment1, out intersection))
	    {
	    case 0:
			output = intersection;
	        return false;
	    case 1:
			output = intersection;
	        return false;
	    case 2:
			output = intersection;
	        return false;
	    case 3:
	        output = intersection;
			return true;
		default:
			output = intersection;
			return false;
	    }
	}
}

public class LineSegment
{
	public static int PARALLEL = 0;
	public static int COINCIDENT = 1;
	public static int NOT_INTERESECTING = 2;
	public static int INTERESECTING = 3;
	
    public Vector2 begin_;
    public Vector2 end_;

    public int Intersect(LineSegment other_line, out Vector2 intersection)
    {
        float denom = ((other_line.end_.y - other_line.begin_.y)*(end_.x - begin_.x)) -
                      ((other_line.end_.x - other_line.begin_.x)*(end_.y - begin_.y));

        float nume_a = ((other_line.end_.x - other_line.begin_.x)*(begin_.y - other_line.begin_.y)) -
                       ((other_line.end_.y - other_line.begin_.y)*(begin_.x - other_line.begin_.x));

        float nume_b = ((end_.x - begin_.x)*(begin_.y - other_line.begin_.y)) -
                       ((end_.y - begin_.y)*(begin_.x - other_line.begin_.x));

        if(denom == 0.0f)
        {
            if(nume_a == 0.0f && nume_b == 0.0f)
            {
				intersection = new Vector2(0f,0f);
                return COINCIDENT;
            }
			intersection = new Vector2(0f,0f);
            return PARALLEL;
        }

        float ua = nume_a / denom;
        float ub = nume_b / denom;

        if(ua >= 0.0f && ua <= 1.0f && ub >= 0.0f && ub <= 1.0f)
        {
            // Get the intersection point.
            intersection.x = begin_.x + ua*(end_.x - begin_.x);
            intersection.y = begin_.y + ua*(end_.y - begin_.y);

            return INTERESECTING;
        }
		intersection = new Vector2(0f,0f);
        return NOT_INTERESECTING;
    }
}
