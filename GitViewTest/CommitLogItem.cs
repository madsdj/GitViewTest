using System.Windows;
using System.Windows.Controls;

namespace GitViewTest
{
    public class CommitLogItem : ListBoxItem
    {
        static CommitLogItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommitLogItem), new FrameworkPropertyMetadata(typeof(CommitLogItem)));
        }
    }
}
