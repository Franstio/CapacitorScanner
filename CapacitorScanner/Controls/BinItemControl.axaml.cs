using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using CapacitorScanner.Core.API.Model;
using System.Windows.Input;
using System.Reactive.Linq;
using System;
using CapacitorScanner.Core.Model;

namespace CapacitorScanner.Controls;

public class BinItemControl : TemplatedControl
{
    public static readonly StyledProperty<BinModel> BinProperty = AvaloniaProperty.Register<BinItemControl, BinModel>(nameof(Bin), defaultValue: new BinModel());

    public static readonly StyledProperty<ContainerBinModel?> ContainerProperty = AvaloniaProperty.Register<BinItemControl, ContainerBinModel?>(nameof(Container), defaultValue: null);
    public static readonly StyledProperty<ICommand> BinCommandProperty = AvaloniaProperty.Register<BinItemControl,ICommand>(nameof(BinCommand));

    private static readonly StyledProperty<bool> BinHeaderClassProperty = AvaloniaProperty.Register<BinItemControl, bool>(nameof(BinHeaderClass),defaultValue: false);

    private static readonly StyledProperty<bool> BinButtonClassProperty = AvaloniaProperty.Register<BinItemControl, bool>(nameof(BinButtonClass), defaultValue: false);

    private static readonly StyledProperty<bool> BinRedClassProperty = AvaloniaProperty.Register<BinItemControl, bool>(nameof(BinRedClass), defaultValue: false);

    private static readonly StyledProperty<bool> BinGrayClassProperty = AvaloniaProperty.Register<BinItemControl, bool>(nameof(BinGrayClass), defaultValue: true);
    public BinModel Bin
    {
        get => GetValue(BinProperty);
        set => SetValue(BinProperty, value);

    }
    public ICommand BinCommand
    {
        get => GetValue(BinCommandProperty);
        set => SetValue(BinCommandProperty, value); 
    }
    public ContainerBinModel? Container
    {
        get => GetValue(ContainerProperty);
        set => SetValue(ContainerProperty, value);
    }
    public bool BinHeaderClass { get => GetValue(BinHeaderClassProperty); set=>SetValue(BinHeaderClassProperty,value); }
    public bool BinButtonClass { get=>GetValue(BinButtonClassProperty); set=>SetValue(BinButtonClassProperty,value); }
    public bool BinRedClass { get => GetValue(BinRedClassProperty); set => SetValue(BinRedClassProperty, value); }

    public bool BinGrayClass { get => GetValue(BinGrayClassProperty); set => SetValue(BinGrayClassProperty, value); }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
    }
    public BinItemControl() 
    {
        ContainerProperty.Changed.Subscribe(item =>
        {
            var newvalue = item.NewValue.Value;
            try
            {
                if (Bin is null)
                    return;
                BinHeaderClass = newvalue is not null && newvalue.scraptype_name == Bin.WasteType;
                BinButtonClass = newvalue is not null && newvalue.scraptype_name == Bin.WasteType;
                BinRedClass = newvalue is not null && !BinButtonClass && !BinHeaderClass;
                BinGrayClass = !BinHeaderClass && !BinRedClass;
            }
            catch { }
        });
    }
}