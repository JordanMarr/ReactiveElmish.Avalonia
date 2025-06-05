namespace AvaloniaExample

open Microsoft.Extensions.DependencyInjection
open AvaloniaExample.ViewModels
open AvaloniaExample.Views
open ReactiveElmish.Avalonia
open ReactiveElmish

type MainViewFacade(root: CompositionRoot) =
    interface IMainViewFacade with
        member this.GetView<'VM & #ReactiveElmishViewModel>(): Avalonia.Controls.Control = root.GetView<'VM>()

type AppCompositionRoot private () =
    inherit CompositionRoot()

    let mainView = MainView()

    override this.RegisterServices services = 
        base.RegisterServices(services)
            .AddSingleton<FileService>(FileService(mainView))
            .AddTransient<IMainViewFacade, MainViewFacade>()

    override this.RegisterViews() = 
        Map [
            VM.Key<MainViewModel>(), View.Singleton(mainView)               // Singleton view will maintain state between navigations
            VM.Key<TodoListViewModel>(), View.Transient<TodoListView>()     // Transient view will be disposed and recreated each time it is navigated to
            VM.Key<CounterViewModel>(), View.Transient<CounterView>()
            VM.Key<AboutViewModel>(), View.Singleton<AboutView>()
            VM.Key<ChartViewModel>(), View.Singleton<ChartView>()
            VM.Key<FilePickerViewModel>(), View.Singleton<FilePickerView>()
        ]

    static member val Instance = AppCompositionRoot()
        
        
