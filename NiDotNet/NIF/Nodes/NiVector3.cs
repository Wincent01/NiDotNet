using System.IO;
using UnityEngine;

namespace NiDotNet.NIF.Nodes
{
    public class NiVector3 : NiObject
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public NiVector3(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public static implicit operator Vector3(NiVector3 v) => new Vector3(v.X, v.Y, v.Z);
    }
}