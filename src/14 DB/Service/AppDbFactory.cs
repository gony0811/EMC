using Autofac;
using System;

namespace EGGPLANT
{
    public interface IAppDbFactory
    {
        AppDbHandle Create(); // using으로 감싸서 쓰면 스코프까지 정리
    }

    public sealed class AppDbHandle : IDisposable, IAsyncDisposable
    {
        public AppDb Db { get; }
        private readonly ILifetimeScope _scope;
        internal AppDbHandle(ILifetimeScope scope, AppDb db) { _scope = scope; Db = db; }
        public void Dispose() => _scope.Dispose();
        public ValueTask DisposeAsync() { _scope.Dispose(); return ValueTask.CompletedTask; }
    }

    public sealed class AppDbFactory : IAppDbFactory
    {
        private readonly ILifetimeScope _root;
        public AppDbFactory(ILifetimeScope root) => _root = root;

        public AppDbHandle Create()
        {
            var scope = _root.BeginLifetimeScope();
            var db = scope.Resolve<AppDb>();
            return new AppDbHandle(scope, db);
        }
    }
}
