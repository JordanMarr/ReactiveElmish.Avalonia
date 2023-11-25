namespace Elmish.Avalonia

open Elmish
open Avalonia.Controls
open System
open Microsoft.Extensions.DependencyInjection
open ReactiveUI

type VM = 
    private VMKey of string
        static member Key(vmType: Type) = VMKey vmType.FullName
        static member Key<'ViewModel & #IReactiveObject>() = VMKey typeof<'ViewModel>.FullName

type View = 
    | SingletonView of view: Control
    | TransientView of viewType: Type
        static member Transient<'View & #Control>() = TransientView typeof<'View>
        static member Singleton<'View & #Control>(view: 'View) = SingletonView view
        static member Singleton<'View & #Control>() = Activator.CreateInstance(typeof<'View>) :?> Control |> SingletonView

type CompositionRoot() as this = 
    
    do ICompositionRoot.instance <- this 
    let serviceProvider = lazy this.RegisterServices(ServiceCollection()).BuildServiceProvider()
    let viewRegistry = lazy this.RegisterViews()

    interface ICompositionRoot with
        member this.ServiceProvider = this.ServiceProvider
        member this.GetView(vmType: Type) = this.GetView(vmType)

    /// Gets the composition root service provider.
    member this.ServiceProvider : IServiceProvider = serviceProvider.Value

    /// Registers services to be consumed by the application and VMs. 
    /// Base implementation scans the CompositionRoot subtype assembly for IReactiveObject VM types and adds them as transient services.
    abstract member RegisterServices: IServiceCollection -> IServiceCollection
    default this.RegisterServices(services: IServiceCollection) = 
        // Scan for IReactiveObject VM types and add them as transient services.
        let vmType = typeof<IReactiveObject>
        this.GetType().Assembly.GetTypes() // Get types in the CompositionRoot subtype assembly.
        |> Array.filter (fun t -> t.IsClass && not t.IsAbstract && t.GetInterfaces() |> Array.contains(vmType))
        |> Array.iter (services.AddTransient >> ignore)
        services
        
    
    /// Allows you to register views by VM type name.
    abstract member RegisterViews : unit -> Map<VM, View>
    default this.RegisterViews() = Map.empty

    /// Gets or creates a view from the view registry by its VM type.
    member this.GetView<'ViewModel & #ReactiveElmishViewModel>() = this.GetView(typeof<'ViewModel>)

    /// Gets or creates a view from the view registry by its VM type.
    member this.GetView(vmType: Type) = 
        let vmKey = VM.Key vmType

        // Returns a view/VM instance from the registry or creates a new one.
        match viewRegistry.Value |> Map.tryFind vmKey with
        | Some registration -> 
            match registration with
            | SingletonView view -> 
                if view.DataContext = null then 
                    let vm = this.ServiceProvider.GetRequiredService(vmType) :?> IReactiveObject
                    ViewBinder.bindSingleton (vm, view) |> snd
                else
                    view
            | TransientView viewType -> 
                let vm = this.ServiceProvider.GetRequiredService(vmType) :?> IReactiveObject
                let view = Activator.CreateInstance(viewType) :?> Control
                ViewBinder.bindWithDispose (vm, view) |> snd
        | None ->
            failwithf $"No view registered for VM type {vmType.FullName}"

    
