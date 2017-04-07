// Copyright 2016, 2015, 2014 Matthias Koch
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
