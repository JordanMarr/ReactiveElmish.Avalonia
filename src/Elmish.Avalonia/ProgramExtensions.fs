namespace Elmish.Avalonia

open Elmish
open System

module Program =
    /// Makes an Avalonia program via Program.mkProgram.
    let mkAvaloniaProgram (init: unit -> 'Model * Cmd<'Msg>) update = 
        Program.mkProgram init update (fun _ _ -> ())

    /// Makes an Avalonia program via Program.mkSimple.
    let mkAvaloniaSimple (init: unit -> 'Model) update =
        Program.mkSimple init update (fun _ _ -> ())

    /// Makes a reactive Elmish store from a program.
    let mkStore (program: Program<unit, 'Model, 'Msg, unit>) = 
        new AvaloniaStore<'Model, 'Msg>(program) 
        :> IStore<'Model, 'Msg>

    /// Makes a reactive Elmish store from a program that terminates on the given "Terminate" 'Msg when the VM is disposed.
    let mkStoreWithTerminate (vm: ReactiveElmishViewModel) (terminateMsg: 'Msg) (program: Program<unit, 'Model, 'Msg, unit>) = 
        let prog = program |> Program.withTermination (fun m -> m = terminateMsg) (fun _ -> printfn $"Terminating store: dispatching {terminateMsg}")
        let store = new AvaloniaStore<'Model, 'Msg>(prog) :> IStore<'Model, 'Msg>
        let disposable = 
            { new IDisposable with 
                member this.Dispose() = 
                    store.Dispatch terminateMsg }
        vm.AddDisposable disposable
        store
