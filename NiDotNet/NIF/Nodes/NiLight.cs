using System.IO;
using UnityEngine;

namespace NiDotNet.NIF.Nodes
{
    public class NiLight : NiDynamicEffect
    {
        public float Dimmer { get; set; }

        public Color AmbientColor { get; set; }

        public Color DiffuseColor { get; set; }

        public Color SpecularColor { get; set; }

        public NiLight(BinaryReader reader, NiFile file) : base(reader, file)
        {
            Dimmer = reader.ReadSingle();

            AmbientColor = reader.ReadColor3();

            DiffuseColor = reader.ReadColor3();

            SpecularColor = reader.ReadColor3();
        }
    }
}