﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network.Interfaces
{
    public interface ICommandRegister
    {
        void RegisterCommands(IPacketHandler handler);
    }
}