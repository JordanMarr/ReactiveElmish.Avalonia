namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish

type IElmishViewModel =
    abstract member StartElmishLoop : view: Avalonia.Controls.Control -> unit

/// Used to bind a view with a view model and provides a method to start the Elmish loop.
type ElmishViewModel<'model, 'msg>(program: AvaloniaProgram<'model, 'msg>) =
    
    member val ViewProgramAugmentations : (Avalonia.Controls.Control -> AvaloniaProgram<'model, 'msg> -> AvaloniaProgram<'model, 'msg>) ResizeArray = ResizeArray()

    interface IElmishViewModel with
        member this.StartElmishLoop(view: Avalonia.Controls.Control) = 
            this.ViewProgramAugmentations
            |> Seq.fold (fun p fn -> fn view p) program
            |> AvaloniaProgram.startElmishLoop view

module ElmishViewModel = 
    
    /// Initializes an ElmishViewModel with the given program.
    let create program = ElmishViewModel(program)

    /// Creates subscriptions with the given view.
    let withViewSubscription subscriptions (vm: ElmishViewModel<'model, 'msg>) = 
        vm.ViewProgramAugmentations.Add(fun view program -> 
            program |> AvaloniaProgram.withSubscription (subscriptions view)
        )
        vm
             
    let terminateOnUnloaded (msg: 'msg) (vm: ElmishViewModel<'model, 'msg>) = 
        vm.ViewProgramAugmentations
            .Add(fun view program -> 
                program 
                |> AvaloniaProgram.mapSubscription (fun oldSubs -> 
                    fun model -> 
                        let viewUnloadedSub (dispatch: 'msg -> unit) = 
                            view.Unloaded |> Observable.subscribe(fun e -> dispatch msg)

                        oldSubs model @
                            [
                                [ nameof viewUnloadedSub ], viewUnloadedSub
                            ]
                )
            )

        vm.ViewProgramAugmentations
           .Add(fun view program -> 
                program 
                |> AvaloniaProgram.withTermination
                    (fun m -> m = msg)
                    (fun _ -> printfn "View unloaded; terminating loop.")
            )
            
        vm
        
