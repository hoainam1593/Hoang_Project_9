using System;

public static class GameEventMgr
{
    public static Dispatcher<GameEvent> GED = new Dispatcher<GameEvent>();
}