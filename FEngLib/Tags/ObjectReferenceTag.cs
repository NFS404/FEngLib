﻿using System.IO;
using CoreLibraries.IO;
using FEngLib.Data;

namespace FEngLib.Tags
{
    public class ObjectReferenceTag : FrontendTag
    {
        public uint ReferencedObjectGuid { get; set; }
        public uint NameHash { get; set; }
        public FE_ObjectFlags Flags { get; set; }
        public int ResourceIndex { get; set; }

        public override void Read(BinaryReader br, FrontendChunkBlock chunkBlock, FrontendPackage package,
            ushort id,
            ushort length)
        {
            ReferencedObjectGuid = br.ReadUInt32();
            NameHash = br.ReadUInt32();
            Flags = br.ReadEnum<FE_ObjectFlags>();
            ResourceIndex = br.ReadInt32();
        }

        public ObjectReferenceTag(FrontendObject frontendObject) : base(frontendObject)
        {
        }
    }
}