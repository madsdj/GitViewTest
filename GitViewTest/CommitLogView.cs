using System.Windows;
using System.Windows.Controls;

namespace GitViewTest
{
    public class CommitLogView : ListBox
    {
        static CommitLogView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommitLogView), new FrameworkPropertyMetadata(typeof(CommitLogView)));
        }

        public static readonly DependencyProperty NodeContainerStyleProperty = DependencyProperty.Register(
            nameof(NodeContainerStyle),
            typeof(Style),
            typeof(CommitLogView));

        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register(
            nameof(NodeTemplate),
            typeof(DataTemplate),
            typeof(CommitLogView));

        public Style NodeContainerStyle
        {
            get { return (Style)GetValue(NodeContainerStyleProperty); }
            set { SetValue(NodeContainerStyleProperty, value); }
        }

        public DataTemplate NodeTemplate
        {
            get { return (DataTemplate)GetValue(NodeTemplateProperty); }
            set { SetValue(NodeTemplateProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CommitLogItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CommitLogItem;
        }
    }
}
