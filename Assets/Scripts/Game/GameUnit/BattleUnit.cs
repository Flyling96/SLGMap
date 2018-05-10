using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BattleUnitProperty
{
    public int unitHP;
    public int unitMP;
    public int actionPower;
    public int attack;
    public int defanse;
    public int attackDistance;
}


public class BattleUnit : MonoBehaviour, IBattleUnitFire, IMove
{
    float moveSpeed = 2.5f;
    float rotateSpeed = 180f;

    public HexCell cell;

    public BattleUnitProperty battleUnitProperty;
    List<HexCell> currentRoad;

    public void FireSoldier(BattleUnit hiter)
    {

    }

    public void FireBuilder(BuildUnit hiter)
    {

    }

    public void Move(List<HexCell> cells)
    {
        StartCoroutine(MoveAnim(cells));
    }

    IEnumerator MoveAnim(List<HexCell> cells)
    {
        Vector3 a, b, c = cells[0].transform.position;
        yield return LookAt(cells[1].transform.position);

        for (int i = 1; i < cells.Count + 1; i++)
        {
            a = c;
            if (i != cells.Count)
            {
                b = cells[i - 1].transform.position;
                c = (b + cells[i].transform.position) * 0.5f;
            }
            else
            {
                b = cells[cells.Count - 1].transform.position;
                c = b;
            }
            for (float t = Time.deltaTime * moveSpeed; t < 1f; t += Time.deltaTime * moveSpeed)
            {
                //transform.position = Vector3.Lerp(start, end, t);
                transform.position = ToolClass.instance.GetBezierPoint(a, b, c, t);
                Vector3 dir = -ToolClass.instance.GetBezierDerivative(a, b, c, t);
                dir.y = 0f;
                transform.localRotation = Quaternion.LookRotation(dir);
                yield return null;
            }
        }
    }

    IEnumerator LookAt(Vector3 point)
    {
        point.y = transform.position.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation =Quaternion.LookRotation(transform.position - point);
        float speed = rotateSpeed / Quaternion.Angle(fromRotation, toRotation);

        for (float t = Time.deltaTime * speed;t < 1f;t += Time.deltaTime * speed)
        {
            transform.localRotation =Quaternion.Slerp(fromRotation, toRotation, t);
            yield return null;
        }

        //transform.LookAt(-point);
        //orientation = transform.localRotation.eulerAngles.y;
    }


    //IEnumerator TravelPath()
    //{
    //    Vector3 a, b, c = pathToTravel[0].Position;
    //    transform.localPosition = c;
    //    yield return LookAt(pathToTravel[1].Position);

    //    float t = Time.deltaTime * travelSpeed;
    //    for (int i = 1; i < pathToTravel.Count; i++)
    //    {
    //        a = c;
    //        b = pathToTravel[i - 1].Position;
    //        c = (b + pathToTravel[i].Position) * 0.5f;
    //        for (; t < 1f; t += Time.deltaTime * travelSpeed)
    //        {
    //            transform.localPosition = Bezier.GetPoint(a, b, c, t);
    //            Vector3 d = Bezier.GetDerivative(a, b, c, t);
    //            d.y = 0f;
    //            transform.localRotation = Quaternion.LookRotation(d);
    //            yield return null;
    //        }
    //        t -= 1f;
    //    }

    //    a = c;
    //    b = pathToTravel[pathToTravel.Count - 1].Position;
    //    c = b;
    //    for (; t < 1f; t += Time.deltaTime * travelSpeed)
    //    {
    //        transform.localPosition = Bezier.GetPoint(a, b, c, t);
    //        Vector3 d = Bezier.GetDerivative(a, b, c, t);
    //        d.y = 0f;
    //        transform.localRotation = Quaternion.LookRotation(d);
    //        yield return null;
    //    }

    //    transform.localPosition = location.Position;
    //    orientation = transform.localRotation.eulerAngles.y;
    //    ListPool<HexCell>.Add(pathToTravel);
    //    pathToTravel = null;
    //}
}
