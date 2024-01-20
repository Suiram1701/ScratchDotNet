﻿using ScratchDotNet.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Enums;

/// <summary>
/// Provides some sound modifiers
/// </summary>
public enum SoundEffect
{
    /// <summary>
    /// Modifies the frequency of the sound
    /// </summary>
    [EnumName("PITCH")]
    Pitch,

    /// <summary>
    /// Modifies the position of the sound in a stereo panorama
    /// </summary>
    [EnumName("PAN")]
    Panorama
}
