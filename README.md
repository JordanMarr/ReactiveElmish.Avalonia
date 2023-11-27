# ReactiveElmish.Avalonia [![NuGet version (ReactiveElmish.Avalonia)](https://img.shields.io/nuget/v/ReactiveElmish.Avalonia.svg?style=flat-square)](https://www.nuget.org/packages/ReactiveElmish.Avalonia/)
Static Avalonia views for Elmish programs

## Why?
Avalonia.FuncUI and Fabulous.Avalonia already exist and they are both fantastic. So then why make an Elmish.Avalonia port?

### Benefits
* Some people may prefer using static xaml views, and it can be an easier sell for some teams due to the immediate availability of all community controls.
* Bindings have to be created for controls in FuncUI. While most of the controls have bindings provided, third party will not have bindings out-of-the-box. Elmish.Avalonia sidesteps this problem by using xaml directly.
* Ability to use the excellent Avalonia design previewer. For me to do any kind of real project work with Avalonia and F#, a design previewer is a necessity. Also, being able to easily construct `DesignInstance` VM for each view that utilizes the Elmish `init` function. 
* You can still use Avalonia.FuncUI and Elmish.Avalonia side-by-side to have the best of both worlds approach if you like using the FuncUI DSL!

### Additional reasons
* Avalonia UI is a big deal in the .NET OSS community; it is always nice for F# community to be able to participate in the latest and greatest with as many options as possible.
* Avalonia already provides first class templates to create an F# project that include creating .axaml views within the same project! (Not possible with WPF!)
* While the built-in F# templates do allow you to do classic MVVM style, Elmish provides a powerful form of state management that has become standard for F# UI projects.
* The "Avalonia UI for Visual Studio 2022" extension provides a xaml preview pane that works with F#! 😄 (Also not possible with WPF!)
* Keeping with tradition that the F# community will [provide important libraries, developer tools and workload support](https://learn.microsoft.com/en-us/dotnet/fsharp/strategy).

