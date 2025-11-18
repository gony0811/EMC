using System.Windows;
using System.Windows.Controls;

namespace EMC
{
    public class DeviceArgsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MotionTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MotionArgs) return MotionTemplate;

            return null;
        }
    }
}
