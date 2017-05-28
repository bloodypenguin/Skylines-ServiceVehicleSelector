﻿using System;

namespace ServiceVehicleSelector2.RedirectionFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class RedirectMethodAttribute : RedirectAttribute
    {
        public RedirectMethodAttribute() : base(false)
        {
        }

        public RedirectMethodAttribute(bool onCreated) : base(onCreated)
        {
        }
    }
}
