using UnityEngine;
using System.Collections.Generic;
using System;

public class PolygonTester : MonoBehaviour {
	public List<GameObject> foldSections;
	public MouseHandler mouseHander;
	public Material lineMaterial;
    void Start () {
		//collection of polygons that make up all the folded sections of the plane
		foldSections = new List<GameObject>();
        // Create Vector2 vertices
        Vector2[] vertices2D = new Vector2[] {
			new Vector2(-5.5f, 4f),
            new Vector2(5.5f, 4f),
            new Vector2(5.5f, -4f),
            new Vector2(-5.5f, -4f),
        };
		// Create Vector2 vertices
        Vector2[] vertices2Db = new Vector2[] {
            new Vector2(-5.5f, -4f),
			new Vector2(5.5f, -4f),
			new Vector2(5.5f, 4f),
			new Vector2(-5.5f, 4f),
        };
       
        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
		Triangulator trb = new Triangulator(vertices2Db);

        int[] indices = tr.Triangulate();
		int[] indicesb = tr.Triangulate();
       
        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
		Vector3[] verticesb = new Vector3[vertices2D.Length];
        for (int i=0; i<vertices2D.Length; i++) {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0f);
			verticesb[i] = new Vector3(vertices2Db[i].x, vertices2Db[i].y, 0.001f);
        }
       
        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();
		
		Mesh mshb = new Mesh();
        mshb.vertices = verticesb;
        mshb.triangles = indicesb;
        mshb.RecalculateNormals();
        mshb.RecalculateBounds();
       
