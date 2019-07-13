using System.IO;
using NiDotNet.NIF.Enums;

namespace NiDotNet.NIF.Nodes
{
    public class NiKeyGroup<T> : NiObject
    {
        public uint KeyCount { get; set; }

        public KeyType Type { get; set; }

        public NiKey<T>[] Keys { get; set; }

        public NiKeyGroup(BinaryReader reader, NiFile niFile, bool interpolation) : base(reader, niFile)
        {
            KeyCount = reader.ReadUInt32();

            Type = (KeyType) reader.ReadUInt32();

            if (!interpolation) return;
            Keys = new NiKey<T>[KeyCount];
            for (var i = 0; i < KeyCount; i++)
            {
                Keys[i] = new NiKey<T>(reader, niFile, Type);
            }
        }
    }
}