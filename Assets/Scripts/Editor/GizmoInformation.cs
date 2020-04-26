using System;
using UnityEngine;

namespace technical.test.editor
{

    [Serializable]
    public struct InformationGizmo
    {
        public string Name;
        public Vector3 Position;

        public InformationGizmo(string name, Vector3 position)
        {
            Name = name;
            Position = position;
        }
    }

}