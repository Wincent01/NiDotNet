using System.IO;
using UnityEngine;

namespace NiDotNet.NIF.Nodes
{
    public class NiTexCoord : NiObject
    {
        public float U { get; set; }

        public float V { get; set; }

        public NiTexCoord(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            U = reader.ReadSingle();
            V = reader.ReadSingle();
        }

        public static implicit operator Vector2(NiTexCoord c) => new Vector2(c.U, c.V);
    }
}