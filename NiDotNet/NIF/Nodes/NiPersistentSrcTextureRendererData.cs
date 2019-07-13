using System.IO;
using NiDotNet.NIF.Enums;

namespace NiDotNet.NIF.Nodes
{
    public class NiPersistentSrcTextureRendererData : NiPixelFormat
    {
        public NiRef<NiPalette> Palette { get; set; }

        public uint MipMapCount { get; set; }

        public uint BytesPerPixel { get; set; }

        public NiMipMap[] MipMaps { get; set; }

        public uint PixelCount { get; set; }

        public uint FacesCount { get; set; }

        public PlatformId Platform { get; set; }

        public byte[,] PixelData { get; set; }

        public NiPersistentSrcTextureRendererData(BinaryReader reader, NiFile niFile) : base(reader, niFile)
        {
            PixelCount = reader.ReadUInt32();

            Palette = new NiRef<NiPalette>(niFile, reader.ReadInt32());

            FacesCount = reader.ReadUInt32();

            //MipMapCount = reader.ReadUInt32();

            BytesPerPixel = reader.ReadUInt32();

            //MipMaps = new NiMipMap[MipMapCount];

            /*
            for (var i = 0; i < MipMapCount; i++)
            {
                MipMaps[i] = new NiMipMap(reader, niFile);
            }
            */

            //Platform = (PlatformId) reader.ReadUInt32();

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