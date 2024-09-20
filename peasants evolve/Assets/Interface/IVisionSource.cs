using UnityEngine;

public interface IVisionSource
{
    Vector3 Position { get; }
    float RevealRadius { get; }
}