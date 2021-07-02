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
        [CascadingParameter]
        public int MyProperty2 { get; set; }
    }

    public partial class Class4
    {
        [Display(Name = "PropTest")]
        [Parameter]
        public int MyProperty { get; set; }

        [Parameter]
        public int MyProperty2 { get; set; }
    }

    public partial class Class5 : ComponentBase
    {
        [Display(Name = "PropTest3")]
        public int MyProperty { get; set; }
        [CascadingParameter]
        public int MyProperty2 { get; set; }
    }

    public abstract partial class Class6 : ComponentBase
    {
        [Display(Name = "PropTest3")]
        public int MyProperty { get; set; }
        [CascadingParameter]
        public int MyProperty2 { get; set; }
    }

    public partial class BitBasicList<TItem> : ComponentBase
    {
        public bool IsCheckedHasBeenSet { get; set; }
        private bool isChecked;

        [Display(Name = "PropTest")]
        [Parameter]
        public int MyProperty { get; set; }

        [Parameter]
        public int MyProperty2 { get; set; }

        [Parameter]
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (value == isChecked) return;
                isChecked = value;
                _ = IsCheckedChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<bool> IsCheckedChanged { get; set; }
    }
}


//public override Task SetParametersAsync(ParameterView parameters)
//{
//    IsCheckedHasBeenSet = false;

//    foreach (ParameterValue parameter in parameters)
//    {
//        switch (parameter.Name)
//        {
//            case nameof(IsChecked):
//                IsCheckedHasBeenSet = true;
//                IsChecked = (bool)parameter.Value;
//                break;
//            case nameof(IsCheckedChanged):
//                IsCheckedChanged = (EventCallback<bool>)parameter.Value;
//                break;
//            case nameof(BoxSide):
//                BoxSide = (BoxSide)parameter.Value;
//                break;
//            case nameof(IsIndeterminate):
//                IsIndeterminate = (bool)parameter.Value;
//                break;
//            case nameof(IsIndeterminateChanged):
//                IsIndeterminateChanged = (EventCallback<bool>)parameter.Value;
//                break;
//            case nameof(ChildContent):
//                ChildContent = (RenderFragment?)parameter.Value;
//                break;
//            case nameof(OnChange):
//                OnChange = (EventCallback<bool>)parameter.Value;
//                break;
//        }
//    }
//    return base.SetParametersAsync(parameters);
//}