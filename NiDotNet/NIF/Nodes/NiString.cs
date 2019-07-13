using System.IO;

namespace NiDotNet.NIF.Nodes
{
    public class NiString
    {
        public readonly string String;

        public NiString(BinaryReader reader)
        {
            String = new string(reader.ReadChars((int) reader.ReadUInt32()));
        }

        public NiString(string str)
        {
            String = str;
        }

        public static implicit operator string(NiString niString) => niString.String;

        public override string ToString()
        {
            return String;
        }
    }
}