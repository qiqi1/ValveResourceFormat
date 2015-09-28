﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
    public class ArgumentDependencies : REDIBlock
    {
        public class ArgumentDependency
        {
            public string ParameterName { get; set; }
            public string ParameterType { get; set; }
            public uint Fingerprint { get; set; }
            public uint FingerprintDefault { get; set; }

            public void WriteText(IndentedTextWriter writer)
            {
                writer.WriteLine("ResourceArgumentDependency_t");
                writer.WriteLine("{");
                writer.Indent++;
                writer.WriteLine("CResourceString m_ParameterName = \"{0}\"", ParameterName);
                writer.WriteLine("CResourceString m_ParameterType = \"{0}\"", ParameterType);
                writer.WriteLine("uint32 m_nFingerprint = 0x{0:X8}", Fingerprint);
                writer.WriteLine("uint32 m_nFingerprintDefault = 0x{0:X8}", FingerprintDefault);
                writer.Indent--;
                writer.WriteLine("}");
            }
        }

        public List<ArgumentDependency> List;

        public ArgumentDependencies()
        {
            List = new List<ArgumentDependency>();
        }

        public override void Read(BinaryReader reader, Resource resource)
        {
            reader.BaseStream.Position = this.Offset;

            for (var i = 0; i < this.Size; i++)
            {
                var dep = new ArgumentDependency();

                var prev = reader.BaseStream.Position;
                reader.BaseStream.Position += reader.ReadUInt32();
                dep.ParameterName = reader.ReadNullTermString(Encoding.UTF8);
                reader.BaseStream.Position = prev + 4;

                prev = reader.BaseStream.Position;
                reader.BaseStream.Position += reader.ReadUInt32();
                dep.ParameterType = reader.ReadNullTermString(Encoding.UTF8);
                reader.BaseStream.Position = prev + 4;

                dep.Fingerprint = reader.ReadUInt32();
                dep.FingerprintDefault = reader.ReadUInt32();

                List.Add(dep);
            }
        }

        public override void WriteText(IndentedTextWriter writer)
        {
            writer.WriteLine("Struct m_ArgumentDependencies[{0}] =", List.Count);
            writer.WriteLine("[");
            writer.Indent++;

            foreach (var dep in List)
            {
                dep.WriteText(writer);
            }

            writer.Indent--;
            writer.WriteLine("]");
        }
    }
}