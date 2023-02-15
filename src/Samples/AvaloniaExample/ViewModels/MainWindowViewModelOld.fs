namespace AvaloniaFSharp.ViewModels

type MainWindowViewModel() =
    inherit ViewModelBase()

    member val Greeting = "Welcome to Avalonia!" with get, set

    member this.ClickCmd() = 
        this.Greeting <- "Clicked!"