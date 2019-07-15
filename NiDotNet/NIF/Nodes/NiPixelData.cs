using System;
using System.IO;

namespace NiDotNet.NIF.Nodes
{
    public class NiPixelData : NiPixelFormat
    {
        public uint PixelCount { get; set; }

        public uint FacesCount { get; set; }

        public byte[,] PixelData { get; set; }

        public NiPixelData(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            throw new NotImplementedException("NiPixelData still W.I.P");

            PixelCount = reader.ReadUInt32();

            FacesCount = reader.ReadUInt32();

            PixelData = new byte[FacesCount, PixelCount];

            for (var i = 0; i < FacesCount; i++)
            {
                for (var j = 0; j < PixelCount; j++)
                {
                    PixelData[i, j] = reader.ReadByte();
                }
            }
        }
    }
}