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
    let serviceCollection = lazy this.RegisterServices()
    let mutable viewRegistry: Map<VMKey, ViewRegistration> = Map.empty

    interface ICompositionRoot with
        member this.ServiceProvider = this.ServiceProvider
        //member this.GetView<'ViewModel & #IReactiveObject>(?vm: 'ViewModel) = 
        //    let vm = defaultArg vm (this.ServiceProvider.GetRequiredService<'ViewModel>()) 
        //    let r = this.GetView<'ViewModel & #IReactiveObject>(vm)
        //    r

    /// Gets the composition root service provider.
    member this.ServiceProvider = 
        serviceCollection.Value.BuildServiceProvider()

    /// Allows you to register services to be consumed by your application and VMs.
    abstract member RegisterServices : unit -> IServiceCollection
    default this.RegisterServices() = ServiceCollection()

    /// Allows you to register views by VM type name.
    abstract member RegisterViews : unit -> Map<VMKey, ViewRegistration>
    default this.RegisterViews() = 
        // TODO: Default should scan the assembly for all VMs and views
        Map.empty

    member this.GetMainWindow<'MainViewModel & #ReactiveElmishViewModel>() = 
        viewRegistry <- this.RegisterViews()
        let window = this.GetView<'MainViewModel>() |> unbox<Window>
        let vm = window.DataContext :?> ReactiveElmishViewModel // |> unbox<ReactiveElmishViewModel>
        vm.Root <- this
        window
    
    /// Gets or creates a view from the view registry by its VM type.
    member this.GetView<'ViewModel & #IReactiveObject>(?vm: 'ViewModel) = 
        let vmType = typeof<'ViewModel>
        let vmKey = VMKey.Create vmType

        let vm = defaultArg vm (this.ServiceProvider.GetRequiredService<'ViewModel>()) 

        // Returns a view/VM instance from the registry or creates a new one.
        match viewRegistry |> Map.tryFind vmKey with
        | Some registration -> 
            match registration with
            | Singleton view -> 
                ViewBinder.bindSingleton (vm, view) |> snd
            | Transient viewType -> 
                let view = Activator.CreateInstance(viewType) :?> Control
                ViewBinder.bindWithDispose (vm, view) |> snd
        | None ->
            failwithf $"No view registered for VM type {vmType.FullName}"

    
