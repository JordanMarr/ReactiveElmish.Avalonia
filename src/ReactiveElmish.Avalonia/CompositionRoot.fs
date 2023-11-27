namespace ReactiveElmish.Avalonia

open System
open Microsoft.Extensions.DependencyInjection
open Avalonia.Controls
open ReactiveUI
open ReactiveElmish

type VM = 
    private VMKey of string
        static member Key(vmType: Type) = VMKey vmType.FullName
        static member Key<'ViewModel & #IReactiveObject>() = VMKey typeof<'ViewModel>.FullName

type View = 
    | SingletonView of view: Control
    | TransientView of view: (unit -> Control)
        /// Creates a new view instance on each request using the view's parameterless constructor.
        static member Transient<'View when 'View :> Control and 'View : (new : unit -> 'View)>() = 
            TransientView (fun () -> new 'View() :> Control)
        /// Creates a new view instance on each request using the given `createView` function.
        static member Transient(createView: unit -> Control) = 
            TransientView createView
        /// Returns the given `view` instance on each request.
        static member Singleton<'View & #Control>(view: 'View) = 
            SingletonView view
        /// Creates a new view instance on the first request using the given `createView` function and returns it on each subsequent request.
        static member Singleton(createView: unit -> Control) = 
            SingletonView (createView())
        /// Creates a new view instance on the first request using the view's parameterless constructor and returns it on each subsequent request.
        static member Singleton<'View when 'View :> Control and 'View : (new : unit -> 'View)>() = 
            SingletonView (new 'View())

type CompositionRoot() as this = 
    
    let serviceProvider = lazy this.RegisterServices(ServiceCollection()).BuildServiceProvider()
    let viewRegistry = lazy this.RegisterViews()

    /// Gets the composition root service provider.
    member this.ServiceProvider : IServiceProvider = serviceProvider.Value

    /// Registers services to be consumed by the application and VMs. 
    /// Base implementation scans the CompositionRoot subtype assembly for IReactiveObject VM types and adds them as transient services.
    abstract member RegisterServices: IServiceCollection -> IServiceCollection
    default this.RegisterServices(services: IServiceCollection) = 
        // Add the composition root instance as a singleton service.
        services.AddSingleton(this) |> ignore
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
    member this.GetView<'ViewModel & #ReactiveElmishViewModel>() = 
        this.GetView(typeof<'ViewModel>)

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
            | TransientView createView -> 
                let vm = this.ServiceProvider.GetRequiredService(vmType) :?> IReactiveObject
                ViewBinder.bindWithDisposeOnViewUnload (vm, createView()) |> snd
        | None ->
            failwithf $"No view registered for VM type {vmType.FullName}"

    
