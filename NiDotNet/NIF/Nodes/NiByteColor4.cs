using System.IO;
using UnityEngine;

namespace NiDotNet.NIF.Nodes
{
    public class NiByteColor4 : NiObject
    {
        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get; set; }

        public byte A { get; set; }

        public NiByteColor4(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            R = reader.ReadByte();

            G = reader.ReadByte();

            B = reader.ReadByte();

            A = reader.ReadByte();
        }

        public static implicit operator Color(NiByteColor4 c) => new Color
        {
            r = c.R / 255f,
            g = c.G / 255f,
            b = c.B / 255f,
            a = c.A / 255f,
        };
    }
}