using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.GraphicsBuffer;

public class CanvasConst : MonoBehaviour
{
    [SerializeField] RotationConstraint rotationConstraint;

    void Start()
    {
        var targetObject = FindAnyObjectByType<PivotCanvas>();

        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = targetObject.transform;
        source.weight = 1.0f;

        rotationConstraint.AddSource(source);
    }
}
