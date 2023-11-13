# Elmish.Avalonia [![NuGet version (Elmish.Avalonia)](https://img.shields.io/nuget/v/Elmish.Avalonia.svg?style=flat-square)](https://www.nuget.org/packages/Elmish.Avalonia/)
Static Avalonia views for Elmish programs

## Why?
Avalonia.FuncUI already exists and it is fantastic. So then why make an Elmish.Avalonia port?

### Benefits
* Some people may prefer using xaml, and it can be an easier sell for some teams due to the immediate availability of all community controls.
* Bindings have to be created for controls in FuncUI. While most of the controls have bindings provided, third party will not have bindings out-of-the-box. Elmish.Avalonia sidesteps this problem by using xaml directly.
* There is not currently a reliable design preview tool for FuncUI (although there is currently a project, [Avalonia.FuncUI.LiveView](https://github.com/SilkyFowl/Avalonia.FuncUI.LiveView), that is working to solve this problem). For me to do any kind of real project work with Avalonia and F#, a design preview is a necessity, and using xaml allows you to utilize the custom Avalonia design preview extension. After recently trying Elmish.WPF, I fell in love with the `ViewModel.designInstance` preview functionality that lets you preview your `init` values in the design preview panel, and this also works with Elmish.Avalonia!
* You can use Avalonia.FuncUI and Elmish.Avalonia side-by-side to have the best of both worlds!

### Other reasons
* Avalonia UI is a big deal in the .NET OSS community; it is always nice for F# community to be able to participate in the latest and greatest with as many options as possible.
* Avalonia already provides first class templates to create an F# project that include creating .axaml views within the same project! (Not possible with WPF!)
* While the built-in F# templates do allow you to do classic MVVM style, Elmish is more powerful and has become the standard for F# UI projects.
* The "Avalonia UI for Visual Studio 2022" extension provides a xaml preview pane that works with F#! 😄 (Also not possible with WPF!)
* The Elmish.WPF `ViewModel.designInstance` concept works with this extension very well. This allows you populate your "Design Preview" window with the defaults from your Elmish `init` function!
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
Instead of using the V1 bindings, you now create a more standard view model that has bindable properties. A new `BindModel` method will take care of binding your view model properties to Elmish model projections. 

![image](https://github.com/JordanMarr/Elmish.Avalonia/assets/1030435/66b76ea0-b008-42b5-8c82-b8d56530879a)


# Sample App
The included sample app shows a obligatory Elmish counter app, and also the Avalonia DataGrid control.

## Design View
Don't forget to install the "Avalonia for Visual Studio 2022" extension.
JetBrains Rider also supports Avalonia previews out-of-the-box!
https://docs.avaloniaui.net/docs/getting-started/ide-support

![image](https://user-images.githubusercontent.com/1030435/219173023-a47d5d9b-8926-4f9d-833b-1406661e1c82.png)

## Runtime View
![image](https://user-images.githubusercontent.com/1030435/219145003-b4168921-ddab-41bc-92ea-d3f432fbc844.png)

## Master View
The sample project uses the `ViewLocator` to instantiate the view, bind the Elmish view model and start the Elmish loop.
Since the design preview is set for both the `MasterView` and the `CounterView`, we are able to see the counter on the `MasterView` design preview!

![image](https://github.com/JordanMarr/Elmish.Avalonia/assets/1030435/e47e1662-b484-4524-b007-718f2d38d232)


# Project Setup

Steps to create a new project:

1) Create a new project using the [Avalonia .NET MVVM App Template for F#](https://github.com/AvaloniaUI/avalonia-dotnet-templates).
2) Install the Elmish.Avalonia package from NuGet.
3) Replace the [`ViewLocator.fs`](https://github.com/JordanMarr/Elmish.Avalonia/blob/main/src/Samples/AvaloniaExample/ViewLocator.fs) with the one from from the [AvaloniaExample project](https://github.com/JordanMarr/Elmish.Avalonia/tree/main/src/Samples/AvaloniaExample). This makes it easier to bind the view/viewmodel and start the Elmish loop using convention.
   Looking at the [AvaloniaExample project](https://github.com/JordanMarr/Elmish.Avalonia/tree/main/src/Samples/AvaloniaExample), this allows us to bind the `MainView.axaml` `Content` via the `ViewLocator` to locate the appropriate view and start the Elmish loop.

# Sample Project
Please view the [AvaloniaExample project](https://github.com/JordanMarr/Elmish.Avalonia/tree/v2-beta/src/Samples/AvaloniaExample).


# Elmish Program Extensions
Opening the `Elmish.Avalonia` and `Elmish` namespaces adds the following extensions to `Program`:

### Program.mkAvaloniaProgram
Creates an Avalonia program via Program.mkProgram.

```F#
Program.mkAvaloniaProgram init update
|> Program.runView this view
```

### Program.mkAvaloniaSimple
Creates an Avalonia program via Program.mkSimple.

```F#
override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
   Program.mkAvaloniaSimple init update
   |> Program.runView this view
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
override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
   Program.mkAvaloniaSimple init update
   |> Program.withSubscription subscriptions
   |> Program.runView this view
```

### Program.terminateOnViewUnloaded
Configures `Program.withTermination` using the given `'Msg`, and fires the terminate `'Msg` when the `view` is `Unloaded`.

NOTE: You must create a `Terminate` `'Msg` that will be registered to trigger loop termination.

```F#
let update (msg: Msg) (model: Model) =
    // ...
    | Terminate -> model // This is just a stub Msg that needs to exist -- it doesn't need to do anything.
```

```F#
override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
   Program.mkAvaloniaSimple init update
   |> Program.withSubscription subscriptions
   |> Program.terminateOnViewUnloaded this Terminate
   |> Program.runView this view
```

