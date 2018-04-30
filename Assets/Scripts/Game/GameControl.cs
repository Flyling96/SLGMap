using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

    public HexGrid hexGrid;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            HandleInputDown();
        }

    }

    public void HandleInputDown()
    {
        HexCell centerCell = null;
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            centerCell = hexGrid.GetCell(hit.point);
            hexGrid.DistanceToOther(centerCell);
        }
    }
}
