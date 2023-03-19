module AvaloniaExample.ViewModels.Messaging

open System.Reactive

type GlobalMsg = 
    | GoHome

/// Provides pub/sub messaging between view models.
let bus = new Subjects.Subject<GlobalMsg>()
