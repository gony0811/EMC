

using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace EGGPLANT
{
    public partial class SwitchButton : UserControl
    {
        public SwitchButton()
        {
            InitializeComponent();
            Loaded += (_, __) => UpdateIsEnabledFromCommand();
            Unloaded += (_, __) => DetachCanExecute();
        }

        // ===== IsOn DP =====
        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(SwitchButton),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // ===== Command/Parameter/Target DPs =====
        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(SwitchButton),
                new PropertyMetadata(null, OnCommandChanged));

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(SwitchButton),
                new PropertyMetadata(null, (_, __) => { /* no-op */ }));

        public IInputElement? CommandTarget
        {
            get => (IInputElement?)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(SwitchButton),
                new PropertyMetadata(null));

        // ===== Toggled 이벤트(옵션) =====
        public static readonly RoutedEvent ToggledEvent =
            EventManager.RegisterRoutedEvent("Toggled", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SwitchButton));
        public event RoutedEventHandler Toggled
        {
            add => AddHandler(ToggledEvent, value);
            remove => RemoveHandler(ToggledEvent, value);
        }

        // 클릭/키보드 처리
        private void OnClick(object sender, RoutedEventArgs e) => ToggleAndExecute();
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                ToggleAndExecute();
                e.Handled = true;
            }
        }

        private void ToggleAndExecute()
        {
            IsOn = !IsOn;
            RaiseEvent(new RoutedEventArgs(ToggledEvent, this));

            var cmd = Command;
            var param = CommandParameter ?? IsOn; // 파라미터 미지정 시 새 상태(bool) 전달
            if (cmd != null)
            {
                if (cmd is RoutedCommand rc)
                {
                    var target = CommandTarget ?? this;
                    if (rc.CanExecute(param, target)) rc.Execute(param, target);
                }
                else
                {
                    if (cmd.CanExecute(param)) cmd.Execute(param);
                }
            }
            UpdateIsEnabledFromCommand();
        }

        // CanExecute 연동 → IsEnabled 자동 반영
        private void UpdateIsEnabledFromCommand()
        {
            var cmd = Command;
            if (cmd == null)
            {
                IsEnabled = true;
                return;
            }

            bool can;
            var param = CommandParameter ?? IsOn;
            if (cmd is RoutedCommand rc)
            {
                can = rc.CanExecute(param, CommandTarget ?? this);
            }
            else
            {
                can = cmd.CanExecute(param);
            }
            IsEnabled = can;
            AttachCanExecute();
        }

        private void AttachCanExecute()
        {
            DetachCanExecute();
            if (Command != null) Command.CanExecuteChanged += Command_CanExecuteChanged;
        }
        private void DetachCanExecute()
        {
            if (Command != null) Command.CanExecuteChanged -= Command_CanExecuteChanged;
        }
        private void Command_CanExecuteChanged(object? sender, EventArgs e) => UpdateIsEnabledFromCommand();

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (SwitchButton)d;
            if (e.OldValue is ICommand oldCmd) oldCmd.CanExecuteChanged -= self.Command_CanExecuteChanged;
            if (e.NewValue is ICommand newCmd) newCmd.CanExecuteChanged += self.Command_CanExecuteChanged;
            self.UpdateIsEnabledFromCommand();
        }
    }
}