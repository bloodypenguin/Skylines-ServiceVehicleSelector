﻿using System;

namespace ServiceVehicleSelector.RedirectionFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class RedirectReverseAttribute : RedirectAttribute
    {
        public RedirectReverseAttribute() : base(false)
        {
        }

        public RedirectReverseAttribute(bool onCreated) : base(onCreated)
        {
        }
    }
}