﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Spacestro.Cloud.Library;

namespace Spacestro.Cloud
{
    class Program
    {
        static void Main(string[] args)
        {
            Cloud cloud = new Cloud("spacestro", 8383);
            cloud.Start();
        }
    }
}