        // Set up game object with mesh;
		GameObject newFold = new GameObject("fold" + foldSections.Count);
		GameObject newFoldb = new GameObject("foldb" + foldSections.Count);
		GameObject line = new GameObject("line" + foldSections.Count);
        newFold.AddComponent(typeof(MeshRenderer));
		newFoldb.AddComponent(typeof(MeshRenderer));
		line.AddComponent(typeof(LineRenderer));
		newFoldb.transform.parent = newFold.transform;
		line.transform.parent = newFold.transform;
		MeshCollider collider = newFold.AddComponent(typeof(MeshCollider)) as MeshCollider;
		collider.sharedMesh = msh;
        MeshFilter filter = newFold.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
		filter.renderer.material.color = Color.white;
		MeshFilter filterb = newFoldb.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filterb.mesh = mshb;
		filterb.renderer.material.color = Color.white;
		newFold.AddComponent("WireframeFilter");
		newFoldb.AddComponent("WireframeFilter");
		addFoldSection(newFold);
    }
	
	public void addFoldSection(GameObject newFold)
	{
		foldSections.Add(newFold);
	}
	
	public void removeFoldSection(GameObject fold)
	{
		foldSections.Remove(fold);
		Destroy(fold);
	}

	public void combineFolds(GameObject obj1, GameObject obj2)
	{
		removeFoldSection(obj1);
		removeFoldSection(obj2);
		Vector3[] obj1Verts = obj1.GetComponent<MeshFilter>().mesh.vertices;
		Vector3[] obj2Verts = obj2.GetComponent<MeshFilter>().mesh.vertices;

		int totalVerts = obj1.GetComponent<MeshFilter>().mesh.vertices.Length;
		int totalVerts2 = obj2.GetComponent<MeshFilter>().mesh.vertices.Length;
		int n;

		Vector2[] verts1 = new Vector2[obj1Verts.Length];
		for(n = 0; n < obj1Verts.Length; n++)
		{
			verts1[n] = new Vector2(obj1.transform.TransformPoint(obj1Verts[n]).x, obj1.transform.TransformPoint(obj1Verts[n]).y);
		}

		Vector2[] verts2 = new Vector2[obj2Verts.Length];
		for(n = 0; n < obj2Verts.Length; n++)
		{
			verts2[n] = new Vector2(obj2.transform.TransformPoint(obj2Verts[n]).x, obj2.transform.TransformPoint(obj2Verts[n]).y);
		}

		List<Vector2> combinedPoly = new List<Vector2>();

		int i2 = 0;
		int j2 = 0;
		Vector2 intersection;
		for(n = 0; n < totalVerts2; n++)
		{
			combinedPoly.Add(verts2[n]);
		}

		for(int j = 0; j < combinedPoly.Count; j++)
		{
			for(int i = 0; i < totalVerts; i++)
			{
				i2 = i + 1 < totalVerts ? i + 1 : 0;
				j2 = j + 1 < totalVerts2 ? j + 1 : 0;
				intersection = new Vector2();
				try{
				if(mouseHander.DoLineSegmentIntersection(verts1[i], verts1[i2], combinedPoly[j], combinedPoly[j2], out intersection) && !checkForVector(combinedPoly, verts1[i2]))
				{
					if(!containsPoint(combinedPoly, verts1[i2]))
					{
						combinedPoly.Insert(j2, verts1[i2]);
						j++;
					}

					combinedPoly.Insert(j2, intersection);
					j++;
				}
				}catch (Exception e)
				{
					Debug.LogError( e.ToString() + " " + i + ":" + totalVerts + " " + i2 + " " + j + ":" + combinedPoly.Count + " " + j2);
				}
			}
		}
		createPolygon(combinedPoly.ToArray());
	}

	bool checkForVector(List<Vector2> list, Vector2 vert)
	{
		foreach(Vector2 vector2 in list)
		{
			if(vector2.x == vert.x && vector2.y == vert.y)
				return true;
		}

		return false;
	}

	bool containsPoint (List<Vector2> polyPoints, Vector2 p)
	{ 
		int j = polyPoints.Count-1; 
		bool inside = false; 
		for (int i = 0; i < polyPoints.Count; j = i++) { 
			if ( ((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) && 
			    (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
				inside = !inside; 
		} 
		return inside; 
	}

	GameObject createPolygon(Vector2[] poly1)
	{
		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(poly1);
		int[] indices = tr.Triangulate();
		Vector2 [] poly1b = new Vector2[poly1.Length];
		for(int i = 0; i<poly1.Length; i++)
		{
			poly1b[i] = poly1[poly1.Length - 1 - i];
		}

		Triangulator trb = new Triangulator(poly1b);
		int[] indicesb = tr.Triangulate();
		
		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[poly1.Length];
		for (int i=0; i<vertices.Length; i++) {
			vertices[i] = new Vector3(poly1[i].x, poly1[i].y, 0f);
		}
		Vector3[] verticesb = new Vector3[poly1b.Length];
		for (int i=0; i<verticesb.Length; i++) {
			verticesb[i] = new Vector3(poly1b[i].x, poly1b[i].y, 0.001f);
		}
		
		// Create the mesh
		Mesh msh = new Mesh();
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		
		Mesh mshb = new Mesh();
		mshb.vertices = verticesb;
		mshb.triangles = indicesb;
		mshb.RecalculateNormals();
		mshb.RecalculateBounds();
		
		// Set up game object with mesh
		GameObject newFold1 = new GameObject("fold" + (foldSections.Count));
		newFold1.AddComponent(typeof(MeshRenderer));
		GameObject newFold1b = new GameObject("foldb" + (foldSections.Count));
		newFold1b.AddComponent(typeof(MeshRenderer));
		newFold1b.transform.parent = newFold1.transform;
		MeshCollider collider = newFold1.AddComponent(typeof(MeshCollider)) as MeshCollider;
		collider.sharedMesh = msh;
		MeshFilter filter = newFold1.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
		filter.renderer.material.color = Color.white;
		MeshFilter filterb = newFold1b.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filterb.mesh = mshb;
		filterb.renderer.material.color = Color.white;
		newFold1.AddComponent("WireframeFilter");
		newFold1b.AddComponent("WireframeFilter");
		addFoldSection (newFold1);
		return newFold1;
	}
	
	public void recreatePolygons(GameObject mcollider, List<Vector2> polyList1, List<Vector2> polyList2)
	{
		Quaternion rotation = new Quaternion(mcollider.transform..x, mcollider.transform.rotation.y, mcollider.transform.rotation.z, mcollider.transform.rotation.w);
		removeFoldSection(mcollider);
		
		Vector2[] poly1 = new Vector2[polyList1.Count];

		for(int i = 0; i<polyList1.Count; i++)
		{
			poly1[i] = polyList1[i];
		}

		GameObject newFold1 = createPolygon(poly1);

		Vector2[] poly2 = new Vector2[polyList2.Count];

		for(int i = 0; i<polyList2.Count; i++)
		{
			poly2[i] = polyList2[i];
		}

		GameObject newFold2 = createPolygon(poly2);

		
		if(newFold1.collider.bounds.size.sqrMagnitude < newFold2.collider.bounds.size.sqrMagnitude)
		{
			mouseHander.movingFold = newFold1;
			mouseHander.otherFold = newFold2;
			newFold1.GetComponent<MeshFilter>().renderer.material.color = new Color(1f, 0f, 1f, 1.0f);
		}
		else
		{
			mouseHander.movingFold = newFold2;
			mouseHander.otherFold = newFold1;
			newFold2.GetComponent<MeshFilter>().renderer.material.color = new Color(1f, 0f, 1f, 1.0f);
		}
	}
}