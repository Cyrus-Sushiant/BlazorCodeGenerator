﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySourceGenerator
{
    public class BitProperty
    {
        public BitProperty(IPropertySymbol propertySymbol, bool isTwoWayBoundProperty)
        {
            PropertySymbol = propertySymbol;
            IsTwoWayBoundProperty = isTwoWayBoundProperty;
        }

        public IPropertySymbol PropertySymbol { get; set; }
        public bool IsTwoWayBoundProperty { get; set; }
    }
}
