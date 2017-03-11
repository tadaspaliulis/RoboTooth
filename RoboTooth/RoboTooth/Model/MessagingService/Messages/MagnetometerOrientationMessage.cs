﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.MessagingService.Messages
{
    public class MagnetometerOrientationMessage : RawMessage
    {
        public MagnetometerOrientationMessage( byte[] rawData) : base( (byte)RxMessageIdsEnum.EMagnetometerOrientation, rawData )
        {

        }

        public MagnetometerOrientationMessage(RawMessage other) : base(other)
        {

        }
    }
}
