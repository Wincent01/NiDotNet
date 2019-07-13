using System.IO;
using UnityEngine;

namespace NiDotNet.NIF.Nodes
{
    public class NiAVObject : NiObjectNet
    {
        public short Flags { get; set; }

        public Vector3 Position { get; set; }

        public Matrix4x4 Rotation { get; set; }

        public float UniformScale { get; set; }

        public int[] Properties { get; set; }

        public uint CollitionObject { get; set; }

        public NiAVObject(BinaryReader reader, NiFile file) : base(reader, file)
        {
            Flags = reader.ReadInt16();

            //
            //    Get position
            //
            Position = reader.ReadVector3();

            //
            //    Get rotation
            //
            Rotation = reader.ReadMatrix4X4();

            //
            //    Get scale
            //
            UniformScale = reader.ReadSingle();

            //
            //    Get properties
            //
            Properties = new int[reader.ReadInt32()];
            for (var i = 0; i < Properties.Length; i++)
            {
                Properties[i] = reader.ReadInt32();
            }

            //
            //    Get collition object
            //
            CollitionObject = reader.ReadUInt32();
        }
    }
}