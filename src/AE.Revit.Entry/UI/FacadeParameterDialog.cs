using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AE.Revit.Core;

namespace AE.Revit.Entry.UI;
public sealed class FacadeParameterDialog : Window
{
    private readonly TextBox _width=new(){Text="1200"},_height=new(){Text="1200"},_horizontal=new(){Text="150"},_vertical=new(){Text="150"},_depth=new(){Text="50"};
    public FacadeParameterDialog() { Title="Facade parameters (mm)"; Width=300; Height=300; var p=new StackPanel{Margin=new Thickness(16)}; Add(p,"Width",_width); Add(p,"Height",_height); Add(p,"Horizontal spacing",_horizontal); Add(p,"Vertical spacing",_vertical); Add(p,"Depth",_depth); var ok=new Button{Content="Generate",IsDefault=true}; ok.Click=(_,_)=> { if(TryRead(out var values)) { Parameters=values; DialogResult=true; } }; p.Children.Add(ok); Content=p; }
    public FacadeParameters? Parameters {get;private set;}
    private static void Add(Panel p,string label,TextBox box) { p.Children.Add(new TextBlock{Text=label}); p.Children.Add(box); }
    private bool TryRead(out FacadeParameters values) { values=new FacadeParameters(); if(!double.TryParse(_width.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var w)||!double.TryParse(_height.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var h)||!double.TryParse(_horizontal.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var x)||!double.TryParse(_vertical.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var y)||!double.TryParse(_depth.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out var d)) { MessageBox.Show("Enter numeric millimetre values."); return false; } values.PanelWidthMillimetres=w;values.PanelHeightMillimetres=h;values.HorizontalSpacingMillimetres=x;values.VerticalSpacingMillimetres=y;values.DepthMillimetres=d; if(values.Validate().Count>0){MessageBox.Show(values.Validate()[0].Message);return false;}return true; }
}