# History
## V1
V1 of this project was a port of the awesome [Elmish.WPF](https://github.com/elmish/Elmish.WPF) library.
Most of the v1 codebase was directly copied, and the WPF bits were replaced with Avalonia bits and adapted where necessary.
The V1 bindings (see below) were translated into an internal `DictionaryViewModel` behind the scenes that was bound to the view's `DataContext`. 
![image](https://github.com/JordanMarr/Elmish.Avalonia/assets/1030435/00988e96-6905-46fa-9d89-25f7bab6881f)

## V2 (beta)
The V2 beta is evolving into a complete rewrite, and all the code that was copied from Elmish.WPF is planned to be removed.
My vision for this library departs from the typical "monolithic" Elmish app. Instead, it uses more of a modular Elmish approach where each view model can run its own Elmish loop.

At the heart of V2 is the new `ReactiveElmishViewModel` base class, which inherits `ReactiveUI.ReactiveObject`. 
Instead of using the V1 bindings, you now create a more standard view model that has able properties. A new `` method will take care of binding your view model properties to Elmish model projections. 

![image](https://github.com/JordanMarr/Elmish.Avalonia/assets/1030435/5278afe4-ce05-4548-b9e9-6a1703394fd7)


### V2 Design Highlights
* Works with Avalonia [Compiled bindings](https://docs.avaloniaui.net/docs/next/basics/data/data-binding/compiled-bindings#enable-and-disable-compiled-bindings) for better performance and compile-time type checking in the views. With Compiled bindings enabled, the build will fail if the view references a binding that doesn't exist in the VM! (The previous `DictionaryViewModel` brought over from Elmish.WPF was not able to take advantage of this because it relied on reflection-based bindings.)
* More standard looking view model pattern while still maintaining the power of Elmish. For example, you can now create an instance of an Elmish view model and actually inspect its properties from the outside -- and even read / write to the properties in OOP fashion. (The fact that a view model is using Elmish internally should not matter because it's an implementation detail.) This is a perfect example of the benefits of OOP + FP side-by-side.
* Elmish.Avalonia now takes a dependency on the Avalonia.ReactiveUI library. (The new `ReactiveElmishViewModel` class inherits from `ReactiveObject`.) Since this is the default view model library for Avalonia, this makes it easier to take advantage of existing patterns when needed.
* Elmish.Avalonia provides custom integration for `ReactiveUI.DynamicData` which provides a very simple way to  lists between the Elmish model and the view / view model.
* Built-in dependency injection using "Microsoft.Extensions.DependencyInjection".

## Design View
Don't forget to install the "Avalonia for Visual Studio 2022" extension.
JetBrains Rider also supports Avalonia previews out-of-the-box!
https://docs.avaloniaui.net/docs/getting-started/ide-support

![image](https://user-images.githubusercontent.com/1030435/219173023-a47d5d9b-8926-4f9d-833b-1406661e1c82.png)

## Runtime View
![image](https://user-images.githubusercontent.com/1030435/219145003-b4168921-ddab-41bc-92ea-d3f432fbc844.png)

# Elmish Stores
V2 introduces the `ElmishStore` which is an Rx powered Elmish loop that can be used to power one or more view models.
This provides flexibility for how you want to configure your viewmodels. 
* Each view model can have its own `ElmishStore`.
* View models may share a store.
* Some view models may not need a store at all.

## App Store
A global app store can be shared between view models to provide view routing:

```F#
module App

open System
open Elmish
open Elmish.Avalonia

type Model =  
    { 
        View: View
    }

and View = 
    | CounterView
    | ChartView
    | AboutView
    | FilePickerView

type Msg = 
    | SetView of View
    | GoHome

let init () = 
    { 
        View = CounterView
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | SetView view -> { View = view }   
    | GoHome -> { View = CounterView }


let app = 
    Program.mkAvaloniaSimple init update
    |> Program.withErrorHandler (fun (_, ex) -> printfn $"Error: {ex.Message}")
    |> Program.withConsoleTrace
    |> Program.mkStore
```

## Accessing the Global App Store
In this example, a simple `AboutViewModel` can access the global `App` store to dispatch a custom navigation message when the `Ok` button is clicked:
```F#
namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open App

type AboutViewModel() =
    inherit ReactiveElmishViewModel()

    member this.Version = "v1.0"
    member this.Ok() = app.Dispatch GoHome

    static member DesignVM = 
        new AboutViewModel()
```

## View Model with its own Store
In this example, a view model has its own local store, and it also accesses the global App store:
```F#
namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open AvaloniaExample

module FilePicker = 

    type Model = 
        {
            FilePath: string option
        }

    type Msg = 
        | SetFilePath of string option

    let init () = 
        { 
            FilePath = None
        }

    let update (msg: Msg) (model: Model) = 
        match msg with
        | SetFilePath path ->
            { FilePath = path }


open FilePicker

type FilePickerViewModel(fileSvc: FileService) =
    inherit ReactiveElmishViewModel()

    let app = App.app

    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.mkStore

    member this.FilePath = this. (local, _.FilePath >> Option.defaultValue "Not Set")
    member this.Ok() = app.Dispatch App.GoHome
    member this.PickFile() = 
        task {
            let! path = fileSvc.TryPickFile()
            local.Dispatch (SetFilePath path)
        }

    static member DesignVM = new FilePickerViewModel(Design.stub)

```

# Creating an Elmish Store
Opening the `Elmish.Avalonia` and `Elmish` namespaces adds the following extensions to `Program`:

### Program.mkAvaloniaProgram
Creates a store via Program.mkProgram (`init` and `update` functions return a `Model * Cmd` tuple).

```F#
let store = 
   Program.mkAvaloniaProgram init update
   |> Program.mkStore
```

### Program.mkAvaloniaSimple
Creates an Avalonia program via Program.mkSimple. (`init` and `update` functions return a `Model`).

```F#
let store = 
   Program.mkAvaloniaSimple init update
   |> Program.mkStore
```

### Program.withSubscription
Creates one or more Elmish subscriptions that can dispatch messages and be enabled/disabled based on the model.

```F#
let subscriptions (model: Model) : Sub<Msg> =
   let autoUpdateSub (dispatch: Msg -> unit) = 
      Observable
          .Interval(TimeSpan.FromSeconds(1))
          .Subscribe(fun _ -> 
              dispatch AddItem
          )

   [
      if model.IsAutoUpdateChecked then
         [ nameof autoUpdateSub ], autoUpdateSub
   ]

```
```F#
let store = 
   Program.mkAvaloniaSimple init update
   |> Program.withSubscription subscriptions
   |> Program.mkStore
```

### Program.mkStoreWithTerminate
Creates a store that configures `Program.withTermination` using the given terminate `'Msg`, and fires the terminate `'Msg` when the `view` is `Unloaded`.
This pattern will dispose your subscriptions when the view is `Unloaded`.
* NOTE 1: You must create a `Terminate` `'Msg` that will be registered to trigger loop termination.
* NOTE 2: This requires that a store be created locally within a view model.

```F#
let update (msg: Msg) (model: Model) =
    // ...
    | Terminate -> model // This is just a stub Msg that needs to exist -- it doesn't need to do anything.
```

```F#
    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.withErrorHandler (fun (_, ex) -> printfn $"Error: {ex.Message}")
        |> Program.mkStoreWithTerminate this Terminate 
```
# ReactiveElmishViewModel

## View Model Bindings
The `ReactiveElmishViewModel` base class contains binding methods that are used to bind data between your Elmish model and your view model.
All binding methods on the `ReactiveElmishViewModel` are disposed when the view model is diposed.

### `Bind`
The `Bind` method binds data from an `IElmishStore` to a property on your view model. This can be a simple model property or a projection based on the model.
```F#
type CounterViewModel() =
    inherit ReactiveElmishViewModel()

    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.mkStore

    member this.Count = this.Bind(local, _.Count)
    member this.IsResetEnabled = this.Bind(local, fun m -> m.Count <> 0)
```

### `BindOnChanged`
The `BindOnChanged` method binds a VM property to a `modelProjection` value and refreshes the VM property when the `onChanged` value changes. The `modelProjection` function will only be called when the `onChanged` value changes. `onChanged` usually returns a property value or a tuple of property values.

This was added to avoid evaluating an expensive model projection more than once. For example, when evaluating the current `ContentView` property on the `MainViewModel`. Using `Bind` in this case would execute the `modelProjection` twice: once to determine if the value had changed, and then again to bind to the property. Using `BindOnChanged` will simply check to see if the `_.View` property changed on the model instead of evaluating the `modelProjection` twice, thereby creating the current view twice.

```F#
namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open App

type MainViewModel(root: CompositionRoot) =
    inherit ReactiveElmishViewModel()
    
    member this.ContentView = 
        this.BindOnChanged (app, _.View, fun m -> 
            match m.View with
            | CounterView -> root.GetView<CounterViewModel>()
            | AboutView -> root.GetView<AboutViewModel>()
            | ChartView -> root.GetView<ChartViewModel>()
            | FilePickerView -> root.GetView<FilePickerViewModel>()
        )

    member this.ShowChart() = app.Dispatch (SetView ChartView)
    member this.ShowCounter() = app.Dispatch (SetView CounterView)
    member this.ShowAbout() = app.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = app.Dispatch (SetView FilePickerView)

    static member DesignVM = new MainViewModel(Design.stub)
```

### `BindSourceList`
The `BindSourceList` method binds a `ReactiveUI` [`SourceList`](https://www.reactiveui.net/docs/handbook/collections) property on the `Model` to a view model property. 
This provides list `Add` and `Removed` notifications to the view.
There is also a `SourceList` helper module that makes it a little nicer to work with.

```F#
    let update (msg: Msg) (model: Model) = 
        match msg with
        | Increment ->
            { 
                Count = model.Count + 1 
                Actions = model.Actions |> SourceList.add { Description = "Incremented"; Timestamp = DateTime.Now }
            }
        | Decrement ->
            { 
                Count = model.Count - 1 
                Actions = model.Actions |> SourceList.add { Description = "Decremented"; Timestamp = DateTime.Now }
            }
        | Reset ->
            {
                Count = 0 
                Actions = model.Actions |> SourceList.clear |> SourceList.add { Description = "Reset"; Timestamp = DateTime.Now }
            }

```
```F#
type CounterViewModel() =
    inherit ReactiveElmishViewModel()

    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.mkStore

    member this.Actions = this.BindSourceList(local, _.Actions)
```

### `BindSourceCache`
The `BindSourceCache` method binds a `ReactiveUI` [`SourceCache`](https://www.reactiveui.net/docs/handbook/collections) property on the `Model` to a view model property. 
This provides list `Add` and `Removed` notifications to the view for lists with items that have unique keys.
There is also a `SourceCache` helper module that makes it a little nicer to work with.

```F#
    type Model =
        {
            FileQueue: SourceCache<File, string>
        }

    let init () =
        {
            FileQueue = SourceCache.create _.FullName
        }

    let update message model =
        | QueueFile path ->
            let file = mkFile path
            { model with FileQueue = model.FileQueue |> SourceCache.addOrUpdate file }
        | UpdateFileStatus (file, progress, moveFileStatus) ->
            let updatedFile = { file with Progress = progress; Status = moveFileStatus }
            { model with FileQueue = model.FileQueue |> SourceCache.addOrUpdate updatedFile }
        | RemoveFile file ->
            { model with FileQueue = model.FileQueue |> SourceCache.removeKey file.FullName}
```
```F#
type MainWindowViewModel() as this =
    inherit ReactiveElmishViewModel()

    member this.FileQueue = this.BindSourceCache(store, _.FileQueue)
```

## GetView
The `GetView<'ViewModel>` method gets a view/VM (based on your `CompositionRoot` configuration).
```F#
type MainViewModel() =
    inherit ReactiveElmishViewModel()

    member this.ContentView = 
        this.Bind (app, fun m -> 
            match m.View with
            | CounterView -> this.GetView<CounterViewModel>()
            | AboutView -> this.GetView<AboutViewModel>()
            | ChartView -> this.GetView<ChartViewModel>()
            | FilePickerView -> this.GetView<FilePickerViewModel>()
        )
```

# Composition Root
The composition root is where you register your views/vms as well as any injected services.

* `RegisterServices` allows you to specify dependencies that can be injected into other view model and service constructors. View models are automatically injected on app load.
* `RegisterViews` allows you to pair up your views and view models and assign them a lifetime. 
* Views can be registered with two lifetimes:
  * `Transient` - view/VM will both be recreated every time `GetView` is called; VM and it subscriptions will be disposed on view Unloaded.
  * `Singleton` - view/VM will both be created only once and then reused on subsequent calls to `GetView`. (VM is never Disposed.)

```F#
namespace AvaloniaExample

open Elmish.Avalonia
open Microsoft.Extensions.DependencyInjection
open AvaloniaExample.ViewModels
open AvaloniaExample.Views

type AppCompositionRoot() =
    inherit CompositionRoot()

    let mainView = MainView()

    override this.RegisterServices services = 
        services.AddSingleton<FileService>(FileService(mainView))

    override this.RegisterViews() = 
        Map [
            VM.Key<MainViewModel>(), View.Singleton(mainView)
            VM.Key<CounterViewModel>(), View.Singleton<CounterView>()
            VM.Key<AboutViewModel>(), View.Singleton<AboutView>()
            VM.Key<ChartViewModel>(), View.Singleton<ChartView>()
            VM.Key<FilePickerViewModel>(), View.Singleton<FilePickerView>()
        ]
```


# Project Setup

Steps to create a new project:

1) Create a new project using the [Avalonia .NET MVVM App Template for F#](https://github.com/AvaloniaUI/avalonia-dotnet-templates).
2) Install the Elmish.Avalonia package from NuGet.
3) Create an `AppCompositionRoot` that inherits from `CompositionRoot` to define your view/VM pairs and services.
4) Launch the startup window using your `CompositionRoot` class in the `App.axaml.fs` 

Refer to the `AvaloniaExample` project in the `Samples` directory as a reference.

```F#
namespace AvaloniaExample

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Controls.ApplicationLifetimes

type App() =
    inherit Application()

    override this.Initialize() =
        // Initialize Avalonia controls from NuGet packages:
        let _ = typeof<Avalonia.Controls.DataGrid>

        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->         
            let appRoot = AppCompositionRoot()
            desktop.MainWindow <- appRoot.GetView<ViewModels.MainViewModel>() :?> Window
        | _ -> 
            // leave this here for design view re-renders
            ()

        base.OnFrameworkInitializationCompleted()

```


# Sample Project
The included sample app shows a obligatory Elmish counter app, and also the Avalonia DataGrid control.
Please view the [AvaloniaExample project](https://github.com/JordanMarr/Elmish.Avalonia/tree/v2-beta/src/Samples/AvaloniaExample).
