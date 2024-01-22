namespace AvaloniaExample.ViewModels

open ReactiveElmish
open ReactiveElmish.Avalonia
open App
open System
open Avalonia.Controls

module TodoApp = 
    open Elmish

    type Model = { Todos: Map<Guid, Todo> }
    and Todo = { Id: Guid; Description: string; Completed: bool }

    type Msg = 
        | AddTodo
        | RemoveTodo of id: Guid
        | UpdateTodo of Todo
        | Clear

    let init() = 
        if Design.IsDesignMode then 
            { Todos = 
                Map [ 
                    let guid1 = Guid.NewGuid()
                    guid1, { Id = guid1; Description = "Todo 1"; Completed = false }
                    let guid2 = Guid.NewGuid()
                    guid2, { Id = guid2; Description = "Todo 2"; Completed = true }
                ]                
            }, Cmd.none
        else
            { Todos = Map.empty
            }, Cmd.ofMsg AddTodo


    let update (msg: Msg) (model: Model) = 
        match msg with
        | AddTodo ->
            { Todos = 
                let guid = Guid.NewGuid()
                model.Todos.Add(guid, { Id = guid; Description = $"Todo {model.Todos.Count + 1}"; Completed = false })
            }, Cmd.none

        | RemoveTodo id ->
            { Todos = model.Todos.Remove(id)
            }, Cmd.none

        | UpdateTodo todo ->
            { Todos = 
                model.Todos.Add(todo.Id, todo)
            }, Cmd.none

        | Clear -> 
            { Todos = Map.empty
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
            store.Dispatch(UpdateTodo { todo with Description = value })
            base.OnPropertyChanged()

    member this.Completed
        with get () = completed
        and set value = 
            store.Dispatch(UpdateTodo { todo with Completed = value })
            base.OnPropertyChanged()

    member this.RemoveTodo() = 
        store.Dispatch(RemoveTodo todo.Id)

    member this.Update(todo: Todo) = 
        completed <- todo.Completed
        description <- todo.Description


type TodoListViewModel() =
    inherit ReactiveElmishViewModel()

    let store = 
        Program.mkAvaloniaProgram init update
        |> Program.mkStore

    member this.Todos = 
        this.BindMap(
            store
            , _.Todos
            , create = fun todo -> new TodoViewModel(store, todo)
            , getKey = fun todo -> todo.Id
            , update = fun todo todoVM -> todoVM.Update(todo)
            , sortBy = fun todo -> todo.Completed, todo.Description
        )

    member this.AddTodo() = store.Dispatch AddTodo
    member this.Clear() = store.Dispatch Clear

    static member DesignVM = new TodoListViewModel()
