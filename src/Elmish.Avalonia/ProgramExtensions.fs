namespace Elmish.Avalonia

open Elmish
open Avalonia.Controls
open System


module Program =
    /// Creates an Avalonia program via Program.mkProgram.
    let mkAvaloniaProgram (init: unit -> 'Model * Cmd<'Msg>) update = 
        Program.mkProgram init update (fun _ _ -> ())

    /// Creates an Avalonia program via Program.mkSimple.
    let mkAvaloniaSimple (init: unit -> 'Model) update =
        Program.mkSimple init update (fun _ _ -> ())

    /// Binds the vm to the view and then runs the Elmish program.    
    let runView (vm: IRunProgram<'Model, 'Msg>) (view: Control) program = 
        if not Design.IsDesignMode 
        then vm.RunProgram(program, view)

    let mkStore (program: Program<unit, 'Model, 'Msg, unit>) = 
        new ElmishStore<'Model, 'Msg>(program)

    /// Configures `Program.withTermination` using the given terminate 'Msg, and dispatches the 'Msg when the view is `Unloaded`.
    let terminateOnViewUnloaded (vm: ReactiveElmishViewModel<'Model, 'Msg>) (terminateMsg: 'Msg) program = 
        vm.TerminateMsg <- Some terminateMsg
        program 
        |> Program.withTermination (fun m -> m = terminateMsg) (fun _ -> printfn $"terminateOnViewUnloaded: dispatching {terminateMsg}")



