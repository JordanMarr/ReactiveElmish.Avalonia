namespace AvaloniaExample

open Microsoft.Extensions.DependencyInjection
open AvaloniaExample.ViewModels
open AvaloniaExample.Views
open ReactiveElmish.Avalonia

type AppCompositionRoot() =
    inherit CompositionRoot()

    let mainView = MainView()

    override this.RegisterServices services = 
        base.RegisterServices(services)
            .AddSingleton<FileService>(FileService(mainView))

    override this.RegisterViews() = 
        Map [
            VM.Key<MainViewModel>(), View.Singleton(mainView)
            VM.Key<TodoListViewModel>(), View.Transient<TodoListView>()
            VM.Key<CounterViewModel>(), View.Singleton<CounterView>()
            VM.Key<AboutViewModel>(), View.Singleton<AboutView>()
            VM.Key<ChartViewModel>(), View.Singleton<ChartView>()
            VM.Key<FilePickerViewModel>(), View.Singleton<FilePickerView>()
        ]

    static member private instanceLazy = lazy AppCompositionRoot()
    static member Instance = AppCompositionRoot.instanceLazy.Value
        
        
