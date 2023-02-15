# Elmish.Avalonia
Static Avalonia views for Elmish programs

This project is a port of the awesome [Elmish.WPF](https://github.com/elmish/Elmish.WPF) library.
Most of the codebase is directly copied, but the WPF bits have been replaced with Avalonia bits and adapted where necessary.
There is a sample project here to get you started, but there are a plethora of [Elmish.WPF examples](https://github.com/elmish/Elmish.WPF/tree/master/src/Samples) that you can also refer to.

## Why?
Avalonia.FuncUI already exists and it is fantastic. So then why make an Elmish.Avalonia port?

### Benefits
* Some people may prefer using xaml, and it can be an easier sell for some teams due to the immediate availability of all community controls.
* Bindings have to be created for controls. Some controls (such as the DataGrid) are too "OOP" and are difficult or impossible to create bindings for. This makes a library like Elmish.Avalonia very useful.
* There is not currently a reliable design preview tool (there is a very experimental project, but the author has stopped working on it and it's way too complex for me to want to mess with it). For me to do any kind of real project work with Avalonia and F#, I feel that a design preview is a necessity. After recently trying Elmish.WPF, I fell in love with the designVM preview functionality.

### Other reasons
* Avalonia UI is a big deal in the .NET OSS community; it is always nice for F# community to be able to participate in the latest and greatest with as many options as possible.
* Avalonia already provides first class templates to create an F# project that include creating .axaml views within the same project! (Not possible with WPF!)
* While the built-in F# templates do allow you to do classic MVVM style, Elmish is more powerful and has become the standard for F# UI projects.
* The "Avalonia UI for Visual Studio 2022" extension provides a xaml preview pane that works with F#! 😄 (Also not possible with WPF!)
* The Elmish.WPF "designVM" concept works with this extension very well. This allows you populate your "Design Preview" window with the defaults from your Elmish `init` function!
* Keeping with tradition that the F# community will [provide important libraries, developer tools and workload support](https://learn.microsoft.com/en-us/dotnet/fsharp/strategy).

# Sample App
The included sample app shows a obligatory Elmish counter app, and also the Avalonia DataGrid control.

## Design View
![image](https://user-images.githubusercontent.com/1030435/219144094-79155085-6b5c-43e4-a0ea-c5087e68a6bc.png)

## Runtime View
![image](https://user-images.githubusercontent.com/1030435/219145003-b4168921-ddab-41bc-92ea-d3f432fbc844.png)

# Project Setup
As of now, the easiest way to get started is to copy the included `AvaloniaExample` project.

Here are some key differences from the default F# MVVM template in the `AvaloniaExample` project:

1) The `Avalonia.ReactiveUI` package has been replaced with the `Elmish.Avalonia` package.
2) In `Program.fs`, `.UseReactiveUI()` has been replaced with `.UseElmishBindings()`.
3) The template `ViewLocator` has code has been modified to work with a custom `IStart` that makes it easier to bind the view/viewmodel and start the Elmish loop.
4) The `MainView.axaml` `Content` property binding utilizes the `ViewLocator` to locate the appropriate view and start the Elmish loop.

