using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using ReactiveElmish;


using ReactiveUI;

namespace WpfExample.ViewModels;

public class CounterStore() : ReactiveStore<CounterModel>(CounterModel.Init())
{
    public void Increment() => 
        Update(m => m with { Count = m.Count + 1, Actions = m.Actions.Append(new Action("Increment", DateTime.Now)).ToArray() });

    public void Decrement() => 
        Update(m => m with { Count = m.Count - 1, Actions = m.Actions.Append(new Action("Decrement", DateTime.Now)).ToArray() });

    public void Reset() => 
        Update(m => CounterModel.Init());
}

public record CounterModel(int Count, Action[] Actions) 
{
    public static CounterModel Init() => new CounterModel(0, [new Action("Initialized", DateTime.Now)]);
}

public record Action(string Description, DateTime Timestamp);

public class CounterViewModel : ReactiveObject
{
    readonly CounterStore store = new CounterStore();
    public CounterViewModel() => Rx = new ReactiveBindingsCS(this.RaisePropertyChanged);                
    ReactiveBindingsCS Rx { get; }

    public int Count => Rx.Bind(store, s => s.Count);
    public ReadOnlyCollection<Action> Actions => Rx.BindList(store, s => s.Actions);
    public IRelayCommand Increment { get => new RelayCommand(store.Increment); }
    public IRelayCommand Decrement { get => new RelayCommand(store.Decrement); }
    public IRelayCommand Reset { get => new RelayCommand(store.Reset); }
}
