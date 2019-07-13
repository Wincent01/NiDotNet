using System.IO;
using UnityEngine;

namespace NiDotNet.NIF
{
    public static class BinaryReaderExtensions
    {
        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static Matrix4x4 ReadMatrix4X4(this BinaryReader reader)
        {
            return new Matrix4x4
            {
                m11 = reader.ReadSingle(),
                m21 = reader.ReadSingle(),
                m31 = reader.ReadSingle(),
                m12 = reader.ReadSingle(),
                m22 = reader.ReadSingle(),
                m32 = reader.ReadSingle(),
                m13 = reader.ReadSingle(),
                m23 = reader.ReadSingle(),
                m33 = reader.ReadSingle()
            };
        }

        public static Color ReadColor3(this BinaryReader reader)
        {
            return new Color
            {
                r = reader.ReadSingle(),
                g = reader.ReadSingle(),
                b = reader.ReadSingle(),
                a = 1
            };
        }

        public static Color ReadColor4(this BinaryReader reader)
        {
            return new Color
            {
                r = reader.ReadSingle(),
                g = reader.ReadSingle(),
                b = reader.ReadSingle(),
                a = reader.ReadSingle()
            };
        }
    }
}