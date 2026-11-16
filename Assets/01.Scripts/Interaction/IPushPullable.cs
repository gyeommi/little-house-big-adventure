using UnityEngine;

public interface IPushPullable
{
    void Grab(Transform player);
    void Release();
}