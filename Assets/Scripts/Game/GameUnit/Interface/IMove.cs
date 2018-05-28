using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove{

    void SetRoad(List<HexCell> roadCells);

    void Move(List<HexCell> cells);

    void MoveInRound();
}
