using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ProcessCounter
{
  internal class MainWindowViewModel : INotifyPropertyChanged
  {
    private DispatcherTimer timer;
    private string searchText;
    private bool _IsRunning = true;
    private List<XProcess> _Processes;

    public MainWindowViewModel()
    {
      timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromMilliseconds(500);
      timer.Tick += timer_Tick;
      timer.Start();
    }

    public List<XProcess> Processes { get; set; } = [];
    public string TextResult { get; set; }

    public bool ExactMatch { get; set; }
    public bool IsRunning
    {
      get => _IsRunning;
      set
      {
        _IsRunning = value;
        __OnPropertyChanged(nameof(PathData));
        __OnPropertyChanged(nameof(PathColor));
      }
    }

    public Geometry PathData => Geometry.Parse(IsRunning ? "M 0,0 H 30 V 100 H 0 Z M 50,0 H 80 V 100 H 50 Z" : "M 0,0 L 0,100 L 87,50 Z");
    public Brush PathColor => IsRunning ? Brushes.Red : Brushes.Green;

    public event PropertyChangedEventHandler? PropertyChanged;

    internal void TextChanged(string text)
    {
      searchText = text;
      if (!_IsRunning && _Processes != null)
      {
        if (string.IsNullOrWhiteSpace(searchText))
          Processes = _Processes;
        else
          Processes = new(ExactMatch ?
            Processes.Where(p => string.Equals(p.Name.Value, searchText, StringComparison.InvariantCultureIgnoreCase))
            : Processes.Where(p => p.Name.Value.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)));
        __RefreshUI();
      }
    }

    private void timer_Tick(object? sender, EventArgs e)
    {
      if (!_IsRunning)
        return;
      Process[] tempProcessArray;
      bool hasSearchtext = !string.IsNullOrWhiteSpace(searchText);
      if (ExactMatch && hasSearchtext)
      {
        tempProcessArray = Process.GetProcessesByName(searchText);
      }
      else if (hasSearchtext)
      {
        tempProcessArray = Process.GetProcesses()
          .Where(p => p.ProcessName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
          .ToArray();
      }
      else
        tempProcessArray = Process.GetProcesses();
      Processes = [.. tempProcessArray.Select(p => new XProcess(p)).OrderBy(op => op.Name.Value)];
      _Processes = new(Processes);
      __RefreshUI();
    }

    private void __RefreshUI()
    {
      TextResult = string.IsNullOrEmpty(searchText) ? $"{Processes.Count} Processes" : $"Found {Processes.Count} Processes containing \"{searchText}\"";
      __OnPropertyChanged(nameof(Processes));
      __OnPropertyChanged(nameof(TextResult));
    }

    private void __OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
      this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    internal void KillProcess(XProcess xProc)
    {
      try
      {
        xProc.Kill();
        if (xProc.ProcessId.Value != 0)
        {
          var toRemove = Processes.FirstOrDefault(p => p.ProcessId.Value == xProc.ProcessId.Value);
          if (toRemove != null)
            Processes.Remove(toRemove);
          __RefreshUI();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, $"Killing process {xProc.Name.Value} ({xProc.ProcessId.Value}) failed");
      }
    }
  }
}
