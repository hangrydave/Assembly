﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtryzeDLL.Blam;
using ExtryzeDLL.Util;

namespace ExtryzeDLL.Blam
{
    /// <summary>
    /// Implementation of IStringIDResolver that uses set definitions to translate stringIDs into array indices.
    /// </summary>
    /// <seealso cref="StringID"/>
    public class StringIDSetResolver : IStringIDResolver
    {
        private SortedList<short, StringIDSet> _setsByID = new SortedList<short, StringIDSet>();
        private SortedList<int, StringIDSet> _setsByGlobalIndex = new SortedList<int, StringIDSet>();

        /// <summary>
        /// Registers a stringID set to use to translate stringIDs.
        /// </summary>
        /// <param name="id">The set's ID number.</param>
        /// <param name="minIndex">The minimum index that a stringID must have in order to be counted as part of the set.</param>
        /// <param name="globalIndex">The index of the set's first string in the global stringID table.</param>
        public void RegisterSet(short id, ushort minIndex, int globalIndex)
        {
            StringIDSet set = new StringIDSet(id, minIndex, globalIndex);
            _setsByID[id] = set;
            _setsByGlobalIndex[globalIndex] = set;
        }

        /// <summary>
        /// Translates a stringID into an index into the global debug strings array.
        /// </summary>
        /// <param name="id">The StringID to translate.</param>
        /// <returns>The index of the string in the global debug strings array.</returns>
        public int StringIDToIndex(StringID id)
        {
            int closestSetIndex = ListSearching.BinarySearch<short>(_setsByID.Keys, id.Set);
            if (closestSetIndex < 0)
            {
                // BinarySearch returns the bitwise complement of the index of the next highest value if not found
                // So, use the set that comes before it...
                closestSetIndex = ~closestSetIndex - 1;
                if (closestSetIndex < 0)
                    return id.Value; // No previous set defined - just return the handle
            }

            // If the index falls outside the set's min value, then put it into the previous set
            if (id.Index < _setsByID.Values[closestSetIndex].MinIndex)
            {
                closestSetIndex--;
                if (closestSetIndex < 0)
                    return id.Value;
            }

            // Calculate the array index by subtracting the value of the first ID in the set
            // and then adding the index in the global array of the set's first string
            StringIDSet set = _setsByID.Values[closestSetIndex];
            StringID firstId = new StringID(set.ID, set.MinIndex);
            return id.Value - firstId.Value + set.GlobalIndex;
        }

        /// <summary>
        /// Translates a string index into a stringID which can be written to the file.
        /// </summary>
        /// <param name="index">The index of the string in the global strings array.</param>
        /// <returns>The stringID associated with the index.</returns>
        public StringID IndexToStringID(int index)
        {
            // Determine which set the index belongs to by finding the set with the closest global index that comes before it
            int closestSetIndex = ListSearching.BinarySearch<int>(_setsByGlobalIndex.Keys, index);
            if (closestSetIndex < 0)
            {
                // BinarySearch returns the bitwise complement of the index of the next highest value if not found
                // So negate it and subtract 1 to get the closest global index that comes before it
                closestSetIndex = ~closestSetIndex - 1;
                if (closestSetIndex < 0)
                    return new StringID(index); // No previous set defined - just return the index
            }

            // Calculate the StringID by subtracting the set's global array index
            // and then adding the value of the first stringID in the set
            StringIDSet set = _setsByGlobalIndex.Values[closestSetIndex];
            StringID firstId = new StringID(set.ID, set.MinIndex);
            return new StringID(index - set.GlobalIndex + firstId.Value);
        }

        /// <summary>
        /// A stringID set definition.
        /// </summary>
        private class StringIDSet
        {
            /// <summary>
            /// Constructs a new set definition.
            /// </summary>
            /// <param name="id">The set's ID number.</param>
            /// <param name="minIndex">The minimum index that a stringID must have in order to be counted as part of the set.</param>
            /// <param name="globalIndex">The index of the set's first string in the global stringID table.</param>
            public StringIDSet(short id, ushort minIndex, int globalIndex)
            {
                ID = id;
                MinIndex = minIndex;
                GlobalIndex = globalIndex;
            }

            /// <summary>
            /// The set's ID number.
            /// </summary>
            public short ID { get; private set; }

            /// <summary>
            /// The minimum index that a stringID must have in order to be counted as part of the set.
            /// </summary>
            public ushort MinIndex { get; private set; }

            /// <summary>
            /// The index of the set's first string in the global stringID table.
            /// </summary>
            public int GlobalIndex { get; private set; }
        }
    }
}
