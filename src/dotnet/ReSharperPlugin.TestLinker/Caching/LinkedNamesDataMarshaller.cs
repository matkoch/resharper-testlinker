// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Util.PersistentMap;

namespace TestLinker.Caching
{
    public class LinkedNamesDataMarshaller : IUnsafeMarshaller<LinkedNamesData>
    {
        #region IUnsafeMarshaller<LinkedNamesData>

        public void Marshal ([NotNull] UnsafeWriter writer, [NotNull] LinkedNamesData value)
        {
            writer.Write(value.Count);
            foreach (var map in value)
            {
                writer.Write(map.Key);
                writer.Write(map.Value.Count);
                foreach (var name in map.Value)
                    writer.Write(name);
            }
        }

        public LinkedNamesData Unmarshal ([NotNull] UnsafeReader reader)
        {
            var linkData = new LinkedNamesData();
            var count = reader.ReadInt();
            for (var i = 0; i < count; i++)
            {
                var sourceType = reader.ReadString();
                var listCount = reader.ReadInt();
                var linkedTypes = Enumerable.Range(start: 0, count: listCount).Select(x => reader.ReadString());
                linkData.AddValueRange(sourceType, linkedTypes);
            }
            return linkData;
        }

        #endregion
    }
}
