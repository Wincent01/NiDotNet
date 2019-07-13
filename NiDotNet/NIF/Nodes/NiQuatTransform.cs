using System.IO;
using UnityEngine;

namespace NiDotNet.NIF.Nodes
{
    public class NiQuatTransform : NiObject
    {
        public Vector3 Translation { get; set; }

        public Quaternion Rotation { get; set; }

        public float Scale { get; set; }

        public NiQuatTransform(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            Translation = reader.ReadVector3();

            Rotation = new NiQuaternion(reader, niFile);

            Scale = reader.ReadSingle();
        }
    }
}