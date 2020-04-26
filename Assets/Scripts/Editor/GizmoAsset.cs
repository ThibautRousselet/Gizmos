using UnityEngine;

namespace technical.test.editor
{
    [CreateAssetMenu(fileName = "Gizmo Asset", menuName = "Custom/Create Information Gizmo Asset")]
    public class GizmoAsset : ScriptableObject
    {
        [SerializeField]
        private InformationGizmo[] _gizmos = default;
    }
}