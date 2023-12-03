namespace AvaloniaExample.ViewModels

open ReactiveElmish
open ReactiveElmish.Avalonia
open App
open System

module TodoApp = 
    open Elmish

    type Model = { Todos: DynamicData.SourceCache<Todo, Guid> }
    and Todo = { Id: Guid; Description: string; Completed: bool }

    type Msg = 
        | AddTodo
        | RemoveTodo of id: Guid
        | UpdateTodo of Todo
        | Clear

    let init() = 
        { 
            Todos = SourceCache.create(_.Id)
        }, Cmd.ofMsg AddTodo


    let update (msg: Msg) (model: Model) = 
        match msg with
        | AddTodo ->
            { 
                Todos = 
                    model.Todos 
                    |> SourceCache.addOrUpdate { Id = Guid.NewGuid(); Description = $"Todo {model.Todos.Count + 1}"; Completed = false }
            }, Cmd.none

        | RemoveTodo id ->
            { 
                Todos = model.Todos |> SourceCache.removeKey id
            }, Cmd.none

        | UpdateTodo todo ->
            { 
                Todos = model.Todos |> SourceCache.addOrUpdate todo
            }, Cmd.none

        | Clear -> 
            { 
                Todos = model.Todos |> SourceCache.clear
            }, Cmd.none

    let store = 
        Program.mkAvaloniaProgram init update
        |> Program.mkStore

open TodoApp

type TodoViewModel(todo: Todo) = 
    inherit ReactiveElmishViewModel()

    member this.Id with get () = todo.Id

    member this.Description 
        with get () = todo.Description
        and set value = store.Dispatch(UpdateTodo {todo with Description = value })

    member this.Completed
        with get () = todo.Completed
        and set value = store.Dispatch(UpdateTodo {todo with Completed = value })

    member this.RemoveTodo() = 
        store.Dispatch(RemoveTodo todo.Id)


type TodoListViewModel() =
    inherit ReactiveElmishViewModel()

    member this.Todos = 
        this.BindSourceCache(
            store.Model.Todos 
            , fun todo -> new TodoViewModel(todo)
            , fun todo -> todo.Completed, todo.Description
        )
    member this.AddTodo() = store.Dispatch AddTodo
    member this.Clear() = store.Dispatch Clear

    static member DesignVM = new TodoListViewModel()