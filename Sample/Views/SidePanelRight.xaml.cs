using System.Windows;
using System.Windows.Controls;
using Sample.Models;

namespace Sample.Views;

public partial class SidePanelRight : UserControl
{
    public SidePanelRight()
    {
        InitializeComponent();

        ShapePresetsComboBox.ItemsSource = new[]
        {
            new ShapePreset("Default", "Default"),
            new ShapePreset("PreFluent", "No Rounding, Thicker Borders"),
        };

        if (App.IsMultiThreaded)
        {
            ColorPresetsComboBox.Visibility = Visibility.Collapsed;
            ShapePresetsComboBox.Visibility = Visibility.Collapsed;
            AccentColorPicker.Visibility = Visibility.Collapsed;
        }

        SizeChanged += (_, _) =>
        {
            //TODO 
        };
    }
}