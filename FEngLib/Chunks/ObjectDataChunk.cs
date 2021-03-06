﻿using System;
using System.IO;
using FEngLib.Data;
using FEngLib.Objects;
using FEngLib.Tags;

namespace FEngLib.Chunks
{
    public class ObjectDataChunk : FrontendObjectChunk
    {
        public override FrontendObject Read(FrontendPackage package, ObjectReaderState readerState, BinaryReader reader)
        {
            FrontendObject newFrontendObject = FrontendObject;
            FrontendTagStream tagStream = new FrontendObjectTagStream(reader, readerState.CurrentChunkBlock, readerState.CurrentChunkBlock.Size);

            while (tagStream.HasTag())
            {
                FrontendTag tag = tagStream.NextTag(newFrontendObject);
                //Debug.WriteLine("OBJECT TAG {0}", tag);
                newFrontendObject = ProcessTag(newFrontendObject, tag);
            }

            return newFrontendObject;
        }

        private FrontendObject ProcessTag(FrontendObject frontendObject, FrontendTag tag)
        {
            switch (tag)
            {
                case ObjectTypeTag objectTypeTag:
                    return ProcessObjectTypeTag(frontendObject, objectTypeTag);
                case StringBufferTextTag stringBufferTextTag when frontendObject is FrontendString frontendString:
                    return ProcessStringBufferTextTag(frontendString, stringBufferTextTag);
                case StringBufferFormattingTag stringBufferFormattingTag when frontendObject is FrontendString frontendString:
                    return ProcessStringBufferFormattingTag(frontendString, stringBufferFormattingTag);
                case StringBufferLeadingTag stringBufferLeadingTag when frontendObject is FrontendString frontendString:
                    return ProcessStringBufferLeadingTag(frontendString, stringBufferLeadingTag);
                case StringBufferLabelHashTag stringBufferLabelHashTag when frontendObject is FrontendString frontendString:
                    return ProcessStringBufferLabelHashTag(frontendString, stringBufferLabelHashTag);
                case StringBufferMaxWidthTag stringBufferMaxWidthTag when frontendObject is FrontendString frontendString:
                    return ProcessStringBufferMaxWidthTag(frontendString, stringBufferMaxWidthTag);
                case ObjectHashTag objectHashTag:
                    frontendObject.NameHash = objectHashTag.Hash;
                    break;
                case ObjectReferenceTag objectReferenceTag:
                    ProcessObjectReferenceTag(frontendObject, objectReferenceTag);
                    break;
                case ImageInfoTag imageInfoTag when frontendObject is FrontendImage frontendImage:
                    ProcessImageInfoTag(frontendImage, imageInfoTag);
                    break;
                case ObjectDataTag objectDataTag:
                    ProcessObjectDataTag(frontendObject, objectDataTag);
                    break;
                default:
                    //Debug.WriteLine("WARN: Unprocessed tag - {0}", tag.GetType());
                    break;
            }

            return frontendObject;
        }

        private FrontendObject ProcessStringBufferMaxWidthTag(FrontendString frontendString, StringBufferMaxWidthTag stringBufferMaxWidthTag)
        {
            frontendString.MaxWidth = stringBufferMaxWidthTag.MaxWidth;
            return frontendString;
        }

        private FrontendObject ProcessStringBufferLabelHashTag(FrontendString frontendString, StringBufferLabelHashTag stringBufferLabelHashTag)
        {
            frontendString.Hash = stringBufferLabelHashTag.Hash;
            return frontendString;
        }

        private FrontendObject ProcessStringBufferLeadingTag(FrontendString frontendString, StringBufferLeadingTag stringBufferLeadingTag)
        {
            frontendString.Leading = stringBufferLeadingTag.Leading;
            return frontendString;
        }

        private FrontendObject ProcessStringBufferFormattingTag(FrontendString frontendString, StringBufferFormattingTag stringBufferFormattingTag)
        {
            frontendString.Formatting = stringBufferFormattingTag.Formatting;
            return frontendString;
        }

        private void ProcessObjectDataTag(FrontendObject frontendObject, ObjectDataTag objectDataTag)
        {
            frontendObject.Position = objectDataTag.Data.Position;
            frontendObject.Pivot = objectDataTag.Data.Pivot;
            frontendObject.Color = objectDataTag.Data.Color;
            frontendObject.Rotation = objectDataTag.Data.Rotation;
            frontendObject.Size = objectDataTag.Data.Size;

            if (objectDataTag.Data is FEImageData imageData)
            {
                FrontendImage image = (FrontendImage) frontendObject;
                image.UpperLeft = imageData.UpperLeft;
                image.LowerRight = imageData.LowerRight;
            }

            if (objectDataTag.Data is FEMultiImageData multiImageData)
            {
                FrontendMultiImage multiImage = (FrontendMultiImage) frontendObject;
                multiImage.PivotRotation = multiImageData.PivotRotation;
                multiImage.TopLeftUV = multiImageData.TopLeftUV;
                multiImage.BottomRightUV = multiImageData.BottomRightUV;
            }

            if (objectDataTag.Data is FEColoredImageData coloredImageData)
            {
                FrontendColoredImage coloredImage = (FrontendColoredImage) frontendObject;
                coloredImage.VertexColors = coloredImageData.VertexColors;
            }
        }

        private FrontendObject ProcessStringBufferTextTag(FrontendString frontendString, StringBufferTextTag stringBufferTextTag)
        {
            frontendString.Value = stringBufferTextTag.Value;
            return frontendString;
        }

        private FrontendObject ProcessObjectTypeTag(FrontendObject frontendObject, ObjectTypeTag objectTypeTag)
        {
            FrontendObject newInstance;

            switch (objectTypeTag.Type)
            {
                case FEObjType.FE_Image:
                    newInstance = new FrontendImage(frontendObject);
                    break;
                case FEObjType.FE_Group:
                    newInstance = new FrontendGroup(frontendObject);
                    break;
                case FEObjType.FE_String:
                    newInstance = new FrontendString(frontendObject);
                    break;
                case FEObjType.FE_MultiImage:
                    newInstance = new FrontendMultiImage(frontendObject);
                    break;
                case FEObjType.FE_ColoredImage:
                    newInstance = new FrontendColoredImage(frontendObject);
                    break;
                default:
                    throw new IndexOutOfRangeException($"cannot handle object type: {objectTypeTag.Type}");
            }

            newInstance.Type = objectTypeTag.Type;

            return newInstance;
        }

        private void ProcessImageInfoTag(FrontendImage frontendImage, ImageInfoTag imageInfoTag)
        {
            //Debug.WriteLine("FEObject {0:X8} has ImageInfo: value={1}", frontendImage.NameHash, imageInfoTag.ImageFlags);
            frontendImage.ImageFlags = imageInfoTag.ImageFlags;
        }

        private void ProcessObjectReferenceTag(FrontendObject frontendObject, ObjectReferenceTag objectReferenceTag)
        {
            frontendObject.Flags = objectReferenceTag.Flags;
            //Debug.WriteLine("FEObject {0:X8} references object {1:X8}; flags={2}", frontendObject.NameHash,
            //objectReferenceTag.ReferencedObjectGuid, objectReferenceTag.Flags);
        }

        public override FrontendChunkType GetChunkType()
        {
            return FrontendChunkType.ObjectData;
        }

        public ObjectDataChunk(FrontendObject frontendObject) : base(frontendObject)
        {
        }
    }
}