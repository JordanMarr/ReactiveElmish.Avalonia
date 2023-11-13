namespace Elmish.Avalonia

open System
open Elmish
open Avalonia.Controls

type IElmishViewModel =
    abstract member StartElmishLoop : view: Control -> unit

/// Used to bind a view with a view model and provides a method to start the Elmish loop.
[<Obsolete("ElmishViewModel is deprecated and will be removed in v2. Please use ReactiveElmishViewModel.")>]
type ElmishViewModel<'model, 'msg>(program: AvaloniaProgram<'model, 'msg>) =
    
    member val internal ViewProgramAugmentations 
        : (Control -> AvaloniaProgram<'model, 'msg> -> AvaloniaProgram<'model, 'msg>) ResizeArray = ResizeArray()

    interface IElmishViewModel with
        member this.StartElmishLoop(view: Control) = 
            this.ViewProgramAugmentations
            |> Seq.fold (fun p fn -> fn view p) program
            |> AvaloniaProgram.startElmishLoop view

/// Helper functions for creating ElmishViewModels.
[<Obsolete("ElmishViewModel is deprecated and will be removed in v2. Please use ReactiveElmishViewModel.")>]
module ElmishViewModel = 
    
    /// Initializes an ElmishViewModel with the given program.
    let create program = ElmishViewModel(program)

    /// Creates Elmish subscriptions with the given view.
    let withViewSubscription subscriptions (vm: ElmishViewModel<'model, 'msg>) = 
        vm.ViewProgramAugmentations.Add(fun view program -> 
            program |> AvaloniaProgram.withSubscription (subscriptions view)
        )
        vm
             
    /// Adds an Elmish subscription that terminates the Elmish loop when the view is unloaded.
    /// NOTE: This must be called after `withViewSubscription` if both are used.
    let terminateOnViewUnloaded (terminateMsg: 'msg) (vm: ElmishViewModel<'model, 'msg>) = 
        vm.ViewProgramAugmentations
            .Add(fun view program -> 
                program 
                |> AvaloniaProgram.mapSubscription (fun oldSubs -> 
                    fun model -> 
                        let viewUnloadedSub (dispatch: 'msg -> unit) = 
                            view.Unloaded |> Observable.subscribe(fun _ -> dispatch terminateMsg)

                        oldSubs model @
                            [
                                [ nameof viewUnloadedSub ], viewUnloadedSub
                            ]
                )
            )

        vm.ViewProgramAugmentations
           .Add(fun _ program -> 
                program 
                |> AvaloniaProgram.withTermination
                    (fun m -> m = terminateMsg)
                    (fun _ -> printfn "View unloaded; terminating loop.")
            )            
        vm

    /// Adds an Elmish subscription.
    let subscribe (subscribeToEvent: Control -> 'model -> ('msg -> unit) -> IDisposable) (vm: ElmishViewModel<'model, 'msg>) = 
        vm.ViewProgramAugmentations
           .Add(fun view program -> 
                program 
                |> AvaloniaProgram.mapSubscription (fun oldSubs -> 
                    fun model -> 
                        let subscription (dispatch: 'msg -> unit) = 
                            subscribeToEvent view model dispatch

                        oldSubs model @
                            [
                                [ $"subscription_{Guid.NewGuid()}" ], subscription
                            ]
                )
            )
        vm
        
