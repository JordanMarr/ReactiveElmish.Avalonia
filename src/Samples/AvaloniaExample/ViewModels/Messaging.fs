module AvaloniaExample.ViewModels.Messaging

open System.Reactive

type GlobalMsg = 
    | GoHome
    | SetCount of int

/// Provides pub/sub messaging between view models.
let bus = new Subjects.Subject<GlobalMsg>()
