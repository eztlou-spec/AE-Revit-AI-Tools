using System.Windows;
using System.Windows.Controls;

namespace AE.Revit.Entry.UI;
public sealed class DashboardWindow : Window
{
    public DashboardWindow()
    {
        Title="AE Revit AI Tools"; Width=760; Height=560; DataContext=new DashboardViewModel(); var tabs=new TabControl();
        foreach(var section in ((DashboardViewModel)DataContext).Sections) { var panel=new StackPanel { Margin=new Thickness(18) }; foreach(var tool in section.Tools) { var button=new Button { Content=$"{tool.Title} — {tool.Description}", Margin=new Thickness(0,0,0,8), IsEnabled=tool.IsAvailable, Command=tool.Execute }; panel.Children.Add(button); } tabs.Items.Add(new TabItem { Header=section.Name, Content=new ScrollViewer { Content=panel } }); }
        Content=tabs;
    }
}
