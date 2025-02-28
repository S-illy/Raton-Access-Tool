using System;
using System.Collections.Generic;

namespace Stuff
{
    [Serializable]
    class PacketStuff
    {
        public Dictionary<string, object> Objects { get; set; }
    }
}
