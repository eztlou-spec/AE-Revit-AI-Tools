using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AE.Revit.Geometry.Domes;

namespace AE.Revit.Entry.UI;

internal sealed class DomeInput
{
    public ProDomeSettings Settings {get;}=new ProDomeSettings();
    public double SectionMm=100,X,Y,Z;
    public string Serialize()
    {
        var s=Settings;
        return string.Join("|",new[]{ "AE1",s.Type.ToString(),F(s.Diameter),F(s.Height),F(s.Segments),F(s.Rings),F(s.Opening),F(s.LayerDepth),F(s.Aspect),F(SectionMm),F(X),F(Y),F(Z) });
    }
    private static string F(double x)=>x.ToString("R",CultureInfo.InvariantCulture);
    private static double D(string x)=>double.Parse(x,CultureInfo.InvariantCulture);
    public static DomeInput Parse(string data)
    {
        var a=data.Split('|');if(a.Length!=13||a[0]!="AE1")throw new FormatException("Invalid dome metadata.");
        var d=new DomeInput();var s=d.Settings;
        s.Type=(ProDomeType)Enum.Parse(typeof(ProDomeType),a[1]);s.Diameter=D(a[2]);s.Height=D(a[3]);s.Segments=(int)D(a[4]);s.Rings=(int)D(a[5]);s.Opening=D(a[6]);s.LayerDepth=D(a[7]);s.Aspect=D(a[8]);d.SectionMm=D(a[9]);d.X=D(a[10]);d.Y=D(a[11]);d.Z=D(a[12]);return d;
    }
}
internal sealed class SmartDomeWindow : Window
{
    private readonly DomeInput input;
    private readonly ComboBox types=new ComboBox();
    private readonly TextBox diameter=new TextBox(),height=new TextBox(),segments=new TextBox(),rings=new TextBox(),opening=new TextBox(),depth=new TextBox(),aspect=new TextBox(),section=new TextBox();
    private readonly Canvas preview=new Canvas { Width=480,Height=340,Background=Brushes.WhiteSmoke,ClipToBounds=true };
    private readonly TextBlock status=new TextBlock { TextWrapping=TextWrapping.Wrap,Margin=new Thickness(0,10,0,10) };
    public SmartDomeWindow(DomeInput d)
    {
        input=d;Title="AE Smart Dome Generator PRO";Width=830;Height=650;MinWidth=780;MinHeight=600;WindowStartupLocation=WindowStartupLocation.CenterOwner;
        var root=new DockPanel { Margin=new Thickness(18) };Content=root;
        var buttons=new StackPanel { Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right };
        DockPanel.SetDock(buttons,Dock.Bottom);root.Children.Add(buttons);
        var ok=new Button { Content="Generate / Update",Padding=new Thickness(16,8,16,8),IsDefault=true,Margin=new Thickness(8) };
        var cancel=new Button { Content="Cancel",Padding=new Thickness(16,8,16,8),IsCancel=true,Margin=new Thickness(8) };
        buttons.Children.Add(ok);buttons.Children.Add(cancel);
        var fields=new StackPanel { Width=250,Margin=new Thickness(0,0,18,0) };DockPanel.SetDock(fields,Dock.Left);root.Children.Add(fields);
        types.ItemsSource=Enum.GetValues(typeof(ProDomeType));types.SelectedItem=d.Settings.Type;Add(fields,"Dome type",types);
        Add(fields,"Diameter (m)",diameter,d.Settings.Diameter);Add(fields,"Height (m)",height,d.Settings.Height);
        Add(fields,"Segments",segments,d.Settings.Segments);Add(fields,"Rings / subdivision",rings,d.Settings.Rings);
        Add(fields,"Static oculus (%)",opening,d.Settings.Opening*100);Add(fields,"Layer depth (m)",depth,d.Settings.LayerDepth);
        Add(fields,"Freeform Y aspect",aspect,d.Settings.Aspect);Add(fields,"Square member (mm)",section,d.SectionMm);
        var right=new StackPanel();root.Children.Add(right);right.Children.Add(new TextBlock{Text="Concept geometry • isometric preview",FontSize=16,Margin=new Thickness(0,0,0,12)});right.Children.Add(preview);right.Children.Add(status);
        right.Children.Add(new TextBlock { Text="Iris Concept is a static spiral approximation, not a working linkage. Geodesic and Kiewitt require a closed crown.",TextWrapping=TextWrapping.Wrap });
        types.SelectionChanged+=(o,e)=>Refresh();foreach(var t in new[]{diameter,height,segments,rings,opening,depth,aspect,section})t.TextChanged+=(o,e)=>Refresh();
        ok.Click+=(o,e)=>{if(Refresh())DialogResult=true;};Refresh();
    }
    private static void Add(StackPanel panel,string label,Control c,double? value=null)
    {
        panel.Children.Add(new TextBlock { Text=label,Margin=new Thickness(0,5,0,2) });panel.Children.Add(c);
        if(value.HasValue&&c is TextBox t)t.Text=value.Value.ToString(CultureInfo.CurrentCulture);
    }
    private static double Read(TextBox t)
    {
        if(double.TryParse(t.Text,NumberStyles.Float,CultureInfo.CurrentCulture,out double d)||double.TryParse(t.Text,NumberStyles.Float,CultureInfo.InvariantCulture,out d))return d;
        throw new FormatException("Enter valid numeric values.");
    }
    private static int Int(TextBox t){var v=Read(t);if(v!=Math.Truncate(v)||v<0||v>1000)throw new FormatException("Use whole numbers for segments and rings.");return (int)v;}
    private bool Refresh()
    {
        try
        {
            var s=input.Settings;s.Type=(ProDomeType)(types.SelectedItem??ProDomeType.Ribbed);s.Diameter=Read(diameter);s.Height=Read(height);s.Segments=Int(segments);s.Rings=Int(rings);s.Opening=Read(opening)/100;s.LayerDepth=Read(depth);s.Aspect=Read(aspect);input.SectionMm=Read(section);
            if(!(input.SectionMm>=20&&input.SectionMm<=1000))throw new FormatException("Member section must be 20–1000 mm.");
            var m=ProDomeGenerator.Generate(s);if(m.Members.Count>6000)throw new InvalidOperationException("Reduce resolution to 6000 members or fewer.");
            preview.Children.Clear();
            double scale=250/Math.Max(s.Diameter*Math.Max(1,s.Aspect),s.Height*1.5);
            Point P(DomeNode n)=>new Point(240+(n.X-n.Y)*scale,250+(n.X+n.Y)*scale*0.32-n.Z*scale);
            foreach(var edge in m.Members){var a=P(m.Nodes[edge.A]);var b=P(m.Nodes[edge.B]);preview.Children.Add(new Line{X1=a.X,Y1=a.Y,X2=b.X,Y2=b.Y,Stroke=Brushes.SteelBlue,StrokeThickness=0.9});}
            status.Text=string.Format(CultureInfo.CurrentCulture,"{0} nodes | {1} members | {2:N2} m total centreline",m.Nodes.Count,m.Members.Count,m.TotalLengthMetres);return true;
        }
        catch(Exception e){preview.Children.Clear();status.Text=e.Message;return false;}
    }
}
