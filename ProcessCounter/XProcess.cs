using System.Diagnostics;

namespace ProcessCounter
{
  internal class XProcess
  {
    public XProcess(Process process)
    {
      ProcessName = process.ProcessName;
      Id = process.Id;
    }

    private string ProcessName;
    private int Id;

    public Lazy<int> ProcessId => new(() => Id);
    public Lazy<string> Name => new(() => ProcessName);

    public override string ToString()
    {
      return $"{ProcessName} => {Id}";
    }

    internal void Kill()
    {
      Process.GetProcessById(Id)?.Kill(true);
    }
  }
}
