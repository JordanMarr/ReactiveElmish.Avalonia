module Elmish.Avalonia.ViewModel

/// Creates a design-time view model using the given model and bindings.
let designInstance (model: 'model) (bindings: Binding<'model, 'msg> list) =
  let args = ViewModelArgs.simple model

  DictionaryViewModel(args, bindings) |> box