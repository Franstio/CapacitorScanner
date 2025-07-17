using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CapacitorScanner.Behaviours
{
    public class LoadBehaviour { }
    public static  class ControlOnLoadBehaviour
    {
        public static readonly AttachedProperty<ICommand> OnLoadProperty = AvaloniaProperty.RegisterAttached<LoadBehaviour, Control, ICommand>("OnLoad");

        public static void SetOnLoad(AvaloniaObject element, ICommand value) => element.SetValue(OnLoadProperty,value);

        public static ICommand GetOnLoad(AvaloniaObject element) => element.GetValue(OnLoadProperty);

        static ControlOnLoadBehaviour()
        {
            OnLoadProperty.Changed.Subscribe((change) =>
            {
                if (change.Sender is Control control)
                {
                    control.AttachedToVisualTree += (_, _) =>
                    {
                        var cmd = GetOnLoad(control);
                        bool canExecute = cmd is not null && cmd?.CanExecute(null) == true;
                        if (canExecute)
                            cmd!.Execute(null);
                    };
                }
            });
        }
    }
}
