using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotateSpeed = 3f;
    public float distanceSpeed = 60f;

    float rotateX = 0;
    float rotateY = 0;
    Vector3 moveX = Vector3.zero;
    Vector3 moveY = Vector3.zero;
    Vector3 changeVector = Vector3.zero;

    float distance = 0;
    //const float maxDistance = -30;
    //const float minDistance = -1;

    public Camera mainCamera = null;
    public HexGrid hexGrid;


    void Start()
    {
        rotateX = transform.eulerAngles.y;
        rotateY = transform.eulerAngles.x;
        moveX = Vector3.zero;
        moveY = Vector3.zero;
        changeVector = Vector3.zero;

        distance = 0;
    }
    HexCell targetCell = null;//旋转的目标

    void Update()
    {
        //if(Input.GetMouseButtonDown(1))
        //{
        //    Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(inputRay, out hit))
        //    {
        //        if (hexGrid.GetCell(hit.point) != null)
        //        {
        //            targetCell = hexGrid.GetCell(hit.point);
        //        }
        //    }
        //}
        if (Input.GetMouseButton(1))
        {
            //if (targetCell != null)
            //{
            //    transform.RotateAround(targetCell.transform.position, Input.GetAxis("Mouse X") * this.transform.up - Input.GetAxis("Mouse Y") * this.transform.right, 25 * rotateSpeed * Time.deltaTime);
            //}
            //else
            //{
                rotateX += Input.GetAxis("Mouse X") * rotateSpeed;

                rotateY -= Input.GetAxis("Mouse Y") * rotateSpeed;

                rotateX = ClampAngle(rotateX, -360, 360);

                rotateY = ClampAngle(rotateY, -360, 360);

                Quaternion rotation = Quaternion.Euler(rotateY, rotateX, 0);

                transform.rotation = rotation;
            //}


        }
        else if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            distance += Input.GetAxis("Mouse ScrollWheel") * distanceSpeed;
            //distance = Mathf.Clamp(distance, maxDistance, minDistance);
            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, distance);
        }

        if (Input.GetMouseButton(2))
        {
            changeVector = Vector3.zero;
            moveX = -(Input.GetAxis("Mouse X") * moveSpeed) * transform.right;
            moveY = -(Input.GetAxis("Mouse Y") * moveSpeed) * transform.up;
            changeVector = moveX + moveY;
            transform.position = transform.position+changeVector;
        }

    }


    static float ClampAngle(float angle, float minAngle, float maxAngle)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, minAngle, maxAngle);
    }

}
