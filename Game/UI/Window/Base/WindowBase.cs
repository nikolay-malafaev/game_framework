using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework.Logging;
using GameFramework.StaticData;
using R3;
using VContainer;
using VContainer.Unity;

namespace GameFramework.UI.Window
{
    public enum WindowState
    {
        None,       // окна нет физически на сцене
        Showing,    // проигрывается анимация показа
        Shown,      // окно видно
        Hiding,     // проигрывается анимация скрытия
        Hidden      // окно физически есть, но скрыто
    }
    
    [Loggable]
    public abstract partial class WindowBase<TView> : IWindow, IAsyncStartable, IDisposable where TView : WindowViewBehaviour
    {
        protected TView _view;
        protected IStaticDataService _staticDataService;
        private WindowViewFactory _windowFactory;
        private LifetimeScope _windowScope;
        private LifetimeScope _parentScope;
        private CompositeDisposable _disposable = new ();
        private bool _initialized = false;

        public abstract string Id { get; }
        public WindowState State { get; private set; } = WindowState.None;
        private IObjectResolver ObjectResolver => _windowScope.Container;

        public WindowBase(LifetimeScope lifetimeScope)
        {
            _parentScope = lifetimeScope;
            _staticDataService = lifetimeScope.Container.Resolve<IStaticDataService>();
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            CreateWindowScope();
            ResolveDependencies();
            await InitializeWindowViewFactory();
            _initialized = true;
            OnStart();
        }

        public void Dispose()
        {
            OnDispose();
            if (_disposable.IsDisposed)
            {
                return;
            }
            _disposable.Dispose();
        }

        public void Open() => Open(null);
        
        public void Open(Action onOpen, params IWindowParameter[] parameters)
        {
            if (!IsWindowInitialized())
            {
                LogError("Window not initialized!");
                return;
            }
            
            if (!_view)
            {
                CreateWindowView();
                SubscribeToViewActions();
            }
            
            OnOpen(parameters);
            LogInfo("Window opened.");
            Show(onOpen, parameters);
        }
        
        public void Close() => Close(null);

        public void Close(Action onClose,  params IWindowParameter[] parameters)
        {
            void OnCloseCallback()
            {
                UnsubscribeFromViewActions();
                DestroyWindowView();
                SetState(WindowState.None);
                LogInfo("Window closed.");
                onClose?.Invoke();
            }
            
            if (!IsWindowValid())
            {
                LogError("Window not valid!");
                return;
            }
            
            OnClose(parameters);
            Hide(OnCloseCallback, parameters);
        }
        
        public void Show() => Show(null);
        
        public void Show(Action onShow, params IWindowParameter[] parameters)
        { 
            void OnShowCallback()
            {
                AfterShow(parameters);
                SetState(WindowState.Shown);
                LogInfo("Window showed.");
                onShow?.Invoke();
            }
            
            if (!IsWindowValid())
            {
                LogError("Window not valid!");
                return;
            }
            
            if (State == WindowState.Showing || State == WindowState.Shown)
            {
                LogWarning("Can't show. Window is already shown.");
                return;
            }
            
            BeforeShow(parameters);
            SetState(WindowState.Showing);
            _view.Show(OnShowCallback, parameters);
        }
        
        public void Hide() => Hide(null);
        
        public void Hide(Action onHide, params IWindowParameter[] parameters)
        {
            void OnHideCallback()
            {
                AfterHide(parameters);
                SetState(WindowState.Hidden);
                LogInfo("Window hided.");
                onHide?.Invoke();
            }

            if (!IsWindowValid())
            {
                LogError("Window not valid!");
                return;
            }

            if (State != WindowState.Shown)
            {
                LogWarning("Can't hide. Window not shown.");
                return;
            }
            
            BeforeHide(parameters);
            SetState(WindowState.Hiding);
            _view.Hide(OnHideCallback, parameters);
        }
        
        protected virtual void OnStart() {}
        protected virtual void OnDispose() {}

        protected virtual void OnOpen(params IWindowParameter[] parameters) { }
        protected virtual void OnClose(params IWindowParameter[] parameters) { }
        
        protected virtual void BeforeShow(params IWindowParameter[] parameters) { }
        protected virtual void AfterShow(params IWindowParameter[] parameters) { }
        protected virtual void BeforeHide(params IWindowParameter[] parameters) { }
        protected virtual void AfterHide(params IWindowParameter[] parameters) { }

        protected virtual void SubscribeToViewActions()
        {
            _view.HideWindowClicked += Hide;
            _view.CloseWindowClicked += Close;
        }

        protected virtual void UnsubscribeFromViewActions()
        {
            _view.HideWindowClicked -= Hide;
            _view.CloseWindowClicked -= Close;
        }

        private void SetState(WindowState state)
        {
            State = state;
        }

        private void CreateWindowScope()
        {
            _windowScope = _parentScope.CreateChild(builder =>
            {
                var commonWindowSettings = _staticDataService.Get<CommonWindowSettings>();
                if (commonWindowSettings.HasValue())
                {
                    builder.RegisterInstance(commonWindowSettings.Value());
                }
                var windowSettings = _staticDataService.Get<WindowSettings>(Id);
                if (windowSettings.HasValue())
                {
                    builder.RegisterInstance(windowSettings.Value());
                }
                builder.Register<WindowViewFactory>(Lifetime.Scoped).WithParameter(Id);
            });
            _disposable.Add(_windowScope);
        }

        private void ResolveDependencies()
        {
            _windowFactory = ObjectResolver.Resolve<WindowViewFactory>();
        }

        private async UniTask InitializeWindowViewFactory()
        {
            await _windowFactory.Initialize();
        }

        private bool IsWindowInitialized()
        {
            return _initialized && !_disposable.IsDisposed;
        }

        private bool IsWindowValid()
        {
            return IsWindowInitialized() && _view != null;
        }

        private void CreateWindowView()
        {
            _view = _windowFactory.Create<TView>();
        }

        private void DestroyWindowView()
        {
            UnityEngine.Object.Destroy(_view.gameObject);
        }
    }
}