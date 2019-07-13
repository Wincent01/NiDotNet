using System.IO;
using NiDotNet.NIF.Enums;
using UnityEngine;

namespace NiDotNet.NIF.Nodes
{
    public class NiMaterialProperty : NiProperty
    {
        public Color AmbientColor { get; set; }

        public Color DiffuseColor { get; set; }

        public Color SpecularColor { get; set; }

        public Color EmissiveColor { get; set; }

        public float Glossiness { get; set; }

        public float Alpha { get; set; }

        public float EmissiveMultiplier { get; set; } = 1f;

        public NiMaterialProperty(BinaryReader reader, NiFile file) : base(reader, file)
        {
            //
            //    Read colors
            //
            AmbientColor = reader.ReadColor3();

            DiffuseColor = reader.ReadColor3();

            SpecularColor = reader.ReadColor3();

            EmissiveColor = reader.ReadColor3();

            //
            //    Get other properties
            //
            Glossiness = reader.ReadSingle();

            Alpha = reader.ReadSingle();

            if (file.Header.NifVersion == NiVersion.Ver20207)
            {
                EmissiveMultiplier = reader.ReadSingle();
            }
        }
    }
}