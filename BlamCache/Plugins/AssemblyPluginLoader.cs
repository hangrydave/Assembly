﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace ExtryzeDLL.Plugins
{
    public static class AssemblyPluginLoader
    {
        /// <summary>
        /// Parses an XML plugin, calling the corresponding method in
        /// IPluginVisitor for each XML tag it encounters.
        /// </summary>
        /// <param name="reader">The XmlReader to read the plugin XML from.</param>
        /// <param name="visitor">The IPluginVisitor to call for each XML tag.</param>
        public static void LoadPlugin(XmlReader reader, IPluginVisitor visitor)
        {
            if (!reader.ReadToNextSibling("plugin"))
                throw new ArgumentException("The XML file is missing a <plugin> tag.");

            int baseSize = 0;
            if (reader.MoveToAttribute("baseSize"))
                baseSize = ParseInt(reader.Value);

            if (visitor.EnterPlugin(baseSize))
            {
                ReadElements(reader, true, visitor);
                visitor.LeavePlugin();
            }
        }

        private static void ReadElements(XmlReader reader, bool topLevel, IPluginVisitor visitor)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (topLevel)
                        HandleTopLevelElement(reader, visitor);
                    else
                        HandleElement(reader, visitor);
                }
            }
        }

        private static void HandleTopLevelElement(XmlReader reader, IPluginVisitor visitor)
        {
            if (reader.Name == "revisions" && visitor.EnterRevisions())
            {
                ReadRevisions(reader.ReadSubtree(), visitor);
                visitor.LeaveRevisions();
            }
            else
            {
                HandleElement(reader, visitor);
            }
        }

        private static void HandleElement(XmlReader reader, IPluginVisitor visitor)
        {
            switch (reader.Name)
            {
                case "comment":
                    ReadComment(reader, visitor);
                    break;
                default:
                    HandleValueElement(reader, reader.Name, visitor);
                    break;
            }
        }

        private static void ReadComment(XmlReader reader, IPluginVisitor visitor)
        {
            string title = "Comment";

            if (reader.MoveToAttribute("title"))
                title = reader.Value;

            reader.MoveToElement();
            string text = reader.ReadElementContentAsString();
            visitor.VisitComment(title, text);
        }

        /// <summary>
        /// Handles an element which describes how a value
        /// should be read from the cache file.
        /// </summary>
        /// <param name="reader">The XmlReader that read the element.</param>
        /// <param name="elementName">The element's name.</param>
        /// <param name="visitor">The IPluginVisitor to call to.</param>
        private static void HandleValueElement(XmlReader reader, string elementName, IPluginVisitor visitor)
        {
            string name = "Unknown";
            uint offset = 0;
            uint pluginLine = (uint)(reader as IXmlLineInfo).LineNumber;
            bool visible = true;

            if (reader.MoveToAttribute("name"))
                name = reader.Value;
            if (reader.MoveToAttribute("offset"))
                offset = ParseUInt(reader.Value);
            if (reader.MoveToAttribute("visible"))
                visible = ParseBool(reader.Value);

            reader.MoveToElement();
            switch (elementName.ToLower()) // FIXME: Using ToLower() here violates XML standards
            {
                case "uint8":
                    visitor.VisitUInt8(name, offset, visible, pluginLine);
                    break;
                case "int8":
                    visitor.VisitInt8(name, offset, visible, pluginLine);
                    break;
                case "uint16":
                    visitor.VisitUInt16(name, offset, visible, pluginLine);
                    break;
                case "int16":
                    visitor.VisitInt16(name, offset, visible, pluginLine);
                    break;
                case "uint32":
                    visitor.VisitUInt32(name, offset, visible, pluginLine);
                    break;
                case "int32":
                    visitor.VisitInt32(name, offset, visible, pluginLine);
                    break;
                case "float32": case "float":
                    visitor.VisitFloat32(name, offset, visible, pluginLine);
                    break;
                case "undefined":
                    visitor.VisitUndefined(name, offset, visible, pluginLine);
                    break;
                case "vector3":
                    visitor.VisitVector3(name, offset, visible, pluginLine);
                    break;
                case "stringid":
                    visitor.VisitStringID(name, offset, visible, pluginLine);
                    break;
                case "tagref":
                    ReadTagRef(reader, name, offset, visible, visitor, pluginLine);
                    break;

                case "range":
                    ReadRange(reader, name, offset, visible, visitor, pluginLine);
                    break;

                case "ascii":
                    ReadAscii(reader, name, offset, visible, visitor, pluginLine);
                    break;

                case "utf16":
                    ReadUtf16(reader, name, offset, visible, visitor, pluginLine);
                    break;

                case "bitfield8":
                    if (visitor.EnterBitfield8(name, offset, visible, pluginLine))
                        ReadBits(reader, visitor);
                    else
                        reader.Skip();
                    break;
                case "bitfield16":
                    if (visitor.EnterBitfield16(name, offset, visible, pluginLine))
                        ReadBits(reader, visitor);
                    else
                        reader.Skip();
                    break;
                case "bitfield32":
                    if (visitor.EnterBitfield32(name, offset, visible, pluginLine))
                        ReadBits(reader, visitor);
                    else
                        reader.Skip();
                    break;

                case "enum8":
                    if (visitor.EnterEnum8(name, offset, visible, pluginLine))
                        ReadOptions(reader, visitor);
                    else
                        reader.Skip();
                    break;
                case "enum16":
                    if (visitor.EnterEnum16(name, offset, visible, pluginLine))
                        ReadOptions(reader, visitor);
                    else
                        reader.Skip();
                    break;
                case "enum32":
                    if (visitor.EnterEnum32(name, offset, visible, pluginLine))
                        ReadOptions(reader, visitor);
                    else
                        reader.Skip();
                    break;

                case "color8": case "colour8":
                case "color16": case "colour16":
                case "color24": case "colour24":
                case "color32": case "colour32":
                    visitor.VisitColorInt(name, offset, visible, ReadColorFormat(reader), pluginLine);
                    break;
                case "colorf": case "colourf":
                    visitor.VisitColorF(name, offset, visible, ReadColorFormat(reader), pluginLine);
                    break;

                case "dataref":
                    visitor.VisitDataReference(name, offset, visible, pluginLine);
                    break;

                case "reflexive":
                    ReadReflexive(reader, name, offset, visible, visitor, pluginLine);
                    break;

                case "raw":
                    ReadRaw(reader, name, offset, visible, visitor, pluginLine);
                    break;

                default:
                    throw new ArgumentException("Unknown element \"" + elementName + "\"." + PositionInfo(reader));
            }
        }

        private static void ReadRevisions(XmlReader reader, IPluginVisitor visitor)
        {
            reader.ReadStartElement();
            while (reader.ReadToFollowing("revision"))
                visitor.VisitRevision(ReadRevision(reader));
        }

        private static PluginRevision ReadRevision(XmlReader reader)
        {
            string author = "", description;
            int version = 1;

            if (reader.MoveToAttribute("author"))
                author = reader.Value;
            if (reader.MoveToAttribute("version"))
                version = ParseInt(reader.Value);

            reader.MoveToElement();
            description = reader.ReadElementContentAsString();
            return new PluginRevision(author, version, description);
        }


        private static void ReadRange(XmlReader reader, string name, uint offset, bool visible, IPluginVisitor visitor, uint pluginLine)
        {
            double min = 0.0;
            double max = 0.0;
            double largeChange = 0.0;
            double smallChange = 0.0;
            string type = "int32";

            if (reader.MoveToAttribute("min"))
                min = double.Parse(reader.Value);
            if (reader.MoveToAttribute("max"))
                max = double.Parse(reader.Value);
            if (reader.MoveToAttribute("smallStep"))
                smallChange = double.Parse(reader.Value);
            if (reader.MoveToAttribute("largeStep"))
                largeChange = double.Parse(reader.Value);
            if (reader.MoveToAttribute("type"))
                type = reader.Value.ToLower();

            visitor.VisitRange(name, offset, visible, type, min, max, smallChange, largeChange, pluginLine);
        }

        private static void ReadTagRef(XmlReader reader, string name, uint offset, bool visible, IPluginVisitor visitor, uint pluginLine)
        {
            bool showJumpTo = true;
            bool withClass = true;

            if (reader.MoveToAttribute("showJumpTo"))
                showJumpTo = ParseBool(reader.Value);
            if (reader.MoveToAttribute("withClass"))
                withClass = ParseBool(reader.Value);

            visitor.VisitTagReference(name, offset, visible, withClass, showJumpTo, pluginLine);
        }

        private static void ReadAscii(XmlReader reader, string name, uint offset, bool visible, IPluginVisitor visitor, uint pluginLine)
        {
            // Both "size" and "length" are accepted here because they are the same
            // with ASCII strings, but "size" should be preferred because it's less ambiguous
            // and <utf16> only supports "size"
            int size = 0;
            if (reader.MoveToAttribute("size") || reader.MoveToAttribute("length"))
                size = ParseInt(reader.Value);

            visitor.VisitAscii(name, offset, visible, size, pluginLine);
        }

        private static void ReadUtf16(XmlReader reader, string name, uint offset, bool visible, IPluginVisitor visitor, uint pluginLine)
        {
            int size = 0;
            if (reader.MoveToAttribute("size"))
                size = ParseInt(reader.Value);

            visitor.VisitUtf16(name, offset, visible, size, pluginLine);
        }

        private static void ReadBits(XmlReader reader, IPluginVisitor visitor)
        {
            XmlReader subtree = reader.ReadSubtree();

            subtree.ReadStartElement();
            while (subtree.ReadToNextSibling("bit"))
                ReadBit(subtree, visitor);

            visitor.LeaveBitfield();
        }
        private static void ReadBit(XmlReader reader, IPluginVisitor visitor)
        {
            string name = "Unknown";
            int index = 0;

            if (reader.MoveToAttribute("name"))
                name = reader.Value;
            if (!reader.MoveToAttribute("index"))
                throw new ArgumentException("Bit definitions must have an index." + PositionInfo(reader));
            index = ParseInt(reader.Value);

            visitor.VisitBit(name, index);
        }
        private static void ReadOptions(XmlReader reader, IPluginVisitor visitor)
        {
            XmlReader subtree = reader.ReadSubtree();

            subtree.ReadStartElement();
            while (subtree.ReadToNextSibling("option"))
                ReadOption(subtree, visitor);

            visitor.LeaveEnum();
        }
        private static void ReadOption(XmlReader reader, IPluginVisitor visitor)
        {
            string name = "Unknown";
            int value = 0;

            if (reader.MoveToAttribute("name"))
                name = reader.Value;
            if (reader.MoveToAttribute("value"))
                value = ParseInt(reader.Value);

            visitor.VisitOption(name, value);
        }

        private static string ReadColorFormat(XmlReader reader)
        {
            string format;

            if (!reader.MoveToAttribute("format"))
                throw new ArgumentException("Color tags must have a format attribute." + PositionInfo(reader));
            format = reader.Value.ToLower();

            for (int i = 0; i < format.Length; i++)
            {
                char ch = format[i];
                if (ch != 'r' && ch != 'g' && ch != 'b' && ch != 'a')
                    throw new ArgumentException("Invalid color format: \"" + format + "\"" + PositionInfo(reader));
            }

            return format;
        }

        private static void ReadReflexive(XmlReader reader, string name, uint offset, bool visible, IPluginVisitor visitor, uint pluginLine)
        {
            uint entrySize;

            if (!reader.MoveToAttribute("entrySize"))
                throw new ArgumentException("Reflexives must have an entrySize attribute." + PositionInfo(reader));
            entrySize = ParseUInt(reader.Value);

            if (visitor.EnterReflexive(name, offset, visible, entrySize, pluginLine))
            {
                reader.MoveToElement();
                XmlReader subtree = reader.ReadSubtree();

                subtree.ReadStartElement();
                ReadElements(subtree, false, visitor);
                visitor.LeaveReflexive();
            }
            else
            {
                reader.Skip();
            }
        }

        private static void ReadRaw(XmlReader reader, string name, uint offset, bool visible, IPluginVisitor visitor, uint pluginLine)
        {
            int size;

            if (!reader.MoveToAttribute("size"))
                throw new ArgumentException("Raw data blocks must have a size attribute." + PositionInfo(reader));
            size = ParseInt(reader.Value);

            visitor.VisitRawData(name, offset, visible, size, pluginLine);
        }

        private static string PositionInfo(XmlReader reader)
        {
            IXmlLineInfo info = reader as IXmlLineInfo;
            if (info != null)
                return string.Format(" Line {0}, position {1}.", info.LineNumber, info.LinePosition);
            return "";
        }

        private static int ParseInt(string str)
        {
            if (str.StartsWith("0x"))
                return int.Parse(str.Substring(2), NumberStyles.HexNumber);
            return int.Parse(str);
        }

        private static uint ParseUInt(string str)
        {
            if (str.StartsWith("0x"))
                return uint.Parse(str.Substring(2), NumberStyles.HexNumber);
            return uint.Parse(str);
        }

        private static bool ParseBool(string str)
        {
            return (str == "1" || str.ToLower() == "true");
        }
    }
}
