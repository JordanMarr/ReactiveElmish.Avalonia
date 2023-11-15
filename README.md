# Elmish.Avalonia [![NuGet version (Elmish.Avalonia)](https://img.shields.io/nuget/v/Elmish.Avalonia.svg?style=flat-square)](https://www.nuget.org/packages/Elmish.Avalonia/)
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
* While the built-in F# templates do allow you to do classic MVVM style, Elmish is more powerful and has become the standard for F# UI projects.
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
Instead of using the V1 bindings, you now create a more standard view model that has bindable properties. A new `Bind` method will take care of binding your view model properties to Elmish model projections. 

![image](https://github.com/JordanMarr/Elmish.Avalonia/assets/1030435/8e989018-3b81-443e-b782-d06f52067654)


### V2 Design Highlights
* Works with Avalonia [Compiled Bindings](https://docs.avaloniaui.net/docs/next/basics/data/data-binding/compiled-bindings#enable-and-disable-compiled-bindings) for better performance and compile-time type checking in the views. With Compiled Bindings enabled, the build will fail if the view references a binding that doesn't exist in the VM! (The previous `DictionaryViewModel` brought over from Elmish.WPF was not able to take advantage of this because it relied on reflection-based bindings.)
* More standard looking view model pattern while still maintaining the power of Elmish. For example, you can now create an instance of an Elmish view model and actually inspect its properties from the outside -- and even read / write to the properties in OOP fashion. (The fact that a view model is using Elmish internally should not matter because it's an implementation detail.) This is a perfect example of the benefits of OOP + FP side-by-side.
* The existing Elmish.WPF `DictionaryViewModel` was not able to bind to DataGrid row columns. The workarounds were pretty cumbersome, imo. Having more typical view models resolves this issue.
* Elmish.Avalonia now takes a dependency on the Avalonia.ReactiveUI library. (The new `ReactiveElmishViewModel` class inherits from `ReactiveObject`.) Since this is the default view model library for Avalonia, this makes it easier to take advantage of existing patterns when needed.

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
This pattern will dispose your subscriptions when the view is `Unloaded`.

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

### Program.runView
Binds the vm to the view and then runs the Elmish program.
