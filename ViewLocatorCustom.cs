using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using Test_DependencyInjection.ViewModels;
using Test_DependencyInjection.Views;


namespace Test_DependencyInjection
{

    public class ViewLocatorCustom : IDataTemplate
    {
        private readonly Dictionary<Type, Func<Control?>> _locator = new();

        public ViewLocatorCustom()
        {
            RegisterViewFactory<MainWindowViewModel, MainWindow>();
            RegisterViewFactory<TestDIViewModel, TestDI>();
        }

        public Control Build(object? data)
        {
            if (data is null)
            {
                return new TextBlock { Text = "No VM provided" };
            }

            _locator.TryGetValue(data.GetType(), out var factory);

            return factory?.Invoke() ?? new TextBlock { Text = $"VM Not Registered: {data.GetType()}" };
        }

        public bool Match(object? data)
        {
            return data is ObservableObject;
        }

        private void RegisterViewFactory<TViewModel, TView>()
            where TViewModel : class
            where TView : Control
            => _locator.Add(
                typeof(TViewModel),
                Design.IsDesignMode
                    ? Activator.CreateInstance<TView>
                    : Ioc.Default.GetService<TView>);
    }
}
