namespace Elmish.Avalonia

open Elmish
open Avalonia.Controls
open System
open Microsoft.Extensions.DependencyInjection
open ReactiveUI

type VMKey = 
    private VMKey of string
    with 
        static member Create(vmType: Type) = 
            VMKey vmType.FullName
        static member Create<'ViewModel & #IReactiveObject>() = 
            let vmType = typeof<'ViewModel>
            VMKey vmType.FullName


type ViewRegistration = 
    | Singleton of view: Control
    | Transient of viewType: Type
    with 
        static member TransientView<'View & #Control>() = Transient typeof<'View>
        static member SingletonView<'View & #Control>(view: 'View) = Singleton view
        static member SingletonView<'View & #Control>() = Activator.CreateInstance(typeof<'View>) :?> Control |> Singleton

type CompositionRoot() as this = 
    let serviceCollection: Lazy<IServiceCollection> = lazy this.InitServices()
    let mutable viewRegistry: Map<VMKey, ViewRegistration> = Map.empty

    interface ICompositionRoot with
        member this.ServiceProvider = this.ServiceProvider
        member this.GetView(vmType: Type) = this.GetView(vmType)
        
    /// Gets the composition root service provider.
    member this.ServiceProvider = serviceCollection.Value.BuildServiceProvider() : IServiceProvider

    /// Registers services to be consumed by the application and VMs.
    abstract member RegisterServices: IServiceCollection -> IServiceCollection
    default this.RegisterServices(services: IServiceCollection) = services
    
    member private this.InitServices() = 
        let services = ServiceCollection()
        // Find all IReactiveObject types in the assembly and add them as transient services.
        let vmType = typeof<IReactiveObject>
        Reflection.Assembly.GetEntryAssembly().GetTypes()
        |> Array.filter (fun t -> t.IsClass && not t.IsAbstract && t.GetInterfaces() |> Array.contains(vmType))
        |> Array.iter (services.AddTransient >> ignore)
        this.RegisterServices(services)

    /// Allows you to register views by VM type name.
    abstract member RegisterViews : unit -> Map<VMKey, ViewRegistration>
    default this.RegisterViews() = 
        // TODO: Default should scan the assembly for all VMs and views
        Map.empty

    member this.GetMainWindow<'MainViewModel & #ReactiveElmishViewModel, 'MainView & #Window>() = 
        viewRegistry <- this.RegisterViews()
        let vm = this.ServiceProvider.GetRequiredService<'MainViewModel>()
        vm.Root <- this
        let window = Activator.CreateInstance(typeof<'MainView>) :?> Window
        ViewBinder.bindSingleton (vm, window) |> ignore
        window

    /// Gets or creates a view from the view registry by its VM type.
    member this.GetView(vmType: Type) = 
        let vmKey = VMKey.Create vmType

        // Returns a view/VM instance from the registry or creates a new one.
        match viewRegistry |> Map.tryFind vmKey with
        | Some registration -> 
            match registration with
            | Singleton view -> 
                if view.DataContext = null then 
                    let vm = this.ServiceProvider.GetRequiredService(vmType) :?> IReactiveObject
                    ReactiveElmishViewModel.trySetRoot this vm
                    ViewBinder.bindSingleton (vm, view) |> snd
                else
                    view
            | Transient viewType -> 
                let vm = this.ServiceProvider.GetRequiredService(vmType) :?> IReactiveObject
                ReactiveElmishViewModel.trySetRoot this vm
                let view = Activator.CreateInstance(viewType) :?> Control
                ViewBinder.bindWithDispose (vm, view) |> snd
        | None ->
            failwithf $"No view registered for VM type {vmType.FullName}"

    
