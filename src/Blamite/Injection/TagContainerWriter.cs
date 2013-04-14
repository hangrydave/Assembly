﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blamite.IO;

namespace Blamite.Injection
{
    public static class TagContainerWriter
    {
        public static void WriteTagContainer(TagContainer tags, IWriter writer)
        {
            ContainerWriter container = new ContainerWriter(writer);
            container.StartBlock("tagc", 0);

            WriteDataBlocks(tags, container, writer);
            WriteTags(tags, container, writer);
            WriteResourcePages(tags, container, writer);
            WriteResources(tags, container, writer);

            container.EndBlock();
        }

        private static void WriteDataBlocks(TagContainer tags, ContainerWriter container, IWriter writer)
        {
            foreach (var dataBlock in tags.DataBlocks)
            {
                container.StartBlock("data", 0);

                // Main data
                writer.WriteUInt32(dataBlock.OriginalAddress);
                WriteByteArray(dataBlock.Data, writer);

                // Address fixups
                writer.WriteInt32(dataBlock.AddressFixups.Count);
                foreach (var blockRef in dataBlock.AddressFixups)
                {
                    writer.WriteUInt32(blockRef.OriginalAddress);
                    writer.WriteInt32(blockRef.WriteOffset);
                }

                // Tagref fixups
                writer.WriteInt32(dataBlock.TagFixups.Count);
                foreach (var tagRef in dataBlock.TagFixups)
                {
                    writer.WriteUInt32(tagRef.OriginalIndex.Value);
                    writer.WriteInt32(tagRef.WriteOffset);
                }

                // Resource reference fixups
                writer.WriteInt32(dataBlock.ResourceFixups.Count);
                foreach (var resourceRef in dataBlock.ResourceFixups)
                {
                    writer.WriteUInt32(resourceRef.OriginalIndex.Value);
                    writer.WriteInt32(resourceRef.WriteOffset);
                }

                container.EndBlock();
            }
        }

        private static void WriteTags(TagContainer tags, ContainerWriter container, IWriter writer)
        {
            foreach (var tag in tags.Tags)
            {
                container.StartBlock("tag!", 0);

                writer.WriteUInt32(tag.OriginalIndex.Value);
                writer.WriteUInt32(tag.OriginalAddress);
                writer.WriteInt32(tag.Class);
                writer.WriteAscii(tag.Name);

                container.EndBlock();
            }
        }

        private static void WriteResourcePages(TagContainer tags, ContainerWriter container, IWriter writer)
        {
            foreach (var page in tags.ResourcePages)
            {
                container.StartBlock("rspg", 0);

                writer.WriteInt32(page.Index);
                writer.WriteByte(page.Flags);
                writer.WriteAscii(page.FilePath);
                writer.WriteInt32(page.Offset);
                writer.WriteInt32(page.UncompressedSize);
                writer.WriteByte((byte)page.CompressionMethod);
                writer.WriteInt32(page.CompressedSize);
                writer.WriteUInt32(page.Checksum);
                WriteByteArray(page.Hash1, writer);
                WriteByteArray(page.Hash2, writer);
                WriteByteArray(page.Hash3, writer);
                writer.WriteInt32(page.Unknown1);
                writer.WriteInt32(page.Unknown2);
                writer.WriteInt32(page.Unknown3);

                container.EndBlock();
            }
        }

        private static void WriteResources(TagContainer tags, ContainerWriter container, IWriter writer)
        {
            foreach (var resource in tags.Resources)
            {
                container.StartBlock("rsrc", 0);

                writer.WriteUInt32(resource.Index.Value);
                writer.WriteUInt32(resource.Flags);
                writer.WriteInt32(resource.Type);
                WriteByteArray(resource.Info, writer);
                writer.WriteUInt32((resource.ParentTag != null) ? resource.ParentTag.Index.Value : 0xFFFFFFFF);
                if (resource.Location != null)
                {
                    writer.WriteByte(1);
                    writer.WriteInt32(resource.Location.PrimaryPage.Index);
                    writer.WriteInt32(resource.Location.PrimaryOffset);
                    writer.WriteInt32(resource.Location.PrimaryUnknown);
                    writer.WriteInt32(resource.Location.SecondaryPage.Index);
                    writer.WriteInt32(resource.Location.SecondaryOffset);
                    writer.WriteInt32(resource.Location.SecondaryUnknown);
                }
                else
                {
                    writer.WriteByte(0);
                }
                writer.WriteInt32(resource.Unknown1);
                writer.WriteInt32(resource.Unknown2);
                writer.WriteInt32(resource.Unknown3);

                writer.WriteInt32(resource.ResourceFixups.Count);
                foreach (var fixup in resource.ResourceFixups)
                {
                    writer.WriteInt32(fixup.Offset);
                    writer.WriteUInt32(fixup.Address);
                }

                writer.WriteInt32(resource.DefinitionFixups.Count);
                foreach (var fixup in resource.DefinitionFixups)
                {
                    writer.WriteInt32(fixup.Offset);
                    writer.WriteInt32(fixup.Type);
                }

                container.EndBlock();
            }
        }

        private static void WriteByteArray(byte[] data, IWriter writer)
        {
            if (data != null)
            {
                writer.WriteInt32(data.Length);
                writer.WriteBlock(data);
            }
            else
            {
                writer.WriteInt32(0);
            }
        }
    }
}
