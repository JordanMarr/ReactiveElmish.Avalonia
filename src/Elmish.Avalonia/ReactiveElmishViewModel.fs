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
open System.Linq.Expressions

type IReactiveElmishViewModel<'Model, 'Msg> =
    abstract member Dispatch: 'Msg -> unit
    abstract member Bind: modelProjection: ('Model ->'ModelProjection) * [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName: string -> 'ModelProjection
    abstract member Model: 'Model with get

[<AbstractClass>]
type ReactiveElmishViewModel<'Model, 'Msg>(initialModel: 'Model) = 
    inherit ReactiveUI.ReactiveObject()

    let propertyChanged = Event<_, _>()
    let _modelSubject = new Subject<'Model>()
    let mutable _model: 'Model = initialModel
    let mutable _dispatch: 'Msg -> unit = 
        fun _ -> 
            if not Design.IsDesignMode 
            then failwith "`Dispatch` failed because the Elmish loop has not been started."

    /// Tracks subscriptions of boxed 'Model projections by the VM property name.
    let propertySubscriptions = Dictionary<string, IDisposable>()

    member this.Model = _model

    member this.ModelObservable = _modelSubject.AsObservable()
    
    /// Starts the Elmish loop for this view model.
    abstract member StartElmishLoop : Control -> unit
    interface IElmishViewModel with
        member this.StartElmishLoop(view: Control) = this.StartElmishLoop(view)

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = propertyChanged.Publish

    /// Fires the `PropertyChanged` event for the given property name. Uses the caller's name if no property name is given.
    member this.OnPropertyChanged([<CallerMemberName; Optional; DefaultParameterValue("")>] ?propertyName: string) =
        propertyChanged.Trigger(this, PropertyChangedEventArgs(propertyName.Value))

    interface IReactiveElmishViewModel<'Model, 'Msg> with
        member this.Dispatch msg = _dispatch msg

        member this.Bind(modelProjection: 'Model -> 'ModelProjection, [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName) = 
            this.BindModel(vmPropertyName.Value, modelProjection)
        
        member this.Model with get () = _model

    /// Dispatches a message to the Elmish loop. 
    /// NOTE: will throw an exception if called before the Elmish loop has started.
    member this.Dispatch msg = _dispatch msg

    /// Binds a VM property to a 'Model projection and refreshes the VM property when the 'Model projection changes.
    member this.Bind(modelProjection: 'Model -> 'ModelProjection, [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName) = 
        this.BindModel(vmPropertyName.Value, modelProjection)

    /// Binds a VM property to a 'Model projection and refreshes the VM property when the 'Model projection changes.
    [<Obsolete "ElmishViewModel is deprecated and will be removed in v2. Please use `Bind` instead.">]
    member this.BindModel(vmPropertyName: string, modelProjection: 'Model -> 'ModelProjection) = 
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
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
            propertySubscriptions.Add(vmPropertyName, disposable)

        // Returns the latest value from the model projection.
        _model |> modelProjection

    /// Binds this VM to the view `DataContext` and runs the Elmish loop.
    member internal this.RunProgram (program: Elmish.Program<unit, 'Model, 'Msg, unit>, view: Control) =
        
        // Updates when the Elmish model changes and sends out an Rx stream.
        let setState model (_: Dispatch<'Msg>) =
            _model <- model
            _modelSubject.OnNext(model)

        // A fn that dispatches messages to the Elmish loop.
        let withDispatch (innerDispatch: Dispatch<'Msg>) : Dispatch<'Msg> =
            let dispatch msg = 
                if Dispatcher.UIThread.CheckAccess()
                then innerDispatch msg |> ignore
                else Dispatcher.UIThread.Post(fun () -> innerDispatch msg)

            _dispatch <- dispatch
            dispatch

        // Wires up the view and the VM.
        view.DataContext <- this

        if this.DisposeOnUnload then
            // Disposes the VM when the view is unloaded, and optionally dispatches a termination message.
            view.Unloaded.AddHandler(fun _ _ -> 
                this.TerminateMsg |> Option.iter _dispatch
                (this :> IDisposable).Dispose())
        
        program
        |> Program.withSetState setState
        |> Program.runWithDispatch withDispatch ()

    /// Determines whether this VM should be disposed when the view is unloaded. Default is true.
    member val DisposeOnUnload = true with get, set

    member val internal TerminateMsg: 'Msg option = None with get, set

    interface IDisposable with
        member this.Dispose() =
            propertySubscriptions.Values |> Seq.iter _.Dispose()
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
        then vm.RunProgram(program, view)

    /// Configures `Program.withTermination` using the given terminate 'Msg, and dispatches the 'Msg when the view is `Unloaded`.
    let terminateOnViewUnloaded (vm: ReactiveElmishViewModel<'Model, 'Msg>) (terminateMsg: 'Msg) program = 
        vm.TerminateMsg <- Some terminateMsg
        program 
        |> Program.withTermination (fun m -> m = terminateMsg) (fun _ -> printfn $"terminateOnViewUnloaded: dispatching {terminateMsg}")

