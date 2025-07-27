using s2protocol.NET;

Console.WriteLine("Hello, World!");

var replayPath = @"C:\Users\pax77\Documents\StarCraft II\Accounts\107095918\2-S2-1-226401\Replays\Multiplayer\Direct Strike (8697).SC2Replay";

ReplayRawDecoder.Decode(replayPath);