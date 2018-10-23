using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GitViewTest
{
    public class CommitLogView : ItemsControl
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
    }
}
