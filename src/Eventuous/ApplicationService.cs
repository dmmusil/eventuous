using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Eventuous {
    [PublicAPI]
    public abstract class ApplicationService<T, TState, TId>
        where T : Aggregate<TState, TId>, new()
        where TState : AggregateState<TState, TId>, new()
        where TId : AggregateId {
        readonly IAggregateStore _store;
        readonly HandlersMap<T>  _handlers = new();
        readonly IdMap<TId>      _getId    = new();

        protected ApplicationService(IAggregateStore store) => _store = store;

        protected void OnNew<TCommand>(Action<T, TCommand> action) where TCommand : class
            => _handlers.Add(
                typeof(TCommand), 
                new RegisteredHandler<T>(ExpectedState.New, (aggregate, cmd) => action(aggregate, (TCommand) cmd))
            );

        protected void OnExisting<TCommand>(Func<TCommand, TId> getId, Action<T, TCommand> action)
            where TCommand : class {
            _handlers.Add(
                typeof(TCommand), 
                new RegisteredHandler<T>(ExpectedState.Existing, (aggregate, cmd) => action(aggregate, (TCommand) cmd))
            );

            _getId.TryAdd(typeof(TCommand), cmd => getId((TCommand) cmd));
        }

        protected void OnAny<TCommand>(Func<TCommand, TId> getId, Action<T, TCommand> action)
            where TCommand : class {
            _handlers.Add(
                typeof(TCommand), 
                new RegisteredHandler<T>(ExpectedState.Any, (aggregate, cmd) => action(aggregate, (TCommand) cmd))
            );

            _getId.TryAdd(typeof(TCommand), cmd => getId((TCommand) cmd));
        }

        public async Task<Result<T, TState, TId>> Handle<TCommand>(TCommand command)
            where TCommand : class {
            if (!_handlers.TryGetValue(typeof(TCommand), out var registeredHandler)) {
                throw new Exceptions.CommandHandlerNotFound(typeof(TCommand));
            }

            var aggregate = registeredHandler.ExpectedState switch {
                ExpectedState.Any      => await TryLoad(),
                ExpectedState.Existing => await Load(),
                ExpectedState.New      => new T()
            };

            registeredHandler.Handler(aggregate, command);

            await _store.Store(aggregate);

            return new OkResult<T, TState, TId>(aggregate.State, aggregate.Changes);

            Task<T> Load() {
                var id = _getId[typeof(TCommand)](command);
                return _store.Load<T>(id);
            }

            async Task<T> TryLoad() {
                try {
                    return await Load();
                }
                catch (Exceptions.AggregateNotFound<T>) {
                    return new T();
                }
            }
        }
    }

    record RegisteredHandler<T>(ExpectedState ExpectedState, Action<T, object> Handler);

    class HandlersMap<T> : Dictionary<Type, RegisteredHandler<T>> { }

    class IdMap<T> : Dictionary<Type, Func<object, T>> { }

    enum ExpectedState {
        New,
        Existing,
        Any
    }
}