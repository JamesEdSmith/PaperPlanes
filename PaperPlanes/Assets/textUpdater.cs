using UnityEngine;
using System.Collections;

public class textUpdater : MonoBehaviour {
    string str;
    void Update()
	{
		str = Input.mousePosition.x + ", " + Input.mousePosition.y;
    }
	
	
}
