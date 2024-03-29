﻿using System;

namespace Ucommerce.Sitecore.Install
{
    public interface ISitecoreVersionChecker
    {
        bool IsEqualOrGreaterThan(Version version);
        bool IsLowerThan(Version version);
        bool SupportsSpeakApps();
    }
}
