namespace Elmish.Avalonia

open Elmish
open Avalonia.Threading
open Avalonia.Controls
open System.ComponentModel
open System.Reactive.Subjects
open System.Reactive.Linq
open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

[<AbstractClass>]
type ReactiveElmishViewModel<'Model, 'Msg>(initialModel: 'Model) = 
    inherit ReactiveUI.ReactiveObject()

    let propertyChanged = Event<_, _>()
    let mutable _model: 'Model = initialModel
    let _modelSubject = new Subject<'Model>()

    /// Tracks subscriptions of boxed 'Model projections by the VM property name.
    let propertySubscriptions = Dictionary<string, IDisposable * ('Model -> obj)>()

    member this.Model = _model

    member this.ModelObservable = _modelSubject.AsObservable()

    /// Dispatches a message to the Elmish loop. NOTE: will throw an exception if called before the Elmish loop has started.
    member val Dispatch : 'Msg -> unit = 
        fun _ -> 
            if not Design.IsDesignMode 
            then failwith "`Dispatch` failed because the Elmish loop has not been started."
        with get, set
    
    /// Starts the Elmish loop for this view model.
    abstract member StartElmishLoop : Control -> unit
    interface IElmishViewModel with
        member this.StartElmishLoop(view: Control) = this.StartElmishLoop(view)

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = propertyChanged.Publish

    /// Fires the `PropertyChanged` event for the given property name. Uses the caller's name if no property name is given.
    member this.OnPropertyChanged([<CallerMemberName; Optional; DefaultParameterValue("")>] ?propertyName: string) =
        propertyChanged.Trigger(this, PropertyChangedEventArgs(propertyName.Value))

    /// Binds a VM property to a 'Model projection and refreshes the VM property when the 'Model projection changes.
    member this.BindModel(vmPropertyName: string, modelProjection: 'Model -> 'ModelProjection) = 
        match propertySubscriptions.TryGetValue(vmPropertyName) with
        | false, _ -> 
            // Creates a subscription to the 'Model projection and stores it in a dictionary.
            let disposable = 
                _modelSubject
                    .DistinctUntilChanged(modelProjection)
                    .Subscribe(fun _ -> 
                        // Alerts the view that the 'Model projection / VM property has changed.
                        this.OnPropertyChanged(vmPropertyName)
                        #if DEBUG
                        printfn $"PropertyChanged: {vmPropertyName}"
                        #endif
                    )

            // Stores the subscription and the boxed model projection ('Model -> obj) in a dictionary
            let boxedModelProjection = modelProjection >> unbox<obj>
            propertySubscriptions.Add(vmPropertyName, (disposable, boxedModelProjection))

            // Returns the latest value from the model projection.
            _model |> modelProjection 

        | true, (_, boxedModelProjection) -> 
            // Returns the latest value from the model projection.
            _model |> boxedModelProjection :?> 'ModelProjection

    /// Binds this VM to the view `DataContext` and runs the Elmish loop.
    member internal this.RunProgram (program: Elmish.Program<unit, 'Model, 'Msg, unit>, view: Control, syncDispatch) =
        
        // Updates when the Elmish model changes and sends out an Rx stream.
        let setState model (_: Dispatch<'Msg>) =
            _model <- model
            _modelSubject.OnNext(model)

        // A fn that dispatches messages to the Elmish loop.
        let withDispatch (innerDispatch: Dispatch<'Msg>) : Dispatch<'Msg> =
            let dispatch = 
                if syncDispatch 
                then fun msg -> Dispatcher.UIThread.Post(fun () -> innerDispatch msg)
                else innerDispatch
            this.Dispatch <- dispatch
            dispatch

        // Wires up the view and the VM.
        view.DataContext <- this

        if this.DisposeOnUnload then
            // Disposes the VM when the view is unloaded, and optionally dispatches a termination message.
            view.Unloaded.AddHandler(fun _ _ -> 
                this.TerminateMsg |> Option.iter this.Dispatch
                (this :> IDisposable).Dispose())
        
        program
        |> Program.withSetState setState
        |> Program.runWithDispatch withDispatch ()

    /// Determines whether this VM should be disposed when the view is unloaded. Default is true.
    member val DisposeOnUnload = true with get, set

    member val internal TerminateMsg: 'Msg option = None with get, set

    interface IDisposable with
        member this.Dispose() =
            propertySubscriptions.Values |> Seq.iter (fun (disposable, _) -> disposable.Dispose())
            propertySubscriptions.Clear()
            _modelSubject.Dispose()



module Program =
    /// Creates an Avalonia program via Program.mkProgram.
    let mkAvaloniaProgram (init: unit -> 'Model * Cmd<'Msg>) update = 
        Program.mkProgram init update (fun _ _ -> ())

    /// Creates an Avalonia program via Program.mkSimple.
    let mkAvaloniaSimple (init: unit -> 'Model) update =
        Program.mkSimple init update (fun _ _ -> ())

    /// Binds the vm to the view and then runs the Elmish program.    
    let runView (vm: ReactiveElmishViewModel<'Model, 'Msg>) (view: Control) program = 
        if not Design.IsDesignMode 
        then vm.RunProgram(program, view, syncDispatch = false)

    /// Binds the vm to the view and then runs the Elmish program.
    /// Ensures that all changes dispatched from non-UI threads are synchronized via the Avalonia Dispatcher.
    let runViewWithSyncDispatch (vm: ReactiveElmishViewModel<'Model, 'Msg>) (view: Control) program = 
        if not Design.IsDesignMode 
        then vm.RunProgram(program, view, syncDispatch = true)

    /// Configures `Program.withTermination` using the given terminate 'Msg, and dispatches the 'Msg when the view is `Unloaded`.
    let terminateOnViewUnloaded (vm: ReactiveElmishViewModel<'Model, 'Msg>) (terminateMsg: 'Msg) program = 
        vm.TerminateMsg <- Some terminateMsg
        program 
        |> Program.withTermination (fun m -> m = terminateMsg) (fun _ -> printfn $"terminateOnViewUnloaded: dispatching {terminateMsg}")

