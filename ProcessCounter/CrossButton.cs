using System.Windows;
using System.Windows.Controls;

namespace ProcessCounter
{
  public class CrossButton : Button
  {
    static CrossButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CrossButton), new FrameworkPropertyMetadata(typeof(CrossButton)));
    }
  }
}
