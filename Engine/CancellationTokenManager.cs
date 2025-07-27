using System;
using System.Collections.Concurrent;
using System.Threading;
using Rage; 

namespace DialogueSystem.Engine; 

internal static class CancellationTokenManager
{
    private static readonly ConcurrentBag<CancellationTokenSource> _activeCancellationTokenSources =
        new ConcurrentBag<CancellationTokenSource>();

    private static bool _appDomainUnloadEventRegistered = false;

    static CancellationTokenManager()
    {
        InitializeAppDomainUnloadHandler();
    }

    private static void InitializeAppDomainUnloadHandler()
    {
        if (!_appDomainUnloadEventRegistered)
        {
            try
            {
                AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
                Game.LogTrivial("CancellationTokenManager: AppDomain.CurrentDomain.DomainUnload event registered.");
                _appDomainUnloadEventRegistered = true;
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"CancellationTokenManager: Failed to register AppDomain.DomainUnload event: {ex}");
            }
        }
    }

    private static void CurrentDomain_DomainUnload(object sender, EventArgs e)
    {
        Game.LogTrivial("CancellationTokenManager: AppDomain.CurrentDomain.DomainUnload event fired. Initiating cleanup.");
        AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
        _appDomainUnloadEventRegistered = false; 
        PerformCleanup();
        Game.LogTrivial("CancellationTokenManager: AppDomain cleanup complete.");
    }

    internal static void RegisterSource(CancellationTokenSource source)
    {
        if (source != null)
        {
            _activeCancellationTokenSources.Add(source);
            Game.LogTrivial($"CancellationTokenManager: Registered CancellationTokenSource (Hash: {source.GetHashCode()}). Total: {_activeCancellationTokenSources.Count}");
        }
    }

    internal static void PerformCleanup()
    {
        Game.LogTrivial($"CancellationTokenManager: Cleaning up {_activeCancellationTokenSources.Count} active CancellationTokenSources.");
        while (_activeCancellationTokenSources.TryTake(out CancellationTokenSource source))
        {
            try
            {
                if (source != null)
                {
                    if (!source.IsCancellationRequested)
                    {
                        source.Cancel(); 
                    }
                    source.Dispose(); 
                    Game.LogTrivial($"CancellationTokenManager: Disposed CancellationTokenSource (Hash: {source.GetHashCode()}).");
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"CancellationTokenManager: Error disposing CancellationTokenSource (Hash: {source?.GetHashCode()}): {ex}");
            }
        }
    }
}
