using System.IO;
using UnityEngine;

namespace NiDotNet.NIF.Nodes
{
    public class NiBound : NiObject
    {
        public Vector3 Center { get; set; }

        public float Radius { get; set; }

        public NiBound(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            Center = reader.ReadVector3();

            Radius = reader.ReadSingle();
        }
    }
}