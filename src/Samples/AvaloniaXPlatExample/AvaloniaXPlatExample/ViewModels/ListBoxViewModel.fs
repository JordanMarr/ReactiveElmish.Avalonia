namespace AvaloniaXPlatExample.ViewModels

open Elmish
open ReactiveElmish
open ReactiveElmish.Avalonia

module ListBox =
    type DNADoc = { Name : string ; Count : int}

    type Model =
        {
            Greeting : string option
            Items: DNADoc list
            SelectedIndex : int option
        }

    type Msg =
        | Delete
        | Add
        | ChangeGreeting of string
        | Clear
        | DoubleTapped
        | Selected of int

    let init() =
        {   Greeting= None
            SelectedIndex = None
            Items = [
                {Name="Meredith" ; Count = 56}
                {Name="had" ; Count = 3}
                {Name="a" ; Count = 1}
                {Name="little" ; Count = 7}
                {Name="lamb" ; Count = 1000} ]
        },Cmd.none

    let update (msg: Msg) (model: Model) =
        match msg with
        | Clear ->
            {model with Items = []},Cmd.none
        | Add ->
            match model.Greeting with
            | None ->
                {model with Greeting = Some "put some text here first"},Cmd.none
            | Some text ->
                {model with Items = {Name=text ; Count = 1}::model.Items ; Greeting = None},Cmd.none
        | ChangeGreeting v ->
            {model with Greeting = Some v},Cmd.none
        | Selected i ->
            {model with SelectedIndex = Some i ; Greeting = if i = -1 then None else Some $"picked {i}"},Cmd.none
        | DoubleTapped ->
            match model.SelectedIndex with
            | None ->
                {model with Greeting = Some $"choose an item"},Cmd.none
            | Some i ->
                {model with Greeting = Some $"doubletapped row {i}"},Cmd.none
        | Delete ->
            match model.SelectedIndex with
            | Some row when row > -1 ->
                {model with Items = model.Items |> List.removeAt row },Cmd.none
            | _ -> {model with Greeting = Some "Select an item first!"},Cmd.none

open ListBox

type ListBoxViewModel() =
    inherit ReactiveElmishViewModel()

    let local =
        Program.mkAvaloniaProgram init update
        |> Program.mkStore

    member this.Greeting
        with get() = this.Bind(local, fun m -> m.Greeting |> Option.defaultValue "")
        and set value = local.Dispatch (ChangeGreeting value)
    member this.SelectedIndex
        with get() = this.Bind (local,fun m -> m.SelectedIndex |> Option.defaultValue -1)
        and set value = local.Dispatch (Selected value)

    member this.Items = this.Bind(local, _.Items)

    member this.DoubleTapped() = local.Dispatch DoubleTapped
    member this.Delete() = local.Dispatch Delete
    member this.Clear() = local.Dispatch Clear
    member this.Add() = local.Dispatch Add

    static member DesignVM = new ListBoxViewModel()
