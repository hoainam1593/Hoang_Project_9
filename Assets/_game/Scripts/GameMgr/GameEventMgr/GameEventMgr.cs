using System;

public static class GameEventMgr
{
    private static Dispatcher<GameEvent> GED = new Dispatcher<GameEvent>();
    
    //Extension:
    public static void RegisterEvent(this IRegister register, GameEvent gameEvent, Action<object> callback)
    {
        GED.Register(gameEvent, callback);
    }
    public static void UnRegisterEvent(this IRegister register, GameEvent gameEvent, Action<object> callback)
    {
        GED.UnRegister(gameEvent, callback);
    }
    public static void DispatcherEvent(this IDispatcher dispatcher, GameEvent gameEvent, object data)
    {
        GED.DispatcherEvent(gameEvent, data);
    }
}
