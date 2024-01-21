namespace AvaloniaExample.ViewModels

open ReactiveElmish
open ReactiveElmish.Avalonia
open App
open System
open Avalonia.Controls

module TodoApp = 
    open Elmish

    type Model = { Todos: Todo list }
    and Todo = { Id: Guid; Description: string; Completed: bool }

    type Msg = 
        | AddTodo
        | RemoveTodo of id: Guid
        | UpdateTodo of Todo
        | Clear

    let init() = 
        if Design.IsDesignMode then 
            { Todos = 
                [ 
                    { Id = Guid.NewGuid(); Description = "Todo 1"; Completed = false }
                    { Id = Guid.NewGuid(); Description = "Todo 2"; Completed = true }
                ]                
            }, Cmd.none
        else
            { Todos = []
            }, Cmd.ofMsg AddTodo


    let update (msg: Msg) (model: Model) = 
        match msg with
        | AddTodo ->
            { Todos = model.Todos @ [ { Id = Guid.NewGuid(); Description = $"Todo {model.Todos.Length + 1}"; Completed = false } ]
            }, Cmd.none

        | RemoveTodo id ->
            { Todos = model.Todos |> List.filter (fun t -> t.Id <> id)
            }, Cmd.none

        | UpdateTodo todo ->
            { Todos = 
                model.Todos |> List.map (fun t -> if t.Id = todo.Id then todo else t)
            }, Cmd.none

        | Clear -> 
            { Todos = []
            }, Cmd.none


open TodoApp

type TodoViewModel(store: IStore<Model, Msg>, todo: Todo) = 
    inherit ReactiveElmishViewModel()

    let mutable completed = todo.Completed
    let mutable description = todo.Description

    member this.Id with get () = todo.Id

    member this.Description 
        with get () = description
        and set value = 
            description <- value
            store.Dispatch(UpdateTodo { todo with Description = value })
            base.OnPropertyChanged()

    member this.Completed
        with get () = completed
        and set value = 
            completed <- value
            store.Dispatch(UpdateTodo { todo with Completed = value })
            base.OnPropertyChanged()

    member this.RemoveTodo() = 
        store.Dispatch(RemoveTodo todo.Id)


type TodoListViewModel() =
    inherit ReactiveElmishViewModel()

    let store = 
        Program.mkAvaloniaProgram init update
        |> Program.mkStore

    member this.Todos = 
        this.BindKeyedList(
            store
            , _.Todos
            , getKey = _.Id
            , create = fun todo -> new TodoViewModel(store, todo)
            //, update = fun vm todo -> 
            //    vm.Completed <- todo.Completed
            //    vm.Description <- todo.Description; 
            , sortBy = 
                fun todo -> todo.Completed, todo.Description
        )

    member this.AddTodo() = store.Dispatch AddTodo
    member this.Clear() = store.Dispatch Clear

    static member DesignVM = new TodoListViewModel()
