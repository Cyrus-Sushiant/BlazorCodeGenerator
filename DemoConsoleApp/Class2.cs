using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoConsoleApp
{
    public class Class2
    {
        public int MyProperty { get; set; }
    }

    public class Class3
    {
        [Display(Name = "PropTest3")]
        public int MyProperty { get; set; }
    }

    public partial class Class4
    {
        [Display(Name = "PropTest")]
        [Parameter]
        public int MyProperty { get; set; }

        [Parameter]
        public int MyProperty2 { get; set; }
    }
}
