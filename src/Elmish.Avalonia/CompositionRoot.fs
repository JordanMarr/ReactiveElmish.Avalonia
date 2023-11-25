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
    | TransientView of view: Control
        /// Creates a new view instance on each request using `Activator.CreateInstance`.
        static member Transient<'View & #Control>() = TransientView (Activator.CreateInstance(typeof<'View>) :?> Control)
        /// Creates a new view instance on each request using the given `createView` function.
        static member Transient<'View & #Control>(createView: unit -> 'View) = TransientView (createView())
        /// Returns the given `view` instance on each request.
        static member Singleton<'View & #Control>(view: 'View) = SingletonView view
        /// Creates a new view instance on the first request using the given `createView` function and returns it on each subsequent request.
        static member Singleton<'View & #Control>(createView: unit -> 'View) = SingletonView (createView())
        /// Creates a new view instance on the first request using `Activator.CreateInstance` and returns it on each subsequent request.
        static member Singleton<'View & #Control>() = SingletonView (Activator.CreateInstance(typeof<'View>) :?> Control)

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

    /// Gets or creates a view/VM pair from the view registry by its VM type.
    member this.GetView(vmType: Type) = 
        match viewRegistry.Value |> Map.tryFind (VM.Key vmType) with
        | Some registration -> 
            match registration with
            | SingletonView view -> 
                if view.DataContext = null then 
                    let vm = this.ServiceProvider.GetRequiredService(vmType) :?> IReactiveObject
                    ViewBinder.bind (vm, view) |> snd
                else
                    view
            | TransientView view -> 
                let vm = this.ServiceProvider.GetRequiredService(vmType) :?> IReactiveObject
                ViewBinder.bindWithDisposeOnViewUnload (vm, view) |> snd
        | None ->
            failwithf $"No view registered for VM type {vmType.FullName}"

    
