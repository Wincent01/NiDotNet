using System.IO;

namespace NiDotNet.NIF.Nodes
{
    public class NiSkinPartition : NiObject
    {
        public uint SkinPartitionCount { get; set; }

        public SkinPartition[] SkinPartitions { get; set; }

        public uint DataSize { get; set; }

        public uint VertexSize { get; set; }

        public SkinPartition[] Partitions { get; set; }

        public NiSkinPartition(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            SkinPartitionCount = reader.ReadUInt32();

            SkinPartitions = new SkinPartition[SkinPartitionCount];

            for (var i = 0; i < SkinPartitionCount; i++)
            {
                SkinPartitions[i] = new SkinPartition(reader, niFile);
            }

            DataSize = reader.ReadUInt32();

            VertexSize = reader.ReadUInt32();

            Partitions = new SkinPartition[SkinPartitionCount];

            for (var i = 0; i < SkinPartitionCount; i++)
            {
                Partitions[i] = new SkinPartition(reader, niFile);
            }
        }
    }
}