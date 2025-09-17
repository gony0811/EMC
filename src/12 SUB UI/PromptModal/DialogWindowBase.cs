﻿using System.Reflection;
using System.Windows;
using Application = System.Windows.Application;

namespace EGGPLANT
{

    public abstract class DialogWindowBase : Window
    {
        private object? _vm;
        private EventInfo? _ev;
        private Delegate? _handler;

        protected DialogWindowBase()
        {
            ShowInTaskbar = false;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SizeToContent = SizeToContent.WidthAndHeight;

            DataContextChanged += (_, e) => { Detach(); Attach(e.NewValue); };
            Loaded += (_, __) =>
            {
                if (Owner == null)
                    Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                            ?? Application.Current?.MainWindow;
            };
            Closed += (_, __) => Detach();
        }

        private void Attach(object? vm)
        {
            if (vm == null) return;
            var iClose = vm.GetType().GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDialogRequestClose<>));
            if (iClose == null) return;

            var ev = iClose.GetEvent(nameof(IDialogRequestClose<object?>.CloseRequested));
            if (ev == null) return;

            var handlerType = ev.EventHandlerType!;
            var evtArgs = handlerType.GetGenericArguments().FirstOrDefault()
                ?? throw new InvalidOperationException("CloseRequested handler type mismatch.");
            var tRes = evtArgs.GetGenericArguments().FirstOrDefault()
                ?? throw new InvalidOperationException("DialogCloseRequestedEventArgs<T> generic not found.");

            var methodDef = typeof(DialogWindowBase).GetMethod(nameof(OnCloseRequestedGeneric),
                BindingFlags.Instance | BindingFlags.NonPublic)!;
            var method = methodDef.MakeGenericMethod(tRes);
            var del = Delegate.CreateDelegate(handlerType, this, method, true);

            ev.AddEventHandler(vm, del);

            _vm = vm; _ev = ev; _handler = del;
        }

        private void Detach()
        {
            if (_vm != null && _ev != null && _handler != null)
            {
                try { _ev.RemoveEventHandler(_vm, _handler); } catch { /* no-op */ }
            }
            _vm = null; _ev = null; _handler = null;
        }

        private void OnCloseRequestedGeneric<T>(object? s, DialogCloseRequestedEventArgs<T> e)
        {
            DialogResult = e.Result.DialogResultValue;
            Close();
        }
    }
}
