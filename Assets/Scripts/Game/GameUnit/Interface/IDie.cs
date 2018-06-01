using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDie
{

    void Die();

    bool WillDie(int injury);
}

