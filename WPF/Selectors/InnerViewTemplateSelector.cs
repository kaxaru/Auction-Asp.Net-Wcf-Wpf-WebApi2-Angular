using System;
using System.Windows;
using System.Windows.Controls;
using WPF.ViewModels;

namespace WPF.Selectors
{
    public class InnerViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TryLoginDataTemplate { get; set; }

        public DataTemplate ProductsDataTemplate { get; set; }

        public DataTemplate WrongEnterDataTemlate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch ((CurrentViewEnum)Enum.Parse(typeof(CurrentViewEnum), item.ToString()))
            {
                case CurrentViewEnum.TryLoginView:
                    return TryLoginDataTemplate;
                case CurrentViewEnum.ProductsView:
                    return ProductsDataTemplate;
                default:
                    return TryLoginDataTemplate;
            }
        }
    }
}
