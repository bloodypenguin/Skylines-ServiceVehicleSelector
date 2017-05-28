﻿using System;
using System.Reflection;

namespace ServiceVehicleSelector.RedirectionFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class IgnoreConditionAttribute : Attribute
    {
        public abstract bool IsIgnored(MemberInfo methodInfo);
    }
}