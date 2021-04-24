using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoConsoleApp
{
    public partial class TestClass
    {
        [Display(Name = "PropTest")]
        [Parameter]
        public int MyProperty { get; set; }

        [Display(Name = "PropTest3")]
        public int MyProperty2 { get; set; }
    }
}
