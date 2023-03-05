using System;
using System.Collections.Generic;

namespace NamedPipeConnection
{
    public class ParametersDataContainer
    {
        public Dictionary<int, string> StringParameters;
        public Dictionary<int, long> NumParameters;
        public Dictionary<int, bool> BoolParameters;
        public Dictionary<int, Guid> KlAkObjectParameters;
        public Dictionary<int, BoxedObjectWrapper> BoxedObjects;
    }
}