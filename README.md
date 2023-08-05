# Elmish.Avalonia [![NuGet version (Elmish.Avalonia)](https://img.shields.io/nuget/v/Elmish.Avalonia.svg?style=flat-square)](https://www.nuget.org/packages/Elmish.Avalonia/)
Static Avalonia views for Elmish programs

This project is a port of the awesome [Elmish.WPF](https://github.com/elmish/Elmish.WPF) library.
Most of the codebase is directly copied, but the WPF bits have been replaced with Avalonia bits and adapted where necessary.
There is a sample project here to get you started, but there is a plethora of [Elmish.WPF examples](https://github.com/elmish/Elmish.WPF/tree/master/src/Samples) that you can also refer to.

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

# Sample App
The included sample app shows a obligatory Elmish counter app, and also the Avalonia DataGrid control.

## Design View
Don't forget to install the "Avalonia for Visual Studio 2022" extension.
JetBrains Rider also supports Avalonia previews out-of-the-box!
https://docs.avaloniaui.net/docs/getting-started/ide-support

![image](https://user-images.githubusercontent.com/1030435/219173023-a47d5d9b-8926-4f9d-833b-1406661e1c82.png)

## Runtime View
![image](https://user-images.githubusercontent.com/1030435/219145003-b4168921-ddab-41bc-92ea-d3f432fbc844.png)

## View Model
![image](https://github.com/JordanMarr/Elmish.Avalonia/assets/1030435/975bc487-b5ff-4e10-a968-a249cd11488f)


## Master View
The sample project uses the `ViewLocator` to instantiate the view, bind the Elmish view model and start the Elmish loop.
Since the design preview is set for both the `MasterView` and the `CounterView`, we are able to see the counter on the `MasterView` design preview!

![image](https://user-images.githubusercontent.com/1030435/219421157-cfa2254c-a1aa-417c-9a8b-69a5bc4ef038.png)


# Project Setup

Steps to create a new project:

1) Create a new project using the [Avalonia .NET MVVM App Template for F#](https://github.com/AvaloniaUI/avalonia-dotnet-templates).
2) Install the Elmish.Avalonia package from NuGet.
3) Remove the `Avalonia.ReactiveUI` package.
4) In `Program.fs`, replace `.UseReactiveUI()` with `.UseElmishBindings()`.
5) Replace the [`ViewLocator.fs`](https://github.com/JordanMarr/Elmish.Avalonia/blob/main/src/Samples/AvaloniaExample/ViewLocator.fs) with the one from from the [AvaloniaExample project](https://github.com/JordanMarr/Elmish.Avalonia/tree/main/src/Samples/AvaloniaExample). This makes it easier to bind the view/viewmodel and start the Elmish loop using convention.
   Looking at the [AvaloniaExample project](https://github.com/JordanMarr/Elmish.Avalonia/tree/main/src/Samples/AvaloniaExample), this allows us to bind the `MainView.axaml` `Content` via the `ViewLocator` to locate the appropriate view and start the Elmish loop.

# AvaloniaProgram
The `AvaloniaProgram` contains functions that configure an Elmish program.

* `AvaloniaProgram.startElmishLoop` - Starts the Elmish loop and binds the given view to the bindings.

# ElmishViewModel
The `ElmishViewModel` contains functions that configure an `IElmishViewModel`. 
The `IElmishViewModel` has a single method, `StartElmishLoop`, which takes an Avalonia view, binds it with the bindings and starts the Elmish loop.
Use of the `ElmishViewModel` is optional and exists primarily to facilitate the `ViewLocator` pattern.

* `ElmishViewModel.create` - Creates an `ElmishViewModel<'model, 'msg>`

* `ElmishViewModel.terminateOnViewUnloaded` - Creates an Elmish subscription when the view `Unloaded` event fires that dispatches the passed-in termination `'msg` to terminate the Elmish loop.

* `ElmishViewModel.subscribe` - Adds an Elmish subscription.
