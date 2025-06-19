using System;
using System.Collections;

public interface IGameState
{
    IEnumerator Execute();
}