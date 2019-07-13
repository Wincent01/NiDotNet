using System.IO;
using NiDotNet.NIF.Enums;

namespace NiDotNet.NIF.Nodes
{
    public class NiKeyframeData : NiObject
    {
        public uint RotationKeyCount { get; set; }

        public KeyType Type { get; set; }

        public QuatKey<NiQuaternion>[] QuaternionKeys { get; set; }

        public NiKeyGroup<NiFloat>[] Rotations { get; set; }

        public NiKeyGroup<NiVector3> Translations { get; set; }

        public NiKeyGroup<NiFloat> Scales { get; set; }

        public NiKeyframeData(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            RotationKeyCount = reader.ReadUInt32();

            if (RotationKeyCount != 0)
                Type = (KeyType) reader.ReadUInt32();

            if (Type != (KeyType) 4)
            {
                QuaternionKeys = new QuatKey<NiQuaternion>[RotationKeyCount];
            }
            else
            {
                Rotations = new NiKeyGroup<NiFloat>[3];
                for (var i = 0; i < 3; i++)
                {
                    Rotations[i] = new NiKeyGroup<NiFloat>(reader, niFile, false);
                }
            }

            Translations = new NiKeyGroup<NiVector3>(reader, niFile, false);

            Scales = new NiKeyGroup<NiFloat>(reader, niFile, false);
        }
    }
}