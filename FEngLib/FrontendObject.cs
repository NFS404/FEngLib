﻿using System.Collections.Generic;
using FEngLib.Data;
using FEngLib.Structures;

namespace FEngLib
{
    public class FrontendObject
    {
        public FEObjType Type { get; set; }
        public FE_ObjectFlags Flags { get; set; }

        public uint NameHash { get; set; }
        public List<FrontendScript> Scripts { get; set; }
        public List<FEMessageResponse> MessageResponses { get; set; }
        public FrontendPackage Package { get; set; }

        public FEColor Color { get; set; }
        public FEVector3 Pivot { get; set; }
        public FEVector3 Position { get; set; }
        public FEQuaternion Rotation { get; set; }
        public FEVector3 Size { get; set; }

        public FrontendObject()
        {
            Scripts = new List<FrontendScript>();
            MessageResponses = new List<FEMessageResponse>();
        }

        public FrontendObject(FrontendObject original) : this()
        {
            Type = original.Type;
            NameHash = original.NameHash;
            Flags = original.Flags;
            Package = original.Package;
        }
    }
}