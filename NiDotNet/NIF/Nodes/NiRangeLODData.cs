using System.IO;
using UnityEngine;

namespace NiDotNet.NIF.Nodes
{
    public class NiRangeLODData : NiLODData
    {
        public Vector3 Center { get; set; }

        public uint LevelCount { get; set; }

        public NiLODRange[] Ranges { get; set; }

        public NiRangeLODData(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            Center = reader.ReadVector3();

            LevelCount = reader.ReadUInt32();

            Ranges = new NiLODRange[LevelCount];
            for (var i = 0; i < LevelCount; i++)
            {
                Ranges[i] = new NiLODRange(reader, niFile);
            }
        }
    }
}