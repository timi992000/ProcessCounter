using System.Windows;
using System.Windows.Controls;

namespace ProcessCounter
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainWindowViewModel();
    }

    private void __TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      if (DataContext is MainWindowViewModel vm && sender is TextBox tb)
        vm.TextChanged(tb.Text);
    }

    private void __Kill(object sender, RoutedEventArgs e)
    {
      if (DataContext is MainWindowViewModel vm && sender is FrameworkElement elem && elem.DataContext is XProcess xProc)
      {
        vm.KillProcess(xProc);
      }
    }

    private void __ToggleAction(object sender, RoutedEventArgs e)
    {
      if (DataContext is MainWindowViewModel vm)
      {
        vm.IsRunning = !vm.IsRunning;
      }
    }
  }
}